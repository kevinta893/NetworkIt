using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetworkIt;

public class SendPoke : MonoBehaviour {

    public NetworkItClient networkInterface;
    public bool deliverToSelf = false;

    private int messageCount = 0;

    public void SendPokeMessage()
    {
        Message m = new Message("Poke!");
        m.DeliverToSelf = deliverToSelf;
        m.AddField("num1", 3);
        m.AddField("num2", 4);
        m.AddField("count", messageCount++);
        networkInterface.SendMessage(m);
    }

    public void SetDeliverToSelf(bool b)
    {
        deliverToSelf = b;
    }
}
