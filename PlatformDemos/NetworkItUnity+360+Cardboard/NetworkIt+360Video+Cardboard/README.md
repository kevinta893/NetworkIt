# Platform Demo: NetworkIt+360Video+Cardboard

A template project that has NetworkIt combined with 360 Video and Google Cardboard. Requires Unity 2017 or higher.

## How to Use
Some setup is required when you import this project. 

To change the 360 video, see notes from: [kevinta893/Unity360Video](https://github.com/kevinta893/Unity360Video)

### Project setup
To run all three pieces together, you will need to setup your *Player Settings* (PC, Android, iOS):
* All platforms:
  * Other Settings > Api Compatibility Level = **.NET 2.0**
  * XR Settings > Virtual Reality Supported = **true**, and add **Cardboard** to the list

* Android:
  * Other Settings > Minimum API Level = **Android 4.4 (API level 19)**
  * Resolution and Presentation > Multithreaded Rendering = **false** (If you are experiencing 360 video stuttering, turn off)
  
### Prefabs
Use the following Prefabs to add 360+Cardboard functionality to your projects
* **Cardboard360Camera** - A prefab that adds a 360 Video player and GVR reticle. You can change your video here.
* **GvrTriggerObject** - A prefab that has an EventTrigger component for the GVR reticle. Use this to add GVR Clickable objects.

### NetworkItUnity
The library for NetworkItUnity is the same. Refer to the [NetworkItUnity](https://github.com/kevinta893/NetworkIt/tree/master/NetworkItUnity) folder for more information on how to use.

## Libraries Used
* [googlevr/gvr-unity-sdk (v1.100.1)](https://github.com/googlevr/gvr-unity-sdk)
* [Unity-Technologies/SkyboxPanoramicShader](https://github.com/Unity-Technologies/SkyboxPanoramicShader)
* [kevinta893/Unity360Video](https://github.com/kevinta893/Unity360Video)
