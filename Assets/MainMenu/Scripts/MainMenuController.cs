﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject Content;

    [SerializeField]
    private GameObject YourTurnSection;

    [SerializeField]
    private GameObject TheirTurnSection;

    [SerializeField]
    private NewGameButtonController newGameButtonController;

    private List<RoomData> roomsPlayerIsCurrentlyIn = new List<RoomData>();

    private string apiEndpointHost = "https://us-central1-aws-tic-tac-toe.cloudfunctions.net";
    private string matchMakeEndpointFormat = "/matchmake?uuid={0}";
    private string getRoomsEndpointFormat = "/getrooms?uuid={0}";

    // Start is called before the first frame update
    void Start()
    {
        // Todo: Get rooms from firebase message
        GetRooms();
    }

    public void FindNewGame()
    {
        string apiEndpoint = apiEndpointHost + matchMakeEndpointFormat;
        string apiCall = string.Format(apiEndpoint, LoggedInUser.Instance.GetUserUID());

        newGameButtonController.Disable();
        StartCoroutine(MakeAPICall(
            apiCall,
            HandleNewGameSuccess,
            (error) => {Debug.LogError(error);}
        ));
    }

    private void GetRooms()
    {
        string apiEndpoint = apiEndpointHost + getRoomsEndpointFormat;
        string apiCall = string.Format(apiEndpoint, LoggedInUser.Instance.GetUserUID());

        StartCoroutine(MakeAPICall(
            apiCall,
            HandleGetRoomsSuccess,
            (error) => {Debug.LogError(error);}
        ));
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

        // Dirty hack to properly display buttons in vertical layout group
        Content.SetActive(false);
        Content.SetActive(true);
    }

    private void HandleNewGameSuccess(string response)
    {
        Debug.Log(response);
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

}
