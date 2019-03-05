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

    // Start is called before the first frame update
    void Start()
    {
        // Get all rooms user is associated with
        string getRoomsEndpoint = string.Format("https://us-central1-aws-tic-tac-toe.cloudfunctions.net/matchmake?uuid={0}", "");
        //UnityWebRequest getRoomsRequest = UnityWebRequest.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
