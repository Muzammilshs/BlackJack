using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using Google;
using TMPro;
using UnityEngine.Networking;
using System.Collections.Generic;
using Firebase.Extensions;

public class FirebaseGoogleLogin : MonoBehaviour
{
    public string GoogleWebAPI = "953453247225-o1jshci3onsijj4n4bvh6v861ti9pdg4.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    public string imageUrl;

    public TMP_Text userNameText, userEmailText;
    public TMP_Text errorText;

    public Image profilePic;

    public GameObject loginPanel, profilePanel;

    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true,
            RequestEmail = true
        };

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
    }

    private void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void GoogleSignInClick()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        Debug.LogError("Calling SignIn");
        errorText.text = "Calling SignIn";

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
    }

    void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Faulted");
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Cancelled");
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                user = auth.CurrentUser;

                userNameText.text = user.DisplayName;
                userEmailText.text = user.Email;

                loginPanel.SetActive(false);
                profilePanel.SetActive(true);

                // StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));
            });
        }
    }


    private void SignInWithFirebase(GoogleSignInUser googleUser)
    {
        Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
        {
            if (authTask.IsCanceled || authTask.IsFaulted)
            {
                errorText.text = "Firebase Auth Failed.";
                Debug.LogError("Firebase Auth Error: " + authTask.Exception);
            }
            else
            {
                user = auth.CurrentUser;
                errorText.text = "Signed in as: " + user.DisplayName;
                Debug.Log("Firebase user: " + user.DisplayName);

                loginPanel.SetActive(false);
                profilePanel.SetActive(true);
                StartCoroutine(LoadImage());
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    IEnumerator LoadImage()
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            Debug.LogWarning("Image URL is empty.");
            yield break;
        }

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load profile image: " + request.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            profilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    public void OnSignOut()
    {
        userNameText.text = "";
        userEmailText.text = "";
        imageUrl = "";

        loginPanel.SetActive(true);
        profilePanel.SetActive(false);

        Debug.Log("Signing out...");
        GoogleSignIn.DefaultInstance.SignOut();
        auth.SignOut();
    }
}
