# NetworkIt
A static IP, socket.io based proxy server and client to allow different platforms to communciate to each other. Designed for CPSC 581 students at the University of Calgary, the kit allows students to quickly combine different plaforms and devices together for prototyping.

Originally developed by: David Ledo & Brennan Jones

## For Students
Download the appropriate releases for your project and import. There are instructions on how to import in each platform release. There is also a demo program in each platform that shows how to use the API on the platform

Current supported platforms:
* Windows WPF 
* Java 7+
* ~~Windows 8 & 8.1 Phones~~
* Unity (Windows, Mac, Linux), (Android)
* Python 2.7 (Raspberry Pi Compatiable)
* Arduino (with node.js v7.5.0)


### Using the Library

The code below is to demonstrate how most of the library has been written. It is meant to make it easy to connect and start developing a protocol as quick as possible. The code pattern is generally the same for all platforms support Socket.io but on an even higher level abstraction that requires little understanding of how Socket.io works.


#### 1. Connecting
First create a Client object by specifying a "secret" username, server URL, and port number (default=8000). Then setup the appropriate events that you want to watch for. Often you will only need to use the Connect and Message events. 


```C#
Client client = new Client("username", "http://localhost", 8000);
client.MessageReceived += Client_MessageReceived;
client.Connected += Client_Connected;
client.StartConnection();


private void Client_Connected(object sender, EventArgs e)
{
    WriteLogLine("Connection Successful");
}

private void Client_MessageReceived(object sender, NetworkItMessageEventArgs e)
{
    WriteLogLine(e.ReceivedMessage.ToString());
}

```



#### 2. Sending a Message
To send a message through the API, create a Message object, and some fields, and send it along via your client object. You can optionally set the DeliverToSelf option which sends you a copy of your own message.

```C#
Message m = new Message("Poke!");
m.DeliverToSelf = false;
m.AddField("num1", 3);
m.AddField("num2", 4);
m.AddField("count", messageCount++);

client.SendMessage(e.ReceivedMessage.ToString());
```

#### 3. Recieving a Message
When you recieve a message, you will get a Message object back. Here you can find your additional fields and parse them back into their original types.

```C#
private void Client_MessageReceived(object sender, NetworkItMessageEventArgs e)
{
    Message m = e.ReceivedMessage;
    if (m.subject.equals("Poke!"))
    {
        int power = 0;
        int.TryParse(m.GetField("count"), out power);
    }
}
```



## Hosting your own instance
If you wish to run your see **Hosting your own instance** below. Basically run the NodeServer on your own machine and use localhost as the URL. This is useful for personal servers for when the instructor's server isn't running anymore.

If you wish to run a server for students, request a static IP and server hosting from your IT department. You may also wish to add a Domain Name so it is easier for students to access the server.

**Installation**: 
Install socket.io (v1.7.3) with commands (or otherwise use the included node_modules): 
```bash
npm install socket.io@1.7.3
npm install dateformat@3.0.2
```
**Run with**
```bash
node server.js
```

## Developers
Refer to the WPF version which defines the protocol. Use the latest version to structure how the library should operate when porting to other platforms. Prioritize building quick connect and easy to use object serialization through JSON.

## Contributors
Credit to the following Githubbers who helped improve/develop the library:
* (your name here)
