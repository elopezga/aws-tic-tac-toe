using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Google;

public class LoginWithGoogle : MonoBehaviour
{
    [SerializeField]
    private string WebClientId = "<your client id here>";

    private GoogleSignInConfiguration googleSignInConfiguration;
    private Firebase.Auth.FirebaseAuth authentication;

    void Awake()
    {
        googleSignInConfiguration = new GoogleSignInConfiguration
        {
            WebClientId = WebClientId,
            RequestIdToken = true
        };

        authentication = Firebase.Auth.FirebaseAuth.DefaultInstance;
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
                }
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
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
