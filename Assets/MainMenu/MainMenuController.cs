using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MainMenuController : MonoBehaviour
{

    [SerializeField]
    private GameObject YourTurnSection;

    [SerializeField]
    private GameObject TheirTurnSection;

    [SerializeField]
    private NewGameButtonController newGameButtonController;

    private string matchMakeEndpointFormat = "https://us-central1-aws-tic-tac-toe.cloudfunctions.net/matchmake?uuid={0}";

    // Start is called before the first frame update
    void Start()
    {
        // Get all rooms user is associated with
        
        //UnityWebRequest getRoomsRequest = UnityWebRequest.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FindNewGame()
    {
        string apiCall = string.Format(matchMakeEndpointFormat, LoggedInUser.Instance.GetUserUID());

        newGameButtonController.Disable();
        StartCoroutine(MakeAPICall(
            apiCall,
            HandleNewGameSuccess,
            (error) => {Debug.LogError(error);}
        ));
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
