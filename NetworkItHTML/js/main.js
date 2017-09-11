//setup connection
var client = new Client("demo_test_username", 'http://localhost', 8000);
client.addMessageListener(function(msg)
{
	//message recieved
	console.log(JSON.stringify(msg));
});

client.addConnectListener(function(msg)
{
	//message recieved
	console.log('Client Connected.');
	document.getElementById("status").innerHTML = 'Connected to Server.';
	document.getElementById("status_img").src = 'images/green_circle.png';
});

client.addDisconnectListener(function(msg)
{
	//message recieved
	console.log('Client Disconnected.');
	document.getElementById("status").innerHTML = 'Disconnected.';
	document.getElementById("status_img").src = 'images/red_circle.png';
});

client.addErrorListener(function(err)
{
	//message recieved
	console.log('Error!' + err);
});




//Your code here!

//send a message demo
var message = new Message("hello");
message.addField('num1', 55);
message.addField('num2', 450);
message.deliverToSelf = true;
client.sendMessage(message);
