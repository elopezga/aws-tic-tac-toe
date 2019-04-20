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
            .ContinueWith( firebaseUserTask => {
                if (firebaseUserTask.IsCanceled)
                {
                    throw new System.Exception("Firebase user task was unexpectedly cancelled.");
                }
                else if (firebaseUserTask.IsFaulted)
                {
                    throw new System.Exception("Firebase user task is faulted.");
                }
                else
                {
                    Firebase.Auth.FirebaseUser newUser = firebaseUserTask.Result.Result;
                    
                    string apiCall = string.Format(createGoogleUserEndpoint, newUser.UserId);
                    StartCoroutine(MakeAPICall(apiCall,
                    status => {
                        if (status == "true")
                        {
                            LoggedInUser.Instance.SetLoggedInUser(newUser);
                            LoggedInUser.Instance.SetAuth(authentication);
                            logInSuccessful = true;
                        }
                    },
                    status => {
                        Debug.LogError("Something went wrong :(");
                    }));
                }
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
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
        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdTokenTask.Result, null);

        if (credential == null)
        {
            throw new System.Exception("Could not get Google Auth provider credential. Id Token provided: " + googleIdTokenTask.Result);
        }
        else
        {
            return credential;
        }
    }

    private Task<Firebase.Auth.FirebaseUser> FirebaseSignIn(Task<Firebase.Auth.Credential> firebaseCredentialTask)
    {
        if (authentication == null)
        {
            throw new System.Exception("Firebase authenticator is null!");
        }
        else
        {
            return authentication.SignInWithCredentialAsync(firebaseCredentialTask.Result);
        }
    }

    private string GetGoogleSignInIdToken(Task<GoogleSignInUser> googleSignInTask)
    {
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
            return googleSignInTask.Result.IdToken;
        }
    }
}
