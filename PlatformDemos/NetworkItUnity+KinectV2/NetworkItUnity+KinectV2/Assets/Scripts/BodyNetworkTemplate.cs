using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyNetworkTemplate : MonoBehaviour {

    private List<BodyGameObject> bodies = new List<BodyGameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //TODO Your code here
        if (bodies.Count > 0)
        {
            //some bodies, send orientation update
            GameObject thumbRight = bodies[0].GetJoint(Windows.Kinect.JointType.ThumbRight);
            Debug.Log(thumbRight.transform.localPosition*100);

            this.transform.rotation = Quaternion.Euler(thumbRight.transform.localPosition*50);
        }
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
}
