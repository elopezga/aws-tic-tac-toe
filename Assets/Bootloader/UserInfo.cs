using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    public static UserInfo Instance = null;

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }
}
