using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using Google;

public class LoginWithGoogle : MonoBehaviour
{
    [SerializeField]
    private string WebClientId = "<your client id here>";

    private GoogleSignInConfiguration googleSignInConfiguration;
    private Firebase.Auth.FirebaseAuth authentication;

    private string createGoogleUserEndpoint = "https://us-central1-aws-tic-tac-toe.cloudfunctions.net/createGoogleUser?uuid={0}";

    private bool logInSuccessful = false;
    private bool doLoginApiCall = false;
    Firebase.Auth.FirebaseUser firebaseUser = null;

    void Awake()
    {
        googleSignInConfiguration = new GoogleSignInConfiguration
        {
            WebClientId = WebClientId,
            RequestIdToken = true
        };

        authentication = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    void Update()
    {
        if (doLoginApiCall)
        {
            // Only do call once
            // StartCoroutine does not work inside Task callback
            doLoginApiCall = false;
            string apiCall = string.Format(createGoogleUserEndpoint, firebaseUser.UserId);
            StartCoroutine(MakeAPICall(apiCall,
            status => {
                LoggedInUser.Instance.SetLoggedInUser(firebaseUser);
                LoggedInUser.Instance.SetAuth(authentication);
                logInSuccessful = true;
            },
            status => {
                Debug.LogError("Something went wrong :(");
            }));
        }

        if (logInSuccessful)
        {
            SceneManager.LoadScene("MainMenu/MainMenu");
        }
    }

    public void SignIn()
    {
        GoogleSignIn.Configuration = googleSignInConfiguration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        try
        {
            GoogleSignIn.DefaultInstance.SignIn()
            .ContinueWith(GetGoogleSignInIdToken)
            .ContinueWith(GetFirebaseCredential)
            .ContinueWith(FirebaseSignIn)
            .ContinueWith(GameSignIn);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Something went wrong with sign in");
            Debug.LogError(e);
        }
        
    }

    private void GameSignIn(Task<Task<Firebase.Auth.FirebaseUser>> userTask)
    {
        Debug.Log("Do game sign in");
        if (userTask.IsCanceled)
        {
            throw new System.Exception("Firebase user task was unexpectedly cancelled.");
        }
        else if (userTask.IsFaulted)
        {
            throw new System.Exception("Firebase user taks is faulted.");
        }
        else
        {
            firebaseUser = userTask.Result.Result;
            Debug.Log("Hello " + firebaseUser.DisplayName);
            doLoginApiCall = true;
        }
    }

    private IEnumerator MakeAPICall(string apiCall, Action<string> onSuccess, Action<string> onFail)
    {
        Debug.Log("Making api call to " + apiCall);
        UnityWebRequest apiRequest = UnityWebRequest.Get(apiCall);
        yield return apiRequest.SendWebRequest();

        if (apiRequest.isNetworkError || apiRequest.isHttpError)
        {
            onFail(apiRequest.error);
        }
        else
        {
            onSuccess(apiRequest.downloadHandler.text);
        }
    }

    private Firebase.Auth.Credential GetFirebaseCredential(Task<string> googleIdTokenTask)
    {
        Debug.Log("Get firebase credential");
        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdTokenTask.Result, null);

        if (credential == null)
        {
            throw new System.Exception("Could not get Google Auth provider credential. Id Token provided: " + googleIdTokenTask.Result);
        }
        else
        {
            Debug.Log(credential.ToString());
            return credential;
        }
    }

    private Task<Firebase.Auth.FirebaseUser> FirebaseSignIn(Task<Firebase.Auth.Credential> firebaseCredentialTask)
    {
        Debug.Log("Do firebase sign in");
        if (authentication == null)
        {
            throw new System.Exception("Firebase authenticator is null!");
        }
        else
        {
            Debug.Log("Success");
            return authentication.SignInWithCredentialAsync(firebaseCredentialTask.Result);
        }
    }

    private string GetGoogleSignInIdToken(Task<GoogleSignInUser> googleSignInTask)
    {
        Debug.Log("Get Google Sign In ID Token");
        if (googleSignInTask.IsFaulted)
        {
            using (IEnumerator<System.Exception> exceptionEnumerator = googleSignInTask.Exception.InnerExceptions.GetEnumerator())
            {
                if (exceptionEnumerator.MoveNext())
                {
                    throw exceptionEnumerator.Current as GoogleSignIn.SignInException;
                }
                else
                {
                    throw googleSignInTask.Exception;
                }
            }
        }
        else if (googleSignInTask.IsCanceled)
        {
            throw new System.Exception("Google sign in unexpectedly canceled.");
        }
        else
        {
            Debug.Log(googleSignInTask.Result.IdToken);
            return googleSignInTask.Result.IdToken;
        }
    }
}
