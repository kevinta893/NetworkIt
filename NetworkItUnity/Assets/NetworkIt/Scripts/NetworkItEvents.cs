using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetworkIt;

public class NetworkItEvents : MonoBehaviour {

    public string urlAddress = "";
    public int port = 58181;
    public string username = "give me a new name";                                    

    public GameObject[] messageListeners;

    private Client connection;

    void Start() {

        if (urlAddress == null || urlAddress.Length <= 0) { 
            Debug.LogError("URL or IP Address required.");
            return;
        }

        if (username == null || username.Length <= 0) { 
            Debug.LogError("Username required.");
            return;
        }

        connection = new Client(username, urlAddress, port);            //create and establish connection to server
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void NotifyMessageListeners(Message recievedMessage)
    {
        Debug.Log("Message recieved:\n" + recievedMessage);

        foreach (GameObject g in messageListeners)
        {
            g.BroadcastMessage("OnNetworkMessage", recievedMessage);
        }

    }

}
