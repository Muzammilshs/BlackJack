using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Google;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Database;
using System;
using UnityEngine.SceneManagement;

public class LoginWithGoogle : MonoBehaviour
{
    #region Fields

    // Google API client ID
    public string GoogleAPI = "169344897492-ca16hmmio829q8rllakn574heqmvdg1a.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;

    // Firebase authentication and user references
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    // UI references for user info
    public TMP_Text Username, UserEmail;
    public Image UserProfilePic;
    private string imageUrl;
    private bool isGoogleSignInInitialized = false;

    // Reference to GoogleAuth script for first launch logic
    [SerializeField] private GoogleAuth googleAuth;

    // Firebase Realtime Database reference
    public DatabaseReference databaseReference;

    // User and coin data
    public string userId;
    public static LoginWithGoogle instance;
    public int totalCash;

    // UI panels
    public GameObject loadingPanel;
    public GameObject googleLoginPanel;
    public MenuController menuController;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Singleton pattern to persist instance across scenes
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Show loading panel while initializing
        loadingPanel.SetActive(true);

        // Initialize Firebase and check for existing user
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to initialize Firebase: " + task.Exception);
                return;
            }

            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            auth = FirebaseAuth.DefaultInstance;
            Debug.Log("Firebase Initialized");

            // Check if user info is stored locally
            string id = PlayerPrefs.GetString("UserName");
            if (!string.IsNullOrEmpty(id))
            {
                userId = id;
                Username.text = PlayerPrefs.GetString("DisplayName");
                UserEmail.text = PlayerPrefs.GetString("Email");
                AddCoins(0);
            }
            else
            {
                loadingPanel.SetActive(false);
            }

#if UNITY_EDITOR
            AddCoins(0);
#endif
        });
    }

    #endregion

    #region Google Sign-In Methods

    /// <summary>
    /// Initiates the Google login process.
    /// </summary>
    public void Login()
    {
        LoginAsync();
    }

    /// <summary>
    /// Handles the asynchronous Google Sign-In and Firebase authentication.
    /// </summary>
    public async void LoginAsync()
    {
        Username.text = "Logging in...";
        Debug.Log("Login started");

        // Initialize Google Sign-In configuration if not already done
        if (!isGoogleSignInInitialized)
        {
            Debug.Log("Initializing Google Sign-In Configuration...");
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                WebClientId = GoogleAPI,
                RequestEmail = true
            };
            isGoogleSignInInitialized = true;
        }
        else
        {
            Debug.Log("Google Sign-In already initialized.");
        }

        try
        {
            Debug.Log("Attempting Google Sign-In...");
            GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn();
            Debug.Log("Google Sign-In Success. UserId: " + googleUser.UserId);
            Username.text = "Google Sign-In Success: " + googleUser.UserId;

            Debug.Log("Creating Firebase Credential...");
            loadingPanel.SetActive(true);
            Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
            Debug.Log("Signing into Firebase with Credential...");
            FirebaseUser newUser = await auth.SignInWithCredentialAsync(credential);
            Debug.Log("Firebase Authentication Success");

            // Post Firebase login success
            Debug.Log("Updating UI and local variables...");
            googleAuth.ChangeTheKey();
            StartCoroutine(DelayedCheckSignIn());

            user = auth.CurrentUser;
            Username.text = user.DisplayName;
            UserEmail.text = user.Email;
            userId = user.UserId;
            PlayerPrefs.SetString("UserName", userId);
            PlayerPrefs.SetString("Email", user.Email);
            PlayerPrefs.SetString("DisplayName", user.DisplayName);

            Debug.Log($"User Info: {user.DisplayName} ({user.Email}), ID: {userId}");

            AddCoins(0);
            // Uncomment this if you implement photo loading
            // StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));
            googleLoginPanel.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.LogError("Login Failed: " + ex.Message);
            Username.text = "Login failed.";
            loadingPanel.SetActive(false);
        }

        Debug.Log("Login() function end reached.");
    }

    #endregion

    #region Database and Coin Management

    /// <summary>
    /// Fetches and updates the user's coin balance in the database.
    /// </summary>
    /// <param name="newCoins">Amount of coins to add.</param>
    public void AddCoins(int newCoins)
    {
        if (databaseReference == null)
        {
            Debug.LogError("Database reference is null. Ensure Firebase is initialized.");
            return;
        }

        databaseReference.Child("users").Child(userId).Child("totalCash").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            int currentCash = 0;
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists && int.TryParse(snapshot.Value.ToString(), out currentCash))
                {
                    totalCash = currentCash;
                    LocalSettingBlackJack.SetTotalCashLocal(currentCash);
                    if (MenuController.instance != null)
                        MenuController.instance.UpdateInStartTxts();
                }
                else
                {
                    Debug.LogWarning($"No total cash value found for user {userId}. Starting with 30000.");
                    currentCash = 300;  // <<< Start new user with 3000
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError($"Failed to fetch total cash: {task.Exception}");
                return;
            }

            int updatedCash = currentCash + newCoins;

            totalCash = updatedCash;
            LocalSettingBlackJack.SetTotalCashLocal(updatedCash);
            databaseReference.Child("users").Child(userId).Child("totalCash").SetValueAsync(updatedCash).ContinueWithOnMainThread(saveTask =>
            {
                if (saveTask.IsCompletedSuccessfully)
                {
                    Username.text = $"Coins: {updatedCash}";
                    LocalSettingBlackJack.GetTotalCash();
                    if (SceneManager.GetActiveScene().buildIndex == 0)
                    {
                        menuController.UpDateTotalChipsTxts();
                        if (loadingPanel.activeInHierarchy)
                            loadingPanel.SetActive(false);
                    }
                    else
                    {
                        Rm.Instance.potHandler.UpDateCashTxt(updatedCash);
                    }
                }
                else if (saveTask.IsFaulted)
                {
                    Debug.LogError($"Failed to save total cash: {saveTask.Exception}");
                    Username.text = "Failed to update coins";
                }
            });
        });
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Waits briefly before checking first launch logic after sign-in.
    /// </summary>
    IEnumerator DelayedCheckSignIn()
    {
        yield return new WaitForSeconds(0.1f); // tiny wait to ensure Firebase is ready
        googleAuth.CheckFirstLaunch();
    }

    /// <summary>
    /// Loads a user profile image from a URL and sets it to the UI.
    /// </summary>
    IEnumerator LoadImage(string imageUri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUri);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            UserProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Debug.Log("Image loaded successfully");
        }
        else
        {
            Debug.LogError("Error loading image: " + www.error);
        }
    }

    #endregion
}
