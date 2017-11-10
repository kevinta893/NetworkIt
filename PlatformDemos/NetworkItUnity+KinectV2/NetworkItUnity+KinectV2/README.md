# Platform Demo: NetworkIt+KinectV2

A template project that has NetworkIt combined with Microsoft Kinect V2. Requires Unity 2017 or higher.
This special version of NetworkIt has modifications to the compiler options to avoid name collisions with the libraries found in Kinect V2. Thus requires the use of a *mcs.rsp* file to allow the use of ```extern alias```. 

## How to Use
See the Kinect+NetworkIt scene on how to use. KinectManagers manage information being passed by the Windows DLLs and Unity. KinectView contains gameobjects that translate information by the KinectManagers into graphical elements.

### Body Tracking
Body tracking information can be fetched from BodySourceView. Register your game object for events concerning the detection of a  Body detected by Kinect. The script emits events for when a new body is detected or destroyed. Since it uses Unity's SendMessage() functionality, implement the following signatures in your script.

Bodies are only instantiated when they are detected. Listen to the following events when a body is found/lost:
```C#

void Kinect_BodyFound(object args)
{
	BodyGameObject bodyFound = (BodyGameObject) args;
}

void Kinect_BodyLost(object args)
{
	ulong bodyDeletedId = (ulong) args;
}
```

Joints on the body only have their positions updated in real time. To get positions of the body you will need to use the transform.localPosition properties of each joint. You can get the positions of the bodies as follows:

```C#
BodyGameObject bodyFound = (BodyGameObject) args;
GameObject thumbRight = bodyFound.GetJoint(Windows.Kinect.JointType.ThumbRight);

Vector3 thumbPosition = thumbRight.transform.localPosition;
```

### Troubleshooting

Some potential errors:
* Visual Studio - *The extern alias ... was not specified in a /reference option*. You may have to go to your project in Visual Studio, Find **References > SocketIoClientDotNet** and **References > WebSocket4Net** and set the Aliases properties to **SocketIoClientDotNet** and **WebSocket4Net** respectively. 
* Unity Editor - *The extern alias ... was not found in a /reference option*. You may need to reload your project. Right click your folder and go **Reimport All**.



### NetworkItUnity
The main functionality and features of NetworkItUnity has not changed. Refer to the [NetworkItUnity](https://github.com/kevinta893/NetworkIt/tree/master/NetworkItUnity) folder for more information on how to use.

## Libraries Used
* Microsoft's KinectForWindows_UnityPro_2.0.1410
* [kevinta893/Unity360Video](https://github.com/kevinta893/Unity360Video)
