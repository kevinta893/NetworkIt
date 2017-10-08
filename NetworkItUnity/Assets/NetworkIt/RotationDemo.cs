﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkIt;
using System;

public class RotationDemo : MonoBehaviour
{

    private MeshRenderer mesh;

    void Start()
    {
        mesh = this.GetComponent<MeshRenderer>();
        mesh.material.color = new Color(1.0f, 0.0f, 0.0f);
    }

    void Update()
    {

    }


    public void NetworkIt_Message(object m)
    {

        Message message = (Message) m;

        int count = 0;
        int.TryParse(message.GetField("count"), out count);

        this.transform.rotation = Quaternion.Euler(0, count * 10, 0);
    }

    public void NetworkIt_Connect(object args)
    {
        EventArgs eventArgs = (EventArgs) args;
        mesh.material.color = new Color(0.0f, 1.0f, 0.0f);
    }

    public void NetworkIt_Disconnect()
    {
        mesh.material.color = new Color(1.0f, 0.0f, 0.0f);
    }

    public void NetworkIt_Error(object err)
    {
        System.IO.ErrorEventArgs error = (System.IO.ErrorEventArgs) err;
        Debug.LogError(error);
    }
}
