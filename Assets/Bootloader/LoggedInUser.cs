using UnityEngine;


public class LoggedInUser : MonoBehaviour
{
    public static LoggedInUser Instance = null;

    private Firebase.Auth.FirebaseUser loggedInUser = null;
    private Firebase.Auth.FirebaseAuth auth = null;
    public string currentGameId = "";

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void SetLoggedInUser(Firebase.Auth.FirebaseUser user)
    {
        loggedInUser = user;
    }

    public void SetAuth(Firebase.Auth.FirebaseAuth auth)
    {
        this.auth = auth;
    }

    public string GetUserUID()
    {
        return loggedInUser.UserId;
    }

    public void Logout()
    {
        if (loggedInUser != null)
        {
            auth.SignOut();
        }
        Google.GoogleSignIn.DefaultInstance.SignOut();
        loggedInUser = null;
    }
}