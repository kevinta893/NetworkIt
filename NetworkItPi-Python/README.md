# NetworkIt Python 2.7 (Raspberry Pi)

The python port of the NetworkIt library. This port is compatiable with Raspberry Pi and Python 2.7. You may need an internet connection to install the socket.io Package.


## How to use
There is a template code to use called **networkit_template_pi.py**. To use this template with the physical push button demo, refer to the wiring diagrams included. Otherwise the regular demo opens up a GUI window.

The template code is meant to look like what you would expect from Arduino's interface. You need not to follow this convention when using this library. See **networkit_demo.py** for a object-oriented version.

### Installation
To install the required socket.io library package, use PIP on the package:
```bash
pip install socketIO-client-0.7.2.tar.gz
```

### Connection
First create a new Client object with the appropriate network configuration. Then listen to the events that you wish to by registering to the appropriate listeners. Start the connection when ready.

```Python
network_client = Client("demo_test_username", "localhost", 8000)
network_client.register_listener(Client.EVENT_MESSAGE, on_message)
network_client.register_listener(Client.EVENT_CONNECT, on_connect)
network_client.register_listener(Client.EVENT_DISCONNECT, on_disconnect)
network_client.register_listener(Client.EVENT_ERROR, on_error)
network_client.start_connection()


def on_message(message):
    blinkCount = int(message.get_field("count")) % 4

def on_connect(args):
    print "Client Connected"

def on_disconnect(args):
    print "Client Disconnected"

def on_error(args):
    print "Error!" + str(args)
    
```

### Recieving a message
Simply register a function to the Message event. Note that all fields are always converted to strings.
```Python
network_client.register_listener(Client.EVENT_MESSAGE, on_message)


def on_message(message):
    print "Message recieved, count=" + message.get_field("count")
    blinkCount = int(message.get_field("count")) % 4

```


### Sending a message
Simply create a Message object and send it along through the client.
```Python
message = Message("Poke!")
message.add_field("num1", str(1))
message.add_field("num2", str(4))
count += 1
message.add_field("count", str(count))
message.deliverToSelf = False

network_client.send_message(message)
```

## Limitations
The reason why we use a Node Server v1.7.3 instead of v2.0+ is because the socket.io client is not compatiable with socket.io 2.0's new transport protocol. This is currently being worked on by the developers of the library. 

There is a beta package of the socket.io client for python that works with socket.io v2.0+ which is included as socket.io-client-nexus. The main issue is that the connect event does not activate on actual connection. To avoid bugs we decided to stick with 1.7.3 for now. See the development fork for socket.io-client for python here: [nexus-devs/socketIO-client-2.0.3]https://github.com/nexus-devs/socketIO-client-2.0.3


## Libraries Used
* [invisibleroads/socketIO-client](https://github.com/invisibleroads/socketIO-client)
