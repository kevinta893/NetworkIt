# NetworkIt Java

Java implementation of NetworkIt. Compiles for Java 7 or above. Simply import the Jar and use. Jar also includes a Demo Poke Tester much like the one in WPF. This is useful for non Windows users.

Also compatiable with Processing 3+.

## How to use
Simply import the precompiled Jar into your project (e.g. Eclipse) and import the networkit package and all associated JAR files provided in the release. See **demo/CMDMain.java** for an example on how to use. You may also run the jar through the commandline to get the Poke Demo debug tool:

```bash
java -jar NetworkItJava.jar
```

### Connecting
Create a Client object and configure it with the appropriate networking information. Setup the appropriate listeners for network events.

```Java
import networkit.*;
import networkit.Client.EventListener;

Client client = new Client("demo_test_username", "http://localhost", 8000);
client.addConnectListener(new EventListener() {
    @Override
    public void call(Object sender, Object... args) {
        //TODO your code here
    }
});

client.addMessageListener(new EventListener() {
    @Override
    public void call(Object sender, Object... args) {
        //TODO your code here
        Message object = (Message) args[0];
    }
});

client.addDisconnectListener(new EventListener() {
    @Override
    public void call(Object sender, Object... args) {
        //TODO your code here
    }
});

client.addErrorListener(new EventListener() {
    @Override
    public void call(Object sender, Object... args) {
        //TODO your code here
        Exception exception = (Exception) args[0];
    }
});

client.startConnection();
```

### Recieving a Message
To listen for messages, it is as simple as listening to the Message event on the Client.
```Java
client.addMessageListener(new EventListener() {
    @Override
    public void call(Object sender, Object... args) {
        //TODO your code here
        Message object = (Message) args[0];
    }
});
```

### Sending a Message
To send a message, make a message object and send it.

```Java
//TODO Your code here!
Message m = new Message("Poke");
m.addField("num1", Integer.toString(66));
m.addField("num2", Integer.toString(888));
m.deliverToSelf = true;						//give ourselves a copy
client.sendMessage(m);
```

### Installing on Processing
To use NetworkIt in Processing 3 or higher. Simply drag all the release JAR files onto your sketch. Processing should automatically add a new folder in your sketch called *code*.

Note: If you get an error: `No library found for networkit.Client`, you can ignore it. The library should still work regardless. You should be able to still recieve messages, check with `System.out.println("hello");` under the Message event listener and with the poke test Java tool.

## Making Releases
Note for compatiablity, please compile for Java 7. This is to ensure the library is also compatiable with Android.

## Libraries Used
* [okhttp-3.9.0](https://github.com/square/okhttp)
* [okio-1.13.0](https://github.com/square/okio)
* [socket.io-1.0.0](https://github.com/socketio/socket.io-client-java)
* [JSON-Java](https://github.com/stleary/JSON-java)
