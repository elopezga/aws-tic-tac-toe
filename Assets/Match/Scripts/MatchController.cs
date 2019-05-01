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
    private TurnController turnController;

    [SerializeField]
    private GridController gridController;

    private string apiEndpointHost = "https://us-central1-aws-tic-tac-toe.cloudfunctions.net";
    private string getGameStateEndpointFormat = "/getgameinfo?game={0}";
    private string setGameStateEndpointFormat = "/setgamestate?gameuuid={0}";

    // Start is called before the first frame update
    void Start()
    {
        loadingController.Show();

        Debug.Log(LoggedInUser.Instance.currentGameId);

        // Start api call to get match info
        string apiEndpoint = apiEndpointHost + getGameStateEndpointFormat;
        string apiCall = string.Format(apiEndpoint, LoggedInUser.Instance.currentGameId);

        StartCoroutine(MakeApiCall(apiCall, HandleStartGame, ErrorStartingGame));

        gridController.OnStateUpdate += HandlePiecePlaced; // State change happens when piece is placed;
    }

    void OnDestroy()
    {
        gridController.OnStateUpdate -= HandlePiecePlaced;
    }

    private void HandleStartGame(string payload)
    {
        MatchPayload match = JsonUtility.FromJson<MatchPayload>(payload);

        loadingController.Hide();

        string currentTurnDisplay = "";
        string currentTurnPieceDisplay = "";
        if (string.IsNullOrEmpty(match.winner) && match.draw == false)
        {
            currentTurnDisplay = (match.currentTurnId == LoggedInUser.Instance.GetUserUID()) ? "Your Turn" : "Opponent Turn";
            currentTurnPieceDisplay = (match.currentTurnId == match.xOwner) ? "X" : "O";
        }
        else if (!string.IsNullOrEmpty(match.winner))
        {
            currentTurnDisplay = (match.winner == LoggedInUser.Instance.GetUserUID()) ? "You Won!" : "Opponent Won!";
        }
        else
        {
            currentTurnDisplay = "It's a draw!";
        }
        turnController.SetState(new TurnState(){
            CurrentTurn = currentTurnDisplay,
            CurrentTurnPiece = currentTurnPieceDisplay
        });

        gridController.SetState(new GridState(){
            bottomRow = match.gameState.bottomRow,
            middleRow = match.gameState.middleRow,
            topRow = match.gameState.topRow,
            currentTurnId = match.currentTurnId,
            currentTurnPiece = GetCurrentTurnPiece(match)
        });

        /* if (isYourTurn(match))
        {
            gridController.EnablePlacingPiece();
        }
        else
        {
            gridController.DisablePlacingPiece();
        } */
        if (!string.IsNullOrEmpty(match.winner) || match.draw == true)
        {
            gridController.DisablePlacingPiece();
        }
        else if (isYourTurn(match))
        {
            gridController.EnablePlacingPiece();
        }
        else
        {
            gridController.DisablePlacingPiece();
        }
    }

    private void ErrorStartingGame(string error)
    {
        Debug.Log(error);
    }

    private void HandlePiecePlaced()
    {
        // State change happens when piece is placed;
        gridController.DisablePlacingPiece();
        turnController.SetOpponentTurn();

        string apiEndpoint = apiEndpointHost + setGameStateEndpointFormat;
        string apiCall = string.Format(apiEndpoint, LoggedInUser.Instance.currentGameId);

        string payload = gridController.GetStateSerialized();

        StartCoroutine(MakeApiPostCall(apiCall, payload, SendGameStateSuccess, SendGameStateFail));
    }

    private void SendGameStateSuccess(string message)
    {
        Debug.Log("Success: " + message);
    }

    private void SendGameStateFail(string message)
    {
        Debug.Log("Fail:" + message);
    }

    private string GetCurrentTurnPiece(MatchPayload payload)
    {
        string piece = "";

        if (payload.currentTurnId == payload.oOwner)
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
        return payload.currentTurnId == LoggedInUser.Instance.GetUserUID();
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

    private IEnumerator MakeApiPostCall(string apiTarget, string payload, Action<string> onSuccess, Action<string> onFail)
    {
        byte[] payloadAsBytes = System.Text.Encoding.UTF8.GetBytes(payload);
        
        using (UnityWebRequest www = UnityWebRequest.Put(apiTarget, payloadAsBytes))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                onFail(www.error);
            }
            else
            {
                onSuccess(www.downloadHandler.text);
            }
        }
    }

    private IEnumerator MakeApiCall(string apiEndpoint, Action<string> onSuccess, Action<string> onFail)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiEndpoint))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                onFail(www.error);
            }
            else
            {
                onSuccess(www.downloadHandler.text);
            }
        }
    }
}

[System.Serializable]
public class MatchPayload
{
    public string currentTurnId;
    public string xOwner;
    public string oOwner;
    public string winner;
    public bool draw;
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