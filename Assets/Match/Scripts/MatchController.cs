using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchController : MonoBehaviour
{
    [SerializeField]
    private LoadingController loadingController;

    // Start is called before the first frame update
    void Start()
    {
        loadingController.Show();

        // Start api call to get match info
        StartCoroutine(MakeApiCall("", OnSuccess, OnFail));

        // On match info recieved, hide loading screen
    }

    private void OnSuccess(string message)
    {
        loadingController.Hide();
    }

    private void OnFail(string message)
    {

    }

    private string GetMockMatchPayload()
    {
        return @"{
            currentTurn: "",
            xOwner: "",
            oOwner: "",
            gameState: {
                bottomRow: [
                    {ownerid: "", piece: ""},
                    {ownerid: "", piece: ""},
                    {ownerid: "", piece: ""}
                ],
                middleRow: [
                    {ownerid: "", piece: ""},
                    {ownerid: "", piece: ""},
                    {ownerid: "", piece: ""}
                ],
                topRow: [
                    {ownerid: "", piece: ""},
                    {ownerid: "", piece: ""},
                    {ownerid: "", piece: ""}
                ]
            }
        }";
    }

    private IEnumerator MakeApiCall(string apiEndpoint, Action<string> onSuccess, Action<string> onFail)
    {
        yield return new WaitForSeconds(1);
        onSuccess.Invoke(GetMockMatchPayload());
    }
}

[System.Serializable]
public class MatchPayload
{
    public string currentTurn;
    public string xOwner;
    public string oOwner;
    public GameStatePayload gameState; 
}

[System.Serializable]
public class GameStatePayload
{
    public CellStatePayload[] bottomRow;
    public CellStatePayload[] middleRow;
    public CellStatePayload[] topRow;
}

[System.Serializable]
public class CellStatePayload
{
    public string ownerid;
    public string piece;
}