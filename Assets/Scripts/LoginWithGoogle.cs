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
    private void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase Auth Initialized Successfully");
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                Username.text = "Could not resolve all Firebase";
            }
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