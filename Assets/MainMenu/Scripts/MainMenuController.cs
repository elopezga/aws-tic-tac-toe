using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private float GameCheckPollingFrequencyInSeconds = 30f;

    [SerializeField]
    private GameObject Content;

    [SerializeField]
    private GameObject YourTurnSection;

    [SerializeField]
    private GameObject TheirTurnSection;

    [SerializeField]
    private NewGameButtonController newGameButtonController;

    private List<RoomData> roomsPlayerIsCurrentlyIn = new List<RoomData>();
    private List<GameTurnsData> gamesPlayerIsCurrentlyIn = new List<GameTurnsData>();
    private PlayerMenuDataContainer currentPlayerMenuData;
    private float timeSinceLastGameCheckPollInSeconds = 0f;
    private bool isMakingRequest = false;

    private string apiEndpointHost = "https://us-central1-aws-tic-tac-toe.cloudfunctions.net";
    private string matchMakeEndpointFormat = "/matchmake?uuid={0}";
    private string getRoomsEndpointFormat = "/getrooms?uuid={0}";
    private string getPlayerInfoEndpointFormat = "/getPlayerInfo?uuid={0}";
    private string getGameInfoEndpointFormat = "/getgameinfo?game={0}";
    private string getGameTurnsEndpointFormat = "/getturns?uuid={0}";
    private string getPlayerMenuDataEndpointFormat = "/getPlayerMenuInfo?uuid={0}";

    // Start is called before the first frame update
    void Start()
    {
        GetRooms();
        GetPlayerMenu();
    }

    void Update()
    {
        // Pool Games
        timeSinceLastGameCheckPollInSeconds += Time.deltaTime;
        if (timeSinceLastGameCheckPollInSeconds >= GameCheckPollingFrequencyInSeconds)
        {
            if (!isMakingRequest)
            {
                /* GetRooms();
                GetGameTurns(); */
                GetRooms();
                GetPlayerMenu();
                timeSinceLastGameCheckPollInSeconds = 0f;
            }
        }

    }

    public void FindNewGame()
    {
        string apiEndpoint = apiEndpointHost + matchMakeEndpointFormat;
        string apiCall = string.Format(apiEndpoint, LoggedInUser.Instance.GetUserUID());

        isMakingRequest = true;
        newGameButtonController.Disable();
        StartCoroutine(MakeAPICall(
            apiCall,
            HandleNewGameSuccess,
            (error) => {Debug.LogError(error);}
        ));
    }

    private void GetPlayerMenu()
    {
        string apiEndpoint = apiEndpointHost + getPlayerMenuDataEndpointFormat;
        string apiCall = string.Format(apiEndpoint, LoggedInUser.Instance.GetUserUID());

        isMakingRequest = true;
        StartCoroutine(MakeAPICall(
            apiCall,
            (response => {
                PlayerMenuDataContainer playerMenuData = JsonUtility.FromJson<PlayerMenuDataContainer>(response);
                // Update main menu only if the recieved data is diff from stored

                if (currentPlayerMenuData == null)
                {
                    currentPlayerMenuData = playerMenuData;
                }
                else
                {
                    if (!currentPlayerMenuData.Equals(playerMenuData))
                    {
                        currentPlayerMenuData = playerMenuData;
                        // Refresh
                    }
                }
                isMakingRequest = false;
            }),
            (error) => {Debug.LogError(error);}
        ));
    }

    private void GetRooms()
    {
        string apiEndpoint = apiEndpointHost + getRoomsEndpointFormat;
        string apiCall = string.Format(apiEndpoint, LoggedInUser.Instance.GetUserUID());

        RemoveAllFindingPlayers();

        StartCoroutine(MakeAPICall(
            apiCall,
            HandleGetRoomsSuccess,
            (error) => {Debug.LogError(error);}
        ));
    }

    private void GetGameTurns()
    {
        string apiEndpoint = apiEndpointHost + getGameTurnsEndpointFormat;
        string apiCall = string.Format(apiEndpoint, LoggedInUser.Instance.GetUserUID());

        RemoveAllGames();

        StartCoroutine(MakeAPICall(
            apiCall,
            HandleGetGameTurnsSuccess,
            (error) => {Debug.LogError(error);}
        ));
    }

    private void HandleGetGameTurnsSuccess(string response)
    {
        GameObject gameButtonPrefab = Resources.Load("GameButton") as GameObject;

        GameTurnsDataContainer[] gameTurnsData = JsonHelper.DeserializeFromServer<GameTurnsDataContainer>(response);

        foreach(GameTurnsDataContainer turnData in gameTurnsData)
        {
            GameObject gameButton = Instantiate(gameButtonPrefab);
            Vector3 localScale = gameButton.transform.localScale;
            
            if (turnData.turn == LoggedInUser.Instance.GetUserUID())
            {
                gameButton.transform.SetParent(YourTurnSection.transform);
            }
            else
            {
                gameButton.transform.SetParent(TheirTurnSection.transform);
            }

            gameButton.transform.localScale = localScale;

            GameTurnsData gameTurnData = gameButton.GetComponent<GameTurnsData>();
            gameTurnData.SetGameTurnData(turnData);
            gamesPlayerIsCurrentlyIn.Add(gameTurnData);
        }

        RefreshContent();
    }

    private void HandleGetRoomsSuccess(string response)
    {
        List<RoomsDataContainer> rooms = new List<RoomsDataContainer>();

        GameObject findingPlayerPrefab = Resources.Load("FindingPlayerButton") as GameObject;
        
        RoomsDataContainer[] roomsData = JsonHelper.DeserializeFromServer<RoomsDataContainer>(response);
        foreach(RoomsDataContainer room in roomsData)
        {
            GameObject findingPlayerButton = Instantiate(findingPlayerPrefab);
            Vector3 localScale = findingPlayerButton.transform.localScale;
            findingPlayerButton.transform.SetParent(YourTurnSection.transform);
            findingPlayerButton.transform.localScale = localScale;

            RoomData roomData = findingPlayerButton.GetComponent<RoomData>();
            roomData.SetRoomData(room);
            roomsPlayerIsCurrentlyIn.Add(roomData);
        }

        RefreshContent();
    }

    private void HandleGetPlayerInfoSuccess(string response)
    {
        PlayerInfoDataContainer playerInfo = JsonUtility.FromJson<PlayerInfoDataContainer>(response);

        GameObject gameButtonPrefab = Resources.Load("GameButton") as GameObject;
        
        // Create api that gives you gameid and whose turn
        // will make this a lot easier
        foreach(string gameid in playerInfo.games)
        {
            // Get current players turn to decide where to place button
            string apiEndpoint = apiEndpointHost + getGameInfoEndpointFormat;
            string apiCall = string.Format(apiEndpoint, gameid);

            /* GameObject gameButton = Instantiate(gameButtonPrefab);
            Vector3 localScale = gameButton.transform.localScale; */
        }
    }

    private void HandleNewGameSuccess(string response)
    {
        isMakingRequest = false;
        Debug.Log(response);

        // Refresh the rooms
        GetRooms();
        newGameButtonController.Enable();
    }

    private IEnumerator MakeAPICall(string apiCall, Action<string> onSuccess, Action<string> onFail)
    {
        Debug.Log("Making api call to " + apiCall);
        UnityWebRequest apiRequest = UnityWebRequest.Get(apiCall);
        yield return apiRequest.SendWebRequest();

        if (apiRequest.isNetworkError || apiRequest.isHttpError)
        {
            onFail(apiRequest.error);
        }
        else
        {
            onSuccess(apiRequest.downloadHandler.text);
        }
    }

    private void RemoveAllFindingPlayers()
    {
        foreach(RoomData roomData in roomsPlayerIsCurrentlyIn)
        {
            Destroy(roomData.gameObject);
        }
        roomsPlayerIsCurrentlyIn.Clear();

        RefreshContent();
    }

    private void RemoveAllGames()
    {
        foreach(GameTurnsData turnData in gamesPlayerIsCurrentlyIn)
        {
            Destroy(turnData.gameObject);
        }
        gamesPlayerIsCurrentlyIn.Clear();

        RefreshContent();
    }

    private void RefreshContent()
    {
        // Dirty hack to properly display buttons in vertical layout group
        Content.SetActive(false);
        Content.SetActive(true);
    }

}
