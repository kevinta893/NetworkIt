using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkIt;

public class BodyNetworkTemplate : MonoBehaviour {

    public NetworkItClient networkItClient;

    private List<BodyGameObject> bodies = new List<BodyGameObject>();
    private MeshRenderer mesh;

	void Start () {
        mesh = GetComponent<MeshRenderer>();
        mesh.material.color = new Color(1.0f, 0.0f, 0.0f);

    }


    //wait for KinectManager to completely update first
    void LateUpdate () {
        //TODO Your code here
        if (bodies.Count > 0)
        {
            //some bodies, send orientation update
            GameObject thumbRight = bodies[0].GetJoint(Windows.Kinect.JointType.ThumbRight);
            GameObject handRight = bodies[0].GetJoint(Windows.Kinect.JointType.HandRight);
            GameObject handTipRight = bodies[0].GetJoint(Windows.Kinect.JointType.HandTipRight);

            float wristRotation = KinectCVUtilities.VerticalWristRotation(
                handTipRight.transform.localPosition,
                handRight.transform.localPosition,
                thumbRight.transform.localPosition
                );

            //send the rotation
            Message wristRotationMessage = new Message("WristRotation");
            wristRotationMessage.AddField("angle", "" + wristRotation);
            wristRotationMessage.DeliverToSelf = true;
            networkItClient.SendMessage(wristRotationMessage);

        }
    }




    void Kinect_BodyFound(object args)
    {
        BodyGameObject bodyFound = (BodyGameObject) args;
        bodies.Add(bodyFound);
    }

    void Kinect_BodyLost(object args)
    {
        ulong bodyDeletedId = (ulong) args;

        lock (bodies){
            foreach (BodyGameObject bg in bodies)
            {
                if (bg.ID == bodyDeletedId)
                {
                    bodies.Remove(bg);
                    return;
                }
            }
        }
    }

    //===================================
    //network messages

    public void NetworkIt_Message(object m)
    {
        //TODO your code here
        Message message = (Message)m;

        float wristRotation = 0;
        float.TryParse(message.GetField("angle"), out wristRotation);

        this.transform.rotation = Quaternion.Euler(0, wristRotation, 0);

    }

    public void NetworkIt_Connect(object args)
    {
        //TODO your code here
        EventArgs eventArgs = (EventArgs)args;
        mesh.material.color = new Color(0.0f, 1.0f, 0.0f);
    }

    public void NetworkIt_Disconnect(object args)
    {
        //TODO your code here
        EventArgs eventArgs = (EventArgs)args;
        mesh.material.color = new Color(1.0f, 0.0f, 0.0f);
    }

    public void NetworkIt_Error(object err)
    {
        //TODO your code here
        ErrorEventArgs error = (ErrorEventArgs)err;
        Debug.LogError(error);
    }
}
