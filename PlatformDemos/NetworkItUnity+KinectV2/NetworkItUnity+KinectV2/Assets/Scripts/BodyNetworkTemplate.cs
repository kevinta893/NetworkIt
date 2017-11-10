using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkIt;

public class BodyNetworkTemplate : MonoBehaviour {

    private List<BodyGameObject> bodies = new List<BodyGameObject>();

    private MeshRenderer mesh;
	// Use this for initialization
	void Start () {
        mesh = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        //TODO Your code here
        if (bodies.Count > 0)
        {
            //some bodies, send orientation update
            GameObject thumbRight = bodies[0].GetJoint(Windows.Kinect.JointType.ThumbRight);
            GameObject handRight = bodies[0].GetJoint(Windows.Kinect.JointType.HandRight);
            GameObject handTipRight = bodies[0].GetJoint(Windows.Kinect.JointType.HandTipRight);

            float angle = VerticalWristRotation(
                thumbRight.transform.position,
                handRight.transform.position,
                handTipRight.transform.position
                );
            Debug.Log(angle);
            this.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    /// <summary>
    /// Gets an angle between 3 points that form a connection:  p1---p2---p3
    /// Such that the vector v = p3-p2 defines the angle around p1
    /// </summary>
    /// <param name="p1">A position in space</param>
    /// <param name="p2">A position in space</param>
    /// <param name="p3">A position in space</param>
    /// <returns></returns>
    float VerticalWristRotation(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = p2 - p3;
        Vector3 b = p2 - p1;
        a.Normalize();
        b.Normalize();
        Vector3 thetaVector = Vector3.ProjectOnPlane(b, a);
        thetaVector.Normalize();

        return Vector3.SignedAngle(Vector3.right, thetaVector, Vector3.up);
    }


    void Kinect_BodyFound(object args)
    {
        BodyGameObject bodyFound = (BodyGameObject) args;
        bodies.Add(bodyFound);
    }

    void Kinect_BodyDeleted(object args)
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



    public void NetworkIt_Message(object m)
    {
        //TODO your code here
        Message message = (Message)m;

        int count = 0;
        int.TryParse(message.GetField("count"), out count);

        this.transform.rotation = Quaternion.Euler(0, count * 10, 0);
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
