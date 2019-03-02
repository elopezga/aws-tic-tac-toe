using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignUpFormController : MonoBehaviour
{
    [SerializeField]
    private Text email;

    [SerializeField]
    private Text password;

    private Firebase.Auth.FirebaseAuth auth;

    void Start()
    {   
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void SignUp()
    {
        auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text)
            .ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsynce was canceled");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                // Firebase user has been created
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

                SceneManager.LoadScene("Login/Login");
            });
    }
}
