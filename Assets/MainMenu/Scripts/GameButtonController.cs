using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButtonController : MonoBehaviour
{
    public string GameId = "";
    public System.Action<string> OnClick;

    public void InvokeOnClickEvent()
    {
        OnClick.Invoke(GameId);
    }
}
