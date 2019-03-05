using UnityEngine;

public class LoggedInUser : MonoBehaviour
{
    public static LoggedInUser Instance = null;

    private static Firebase.Auth.FirebaseUser loggedInUser = null;

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public static void SetLoggedInUser(Firebase.Auth.FirebaseUser user)
    {
        loggedInUser = user;
    }

    public static string GetUserUID()
    {
        return loggedInUser.UserId;
    }
}