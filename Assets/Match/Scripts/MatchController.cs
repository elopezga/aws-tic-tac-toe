using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchController : MonoBehaviour
{
    [SerializeField]
    private LoadingController loadingController;

    [SerializeField]
    private GridController gridController;

    // Start is called before the first frame update
    void Start()
    {
        loadingController.Show();

        // Start api call to get match info
        StartCoroutine(MakeApiCall("", OnSuccess, OnFail));

        gridController.OnPiecePlaced += HandlePiecePlaced;
    }

    void OnDestroy()
    {
        gridController.OnPiecePlaced -= HandlePiecePlaced;
    }

    private void HandlePiecePlaced()
    {
        gridController.DisablePlacingPiece();

        Debug.Log("TODO: Send API Request");
    }

    private void OnSuccess(string payload)
    {
        loadingController.Hide();
        MatchPayload matchPayload = JsonUtility.FromJson<MatchPayload>(payload);

        gridController.SetState(new GridState(){
            bottomRow = matchPayload.gameState.bottomRow,
            middleRow = matchPayload.gameState.middleRow,
            topRow = matchPayload.gameState.topRow,
            currentTurnId = matchPayload.currentTurn,
            currentTurnPiece = GetCurrentTurnPiece(matchPayload)
        });

        if (isYourTurn(matchPayload))
        {
            gridController.EnablePlacingPiece();
        }
        else
        {
            gridController.DisablePlacingPiece();
        }
    }

    private string GetCurrentTurnPiece(MatchPayload payload)
    {
        string piece = "";

        if (payload.currentTurn == payload.oOwner)
        {
            piece = "O";
        }
        else
        {
            piece = "X";
        }

        return piece;
    }

    private bool isYourTurn(MatchPayload payload)
    {
        return true;
        //return payload.currentTurn == LoggedInUser.Instance.GetUserUID();
    }

    private void OnFail(string message)
    {

    }

    private string GetMockMatchPayload()
    {
        return @"{
            ""currentTurn"": ""ME"",
            ""xOwner"": ""ME"",
            ""oOwner"": ""YOU"",
            ""gameState"": {
                ""bottomRow"": [
                    {""ownerid"": ""ME"", ""piece"": ""X""},
                    {""ownerid"": ""YOU"", ""piece"": ""O""},
                    {""ownerid"": ""YOU"", ""piece"": ""O""}
                ],
                ""middleRow"": [
                    {""ownerid"": """", ""piece"": ""-""},
                    {""ownerid"": ""ME"", ""piece"": ""X""},
                    {""ownerid"": ""YOU"", ""piece"": ""O""}
                ],
                ""topRow"": [
                    {""ownerid"": """", ""piece"": ""-""},
                    {""ownerid"": """", ""piece"": ""-""},
                    {""ownerid"": """", ""piece"": ""-""}
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