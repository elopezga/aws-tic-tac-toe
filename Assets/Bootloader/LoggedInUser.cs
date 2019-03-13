using UnityEngine;

public class LoggedInUser : MonoBehaviour
{
    public static LoggedInUser Instance = null;

    private Firebase.Auth.FirebaseUser loggedInUser = null;

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void SetLoggedInUser(Firebase.Auth.FirebaseUser user)
    {
        loggedInUser = user;
    }

    public string GetUserUID()
    {
        return loggedInUser.UserId;
    }

    public void Logout()
    {
        loggedInUser = null;
    }
}