using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginWithEmailFormController : MonoBehaviour
{
    [SerializeField]
    private Text email;

    [SerializeField]
    private InputField password;

    private Firebase.Auth.FirebaseAuth auth;

    private bool logInSuccessful = false;

    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    void Update()
    {
        if(logInSuccessful)
        {
            SceneManager.LoadScene("MainMenu/MainMenu");
        }
    }

    public void Login()
    {
        Debug.Log(email.text);
        Debug.Log(password.text);
        auth.SignInWithEmailAndPasswordAsync(email.text, password.text)
            .ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsynce encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                LoggedInUser.Instance.SetLoggedInUser(newUser);
                logInSuccessful = true;
            });
        LoggedInUser.Instance.SetAuth(auth); // Added
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu/MainMenu");
    }

    public void GoToLoginWithEmailScene()
    {
        SceneManager.LoadScene("Login/LoginWithEmail");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("Login/Login");
    }
}
