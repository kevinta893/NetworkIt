using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetworkIt;

public class NetworkItEvents : MonoBehaviour {

    public string urlAddress = "";
    public int port = 8000;
    public string username = "demo_test_username";                                    

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
        connection.StartConnection();
        connection.MessageReceived += Connection_MessageReceived;
        connection.Connected += Connection_Connected;
	}

    private void Connection_Connected(object sender, System.EventArgs e)
    {
        Message m = new Message("hyppppppe");
        m.AddField<string>("nameness", "null");
        connection.SendMessage(m);
    }

    private void Connection_MessageReceived(object sender, NetworkItMessageEventArgs e)
    {
        Debug.Log("Message Recieved");
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

    private void OnApplicationQuit()
    {
        connection.CloseConnection();
    }

}
