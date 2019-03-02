using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootloader : MonoBehaviour
{
    void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "Load"))
        {
            SceneManager.LoadScene("Login/Login");
        }
    }
}
