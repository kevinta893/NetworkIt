using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkIt;


public class RotationDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void MessageReceived(object m)
    {
        Message message = (Message) m;
        int count = 0;
        int.TryParse(message.GetField("count"), out count);

        this.transform.rotation = Quaternion.Euler(0, count * 10, 0);
    }
}
