# Platform Demo: NetworkIt+KinectV2+OpenCV3

A template project that has NetworkIt combined with Microsoft Kinect V2 and OpenCV 3 (using OpenCVSharp). Requires Unity 2017 or higher.
This special version of NetworkIt has modifications to the compiler options to avoid name collisions with the libraries found in Kinect V2. Thus requires the use of a *mcs.rsp* file to allow the use of ```extern alias```. 

## How to Use
See the Assets/Scene/Kinect+OpenCV+NetworkIt scene on how to use. The KinectManager manages information being passed by the Windows DLLs and Unity. KinectView contains gameobjects that translate information by the KinectManagers into graphical elements.

### Body Tracking
Body tracking information can be fetched from the **BodyView** script which is usually attached to the KinectManager. Register your game object for events concerning the detection of a  Body detected by Kinect. The script emits events for when a new body is detected or destroyed. Since it uses Unity's SendMessage() functionality, implement the *both* of the following signatures in your script:

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

### OpenCV 3
Using OpenCVSharp, it is possible to work with the Kinect's camera streams using OpenCV 3. Note that you will want to use **LateUpdate()** function instead of Update() to allow the Kinect to fully collect and create the textures before debugging. 

See also the **KinectCVUtilities** script for a bunch of useful functions for working with OpenCV and Kinect in Unity. 

Included OpenCV examples are:
* Blob tracking in Infrared stream, use a retroreflective material (e.g. mocap) to have that be tracked by the Kinect
* Face Detection in Color stream, slower than what the Kinect provides, but may be of use

Enable the game objects for these demos as necessary or remove them entirely to use the scenes as a template.

### NetworkItUnity
The main functionality and features of NetworkItUnity has not changed. Refer to the [NetworkItUnity](https://github.com/kevinta893/NetworkIt/tree/master/NetworkItUnity) folder for more information on how to use.


### Troubleshooting

Some potential errors:
* Visual Studio - *The extern alias ... was not specified in a /reference option*. You may have to go to your project in Visual Studio, Find **References > SocketIoClientDotNet** and **References > WebSocket4Net** and set the Aliases properties to **SocketIoClientDotNet** and **WebSocket4Net** respectively. 
* Unity Editor - *The extern alias ... was not found in a /reference option*. You may need to reload your project. Right click your folder and go **Reimport All**.





## Libraries Used
* Microsoft's KinectForWindows_UnityPro_2.0.1410
* [shimat/opencvsharp](https://github.com/shimat/opencvsharp)
* Demo code derived from: [VahidN/OpenCVSharp-Samples](https://github.com/VahidN/OpenCVSharp-Samples)
