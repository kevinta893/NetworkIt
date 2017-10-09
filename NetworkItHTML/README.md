# NetworkIt HTML-Javascript

The HTML-Javascript implementation of NetworkIt. Useful for utilizing web APIs such as Facebook, Twitter, and other internet APIs.

## How to use
Download the entire NetworkItHTML folder which includes a basic HTML index template, CSS file, and javascripts. Refer to **js/main.js** to insert your code.

### Installation
You need not to host the HTML on XAMPP or simular hosting software unless you need the other services such as MySQL, PHP, and etc.

Simply include the scripts in your **index.html** page:
```HTML
<script src="js/socket.io.js"></script>  <!-- socket.io 2.0.3 -->
<script src="js/networkit.js"></script>
<script src="js/main.js"></script>		 <!-- Your code here -->
```

Other useful libraries you may want to include are:
* jQuery/jQuery Mobile

### Connection
To create a connection, create a new Client object and configure the appropriate network settings. Then create callback functions
```Javascript
var client = new Client("demo_test_username", 'http://localhost', 8000);
client.addMessageListener(function(msg)
{
	//TODO Your code here!
	console.log(JSON.stringify(msg));
});

client.addConnectListener(function(args)
{
	//TODO Your code here!
	console.log('Client Connected.');
});

client.addDisconnectListener(function(args)
{
	//TODO Your code here!
	console.log('Client Disconnected.');
});

client.addErrorListener(function(err)
{
	//TODO Your code here!
	console.log('Error!' + err);
});

```

### Recieving a message
Simply add a message callback to your client

```Javascript
var client = new Client("demo_test_username", 'http://localhost', 8000);


client.addMessageListener(function(msg)
{
	//TODO Your code here!
	console.log(JSON.stringify(msg));
});
```

### Sending a message
Simply create a Message object and add the appropriate fields. Note that each field (key and value) are automatically converted into strings.
```Javascript
var message = new Message("hello");
message.addField('num1', 55);
message.addField('num2', 450);
message.deliverToSelf = true;
client.sendMessage(message);
```

## Limitations and Issues
* You will may often run into cross-origin errors using HTML platforms. Modern browsers will often block connection requests to anything outside the ip-address or domain name. Thus it may be helpful to delve into hosting your own node proxy server. See **NodeServer** for more information on how to host your own private instance of the proxy server.


## Libraries used
* [socket.io-client 2.0.3](https://github.com/socketio/socket.io-client)
