using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetworkIt;

public class NetworkItUnity : MonoBehaviour {

    public string urlAddress;
    public int port = 8000;
    public string username;

    private Client connection;

	void Start () {

        connection = new Client(username, urlAddress, port);            //create and establish connection to server
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
