using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootloader : MonoBehaviour
{
    void OnGUI()
    {
        if (GUI.Button(new Rect(50f, 50f, 400f, 400f), "Play!"))
        {
            SceneManager.LoadScene("Login/Login");
        }
    }
}
