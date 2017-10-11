# NetworkIt CSharp

The definitive version of NetworkIt. All changes to the protocol and features should start here in WPF .NET. 

This project also includes a debugging tool called the NetworkItPokeDemo. It is a tool that listens in on network messages from a particular username. Useful for debugging your own protocol bugs.

### How to use

You will need to import the DLLs into your project. Do this by right clicking right-clicking on your project in Visual Studio and go to *Add > Reference*. Recommended to create a folder inside your project to store the DLLs to ensure your project can find them on other computers.


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
m.AddField("num1", "" + 3);
m.AddField("num2", "" + 4);
m.AddField("count", "" + messageCount++);

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

##Libraries Used
With a bit of magic, these libraries have been modified to work with WPF:
* EngineIoClinetDotNet
* Newtonsoft.JSON
* SocketIoClientDotNet
* WebSocket4Net
