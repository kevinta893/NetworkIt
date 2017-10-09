# NetworkIt Unity
The unity port of the Networkit Library. Additional classes have been added in Unity to support the program paradigm there. The supported Unity version for this library is Unity 5.6.3f1.

## How to use
The unity package contains the usual libraries for Socket.io support and the NetworkIt package. The difference is that we have a NetworkItClient class which can be used as a MonoBehaviour.

1. Add an empty game object with the NetworkItClient script
2. Configure the network properties in the inspector
3. For any game object that needs to listen to network events, add them to the list of **Event Listeners** in NetworkItClient
4. Override any of the message signatures in those gameobject's scripts to react to network events

Below are the override method signatures you would need to add to your gameobject's scripts (See RotationDemo.cs for example):
```C#
public void NetworkIt_Message(object m)
{
    Message message = (Message) m;
}

public void NetworkIt_Connect(object args)
{
    EventArgs eventArgs = (EventArgs) args;
}

public void NetworkIt_Disconnect(object args)
{
    EventArgs eventArgs = (EventArgs)args;
}

public void NetworkIt_Error(object err)
{
    ErrorEventArgs error = (ErrorEventArgs) err;
}

```


## Installation
Simply drag the .unitypackage into your project and you're mostly ready to go. You will need to configure your build settings.

You will need to setup the following for Unity 5.6.3f1 in the player settings:

Player settings:
* *Resolution and Presentation > Run In Background* should be set to true
* *File > Build Settings... > Player Settings... > Other Settings > Configuration> API Compatibility Level* should be set to **.NET 2.0**


## Libraries used

* Newtonsoft JSON 10.0.3 (.NET 2.0)
* [socket.io-unity](https://github.com/floatinghotpot/socket.io-unity)
    * WebSocket4Net (precompiled from floating hotspot), see [official repo](https://github.com/kerryjiang/WebSocket4Net)
