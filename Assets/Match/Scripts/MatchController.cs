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
    private string setGameStateEndpointFormat = "/setgamestate?gameuuid={0}";

    // Start is called before the first frame update
    void Start()
    {
        loadingController.Show();

        // Start api call to get match info
        StartCoroutine(MakeApiCall("", OnSuccess, OnFail));

        gridController.OnStateUpdate += HandlePiecePlaced; // State change happens when piece is placed;
    }

    void OnDestroy()
    {
        gridController.OnStateUpdate -= HandlePiecePlaced;
    }

    private void HandlePiecePlaced()
    {
        // State change happens when piece is placed;
        gridController.DisablePlacingPiece();
        turnController.SetOpponentTurn();

        string apiEndpoint = apiEndpointHost + setGameStateEndpointFormat;
        string apiCall = string.Format(apiEndpoint, "2GznDkSGIj626vq4hKvJ");

        string payload = gridController.GetStateSerialized();

        Debug.Log(gridController.GetStateSerialized());

        Debug.Log("TODO: Send API Request");
        StartCoroutine(MakeApiPostCall(apiCall, payload, (msg) => {
            Debug.Log("Success: " + msg);
        }, (msg) => {
            Debug.Log("Too bad: " + msg);
        }));
    }

    private void OnSuccess(string payload)
    {
        loadingController.Hide();
        MatchPayload matchPayload = JsonUtility.FromJson<MatchPayload>(payload);

        turnController.SetState(new TurnState(){
            CurrentTurn = matchPayload.currentTurn,
            CurrentTurnPiece = (matchPayload.currentTurn == matchPayload.xOwner) ? "X" : "O"
        });

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
        yield return new WaitForSeconds(1);
        onSuccess.Invoke(GetMockMatchPayload());

        /* Debug.Log("Making api call to " + apiEndpoint);
        UnityWebRequest apiRequest = UnityWebRequest.Get(apiEndpoint); */
        
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