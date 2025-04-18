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

public class LoginWithGoogle : MonoBehaviour
{
    public string GoogleAPI = "382515883778-um96eqqrtp23j6sme0t0kgspheqskrg7.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    public TMP_Text Username, UserEmail;

    public Image UserProfilePic;
    private string imageUrl;
    private bool isGoogleSignInInitialized = false;

    [SerializeField] private GoogleAuth googleAuth;
   public DatabaseReference databaseReference;

    public string userId;

    public static LoginWithGoogle instance;
    public int totalCash;


    private void Awake()
    {
        if (instance == null)
        {

            instance = this;
        }
        
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
       
            Debug.Log("Testing Firebase Initialization...");
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"Firebase initialization failed: {task.Exception}");
                    return;
                }

                var dependencyStatus = task.Result;
                Debug.Log($"Dependency Status: {dependencyStatus}");
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    StartCoroutine(Assign());
                }
                else
                {
                    Debug.LogError($"Firebase dependencies not resolved: {dependencyStatus}");
                }
            });
        
    }

    public IEnumerator Assign()
    {
      
        FirebaseApp app = FirebaseApp.DefaultInstance;
        if (app == null)
        {
            app = FirebaseApp.Create();
        }
        Debug.Log("Firebase Initialized Successfully!");
        yield return new WaitForSeconds(1);
        Debug.Log("Firebase Initialized Successfully!2");
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        yield return new WaitForSeconds(1);
        //AddCoins(0);



    }
    // Inside your LoginWithGoogle or similar script

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
                    Debug.Log($"Fetched total cash: {currentCash} for user {userId}");
                    totalCash = currentCash;
                    if(MenuController.instance != null)
                    MenuController.instance.UpdateInStartTxts();
                }
                else
                {
                    Debug.LogWarning($"No total cash value found for user {userId}. Starting with 0.");
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError($"Failed to fetch total cash: {task.Exception}");
                return; 
            }

            int updatedCash = currentCash + newCoins;
            totalCash = updatedCash; 

           
            databaseReference.Child("users").Child(userId).Child("totalCash").SetValueAsync(updatedCash).ContinueWithOnMainThread(saveTask =>
            {
                if (saveTask.IsCompletedSuccessfully)
                {
                    Debug.Log($"Successfully saved total cash {updatedCash} for user {userId}");
                    Username.text = $"Coins: {updatedCash}";
                }
                else if (saveTask.IsFaulted)
                {
                    Debug.LogError($"Failed to save total cash: {saveTask.Exception}");
                    Username.text = "Failed to update coins";
                }
            });
        });
    }



    public void Login()
    {
        Username.text = "Logging in...";
        if (!isGoogleSignInInitialized)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                WebClientId = GoogleAPI,
                RequestEmail = true
            };

            isGoogleSignInInitialized = true;
        }
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = GoogleAPI
        };
        GoogleSignIn.Configuration.RequestEmail = true;

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        //   Username.text = "firbase section in... " + (auth == null);
        if (auth == null)
            InitFirebase();
        signIn.ContinueWith(task =>
        {

            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
                Debug.Log("Cancelled");
                Username.text = "Cancelled";
            }
            else if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);

                Debug.Log("Faulted " + task.Exception);
                Username.text = "Faulted ";
            }
            else
            {
                Username.text = "firbase section in... " + task.Result.UserId;
                UserEmail.text = "firbase section in... " + task.Result.Email;
                Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
                {
                    if (authTask.IsCanceled)
                    {
                        signInCompleted.SetCanceled();
                        Username.text = ("cancel auth ");
                        return;
                    }
                    else if (authTask.IsFaulted)
                    {
                        signInCompleted.SetException(authTask.Exception);
                        Username.text = ("Faulted In Auth ");
                        return;
                    }
                    //  else
                    {
                        signInCompleted.SetResult(((Task<FirebaseUser>)authTask).Result);
                        Debug.Log("Success");
                        googleAuth.ChangeTheKey();
                        StartCoroutine(DelayedCheckSignIn());
                        user = auth.CurrentUser;
                        Username.text = user.DisplayName;
                        UserEmail.text = user.Email;
                        userId = user.UserId;
                        AddCoins(0);
                        StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));
                    }
                });
            }
        });
    }

    IEnumerator DelayedCheckSignIn()
    {
        yield return null; // wait for 1 frame to ensure PlayerPrefs is updated
        yield return new WaitForSeconds(0.1f); // short delay just to be safe
        googleAuth.CheckFirstLaunch();
    }

    private string CheckImageUrl(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            return url;
        }
        return imageUrl;
    }

    IEnumerator LoadImage(string imageUri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUri);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            // Use the loaded texture here
            Debug.Log("Image loaded successfully");
            UserProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }
        else
        {
            Debug.Log("Error loading image: " + www.error);
        }


    }
}