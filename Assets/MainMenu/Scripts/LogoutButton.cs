using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutButton : MonoBehaviour
{
    public void Logout()
    {
        LoggedInUser.Instance.Logout();
        SceneManager.LoadScene("Login/Login");
    }
}
