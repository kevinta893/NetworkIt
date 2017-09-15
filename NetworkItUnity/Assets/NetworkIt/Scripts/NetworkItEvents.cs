using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetworkIt;
using System;

public class NetworkItEvents : MonoBehaviour {

    public string urlAddress = "";
    public int port = 8000;
    public string username = "demo_test_username";                                    

    public GameObject[] messageListeners;
    private volatile LinkedList<Message> messageEvents = new LinkedList<Message>();


    private Client client;

    void Start() {

        if (urlAddress == null || urlAddress.Length <= 0) { 
            Debug.LogError("URL or IP Address required.");
            return;
        }

        if (username == null || username.Length <= 0) { 
            Debug.LogError("Username required.");
            return;
        }

        client = new Client(username, urlAddress, port);            //create and establish connection to server
        client.StartConnection();
        client.Connected += Connection_Connected;
        client.Disconnected += Connection_Disconnected;
        client.MessageReceived += Connection_MessageReceived;
        client.Error += Connection_Error;
	}

    void Update()
    {
        ConsumeNetworkMessages();
    }


    //consume messages as they come in
    private void ConsumeNetworkMessages()
    {
        if (messageEvents.Count <= 0)
        {
            return;
        }

        while (messageEvents.Count > 0)
        {
            Message m = messageEvents.First.Value;

            foreach (GameObject g in messageListeners)
            {
                g.SendMessage("MessageReceived", m);
            }

            messageEvents.RemoveFirst();
        }

    }
    private void Connection_MessageReceived(object sender, NetworkItMessageEventArgs e)
    {
        NotifyMessageListeners(e.ReceivedMessage);
    }

    private void Connection_Error(object sender, System.IO.ErrorEventArgs e)
    {

    }

    private void Connection_Disconnected(object sender, System.EventArgs e)
    {

    }

    private void Connection_Connected(object sender, System.EventArgs e)
    {

    }


    //consumer producer pattern for threads
    private void NotifyMessageListeners(Message recievedMessage)
    {
        messageEvents.AddLast(recievedMessage);

    }

    private void OnApplicationQuit()
    {
        client.CloseConnection();
    }

}
