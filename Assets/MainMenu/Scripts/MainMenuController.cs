using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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

    private List<RoomData> roomsPlayerIsCurrentlyIn;
    private List<GameTurnsData> gamesPlayerIsCurrentlyIn = new List<GameTurnsData>();
    private List<GameObject> findingPlayerButtons = new List<GameObject>();
    private List<GameObject> gameButtons = new List<GameObject>();
    private RoomsDataContainer[] roomsDataContainer;
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
        //GetRooms();
        //GetPlayerMenu();
        StartCoroutine(GetPlayerMenuRoutine());
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
                //GetRooms();
                //GetPlayerMenu();
                StartCoroutine(GetPlayerMenuRoutine());
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
        string playerMenuApiEndpoint = apiEndpointHost + getPlayerMenuDataEndpointFormat;
        string playerMenuApiCall = string.Format(playerMenuApiEndpoint, LoggedInUser.Instance.GetUserUID());



        /* string apiEndpoint = apiEndpointHost + getPlayerMenuDataEndpointFormat;
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
        )); */
    }

    private IEnumerator GetPlayerMenuRoutine()
    {
        isMakingRequest = true;

        string playerMenuApiEndpoint = apiEndpointHost + getPlayerMenuDataEndpointFormat;
        string playerMenuApiCall = string.Format(playerMenuApiEndpoint, LoggedInUser.Instance.GetUserUID());

        string roomsApiEndpoint = apiEndpointHost + getRoomsEndpointFormat;
        string roomsApiCall = string.Format(roomsApiEndpoint, LoggedInUser.Instance.GetUserUID());

        bool roomsReady = false;
        bool gamesReady = false;

        StartCoroutine(MakeAPICall(
            roomsApiCall,
            (response) => {HandleGetRoomsSuccess(response);roomsReady = true;},
            (error) => {}
        ));

        StartCoroutine(MakeAPICall(
            playerMenuApiCall,
            (response) => {HandleGetPlayerInfoSuccess(response); gamesReady = true;},
            (error) => {}
        ));

        while(!roomsReady || !gamesReady)
        {
            yield return null;
        }

        isMakingRequest = false;
        Debug.Log("GetPlayerMenu complete!");
    }

    private void GetRooms()
    {
        string apiEndpoint = apiEndpointHost + getRoomsEndpointFormat;
        string apiCall = string.Format(apiEndpoint, LoggedInUser.Instance.GetUserUID());

        //RemoveAllFindingPlayers();

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
        RoomsDataContainer[] roomsPayload = JsonHelper.DeserializeFromServer<RoomsDataContainer>(response);
        
        if (roomsDataContainer == null)
        {
            roomsDataContainer = roomsPayload;
            UpdateRoomsView();
        }
        else if (roomsDataContainer.Length != roomsPayload.Length)
        {
            Debug.Log("Rooms payload not same size as cached rooms. Update cached rooms");
            // Update
            roomsDataContainer = roomsPayload;
            // Update view
            UpdateRoomsView();

        }
    }

    private void HandleGetPlayerInfoSuccess(string response)
    {
        PlayerMenuDataContainer playerInfo = JsonUtility.FromJson<PlayerMenuDataContainer>(response);

        if (currentPlayerMenuData == null)
        {
            currentPlayerMenuData = playerInfo;
            UpdateGamesView();
        }
        else if (!currentPlayerMenuData.Equals(playerInfo))
        {
            Debug.Log("Games payload not same as cached games. Update cached games");
            currentPlayerMenuData = playerInfo;
            UpdateGamesView();
        }

        /* GameObject gameButtonPrefab = Resources.Load("GameButton") as GameObject;
        
        // Create api that gives you gameid and whose turn
        // will make this a lot easier
        foreach(string gameid in playerInfo.games)
        {
            // Get current players turn to decide where to place button
            string apiEndpoint = apiEndpointHost + getGameInfoEndpointFormat;
            string apiCall = string.Format(apiEndpoint, gameid);

            //GameObject gameButton = Instantiate(gameButtonPrefab);
            //Vector3 localScale = gameButton.transform.localScale;
        } */
    }

    private void UpdateRoomsView()
    {
        if (roomsDataContainer != null)
        {
            GameObject findingPlayerPrefab = Resources.Load("FindingPlayerButton") as GameObject;
            RemoveAllFindingPlayers();
            foreach (RoomsDataContainer roomData in roomsDataContainer)
            {
                GameObject findingPlayerButton = Instantiate(findingPlayerPrefab);
                Vector3 localScale = findingPlayerButton.transform.localScale;
                findingPlayerButton.transform.SetParent(YourTurnSection.transform);
                findingPlayerButton.transform.localScale = localScale;

                findingPlayerButtons.Add(findingPlayerButton);
            }

            RefreshContent();
        }
    }

    private void UpdateGamesView()
    {
        if (currentPlayerMenuData != null)
        {
            GameObject gameButtonPrefab = Resources.Load("GameButton") as GameObject;
            RemoveAllGames();
            foreach (PlayerMenuGamesDataContainer game in currentPlayerMenuData.games)
            {
                GameObject gameButton = Instantiate(gameButtonPrefab);
                GameButtonController controller = gameButton.GetComponent<GameButtonController>();
                controller.OnClick += (gameid) => {
                    LoggedInUser.Instance.currentGameId = gameid;
                    SceneManager.LoadScene("Match/Match");
                };
                controller.GameId = game.gameid;
                Vector3 localScale = gameButton.transform.localScale;

                if (game.turn == LoggedInUser.Instance.GetUserUID())
                {
                    gameButton.transform.SetParent(YourTurnSection.transform);
                }
                else
                {
                    gameButton.transform.SetParent(TheirTurnSection.transform);
                }

                gameButton.transform.localScale = localScale;

                gameButtons.Add(gameButton);
            }

            RefreshContent();
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
        foreach(GameObject roomButton in findingPlayerButtons)
        {
            Destroy(roomButton);
        }
        findingPlayerButtons.Clear();

        RefreshContent();
        /* foreach(RoomData roomData in roomsPlayerIsCurrentlyIn)
        {
            Destroy(roomData.gameObject);
        }
        roomsPlayerIsCurrentlyIn.Clear();

        RefreshContent(); */
    }

    private void RemoveAllGames()
    {
        foreach (GameObject gameButton in gameButtons)
        {
            Destroy(gameButton);
        }
        gameButtons.Clear();

        RefreshContent();

        /* foreach(GameTurnsData turnData in gamesPlayerIsCurrentlyIn)
        {
            Destroy(turnData.gameObject);
        }
        gamesPlayerIsCurrentlyIn.Clear();

        RefreshContent(); */
    }

    private void RefreshContent()
    {
        // Dirty hack to properly display buttons in vertical layout group
        Content.SetActive(false);
        Content.SetActive(true);
    }

}
