using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InteractWithSpace : MonoBehaviour
{
    public Button button;
    public Text buttonText;
    public GameController gameController;

    public void SetSpace()
    {
        buttonText.text = gameController.GetPlayerSide();
        button.interactable = false;
        gameController.EndTurn();
    }
}

//InteractWithSpace