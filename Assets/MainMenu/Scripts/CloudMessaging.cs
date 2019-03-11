using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMessaging : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received registration token: " + token.Token);
    }

    private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs message)
    {
        Debug.Log("Received a new message from: " + message.Message.From);
    }
}
