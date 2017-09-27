var username = 'demo_test_username';			//defaults
var url = 'http://localhost';
var port = 8000;

var socket = require('socket.io-client');
var serialport = require('serialport');

var arduinoPort;

var serialBuffer = '';

var NETWORK_SEND_HEADER = 'send_networkit_message=';
var NETWORK_SEND_END_HEADER = '=send_end';

//=================================
//Serialport connection to Arduino
serialport.list(function(err, ports)
{
	ports.forEach(function(p)
	{
		if(p["manufacturer"] && p["manufacturer"].includes("Arduino"))
		{
			var port = new serialport(p['comName'], 
			{
				baudRate: 115200, 
			});

			port.on('open', function(data)
			{
				sendMessageArduino(0,2000);
				console.log("Arduino connected.");
			});
			
			port.on('data', function(data)
			{
				serialBuffer += data.toString();
				var splitBuffer = serialBuffer.split(/\r?\n/g);				//arduino outputs \r\n per new line
				serialBuffer = splitBuffer.pop();							//retain incomplete lines by buffer
				
				if (splitBuffer != null && splitBuffer.length > 0)
				{
					for (var i = 0 ; i < splitBuffer.length ; i++)
					{
						var bufferLine = splitBuffer[i];
					
						//if equals the header, this is a send request
						if (bufferLine.indexOf(NETWORK_SEND_HEADER) >= 0)
						{
							//strip out header and send message string
							var messageStripped = bufferLine.substring(
								bufferLine.indexOf(NETWORK_SEND_HEADER) + NETWORK_SEND_HEADER.length, 
								bufferLine.indexOf(NETWORK_SEND_END_HEADER)
							);
							sendNetworkMessage(messageStripped);
						} else {
							console.log(bufferLine);				//not a network message, probably debug message
						}
					}
					
				}
			});

			port.on('close', function(data)
			{
				console.log("Serial port closed.");
			});
			
			port.on('error', function(data)
			{
				console.log("Serial port error!");
				console.log(data);
			});
			
			
			arduinoPort = port;
			return;
		}

	});
});




//=================================
//Socket.io

socket = socket(url + ':' + port);

socket.on('connect', function (data) 
{
	console.log('Connected to Server. ' + socket.id);			
	socket.emit('client_connect', 
	{ 
		username: username,
		platform : 'Arduino'
	});

	emitEvent('connect', data);
});

socket.on('disconnect', function(data)
{
	emitEvent('disconnect', data);
});

socket.on('message', function (data) 
{
	//console.log('Message Recieved:' + JSON.stringify(data));
	console.log('Message Recieved.');
	emitEvent('message', data);
});

socket.on('error', function (err)
{
	console.log ('Error! ' + err);
	emitEvent('error', err);
});


//sends event to arduino
function emitEvent(eventName, args)
{
	var sendString = 
	{
		event_name: eventName,
		args : args
	}
	
	sendMessageArduino(JSON.stringify(sendString));
}


function sendMessageArduino(msg)
{
		
	arduinoPort.write(msg + '\n' ,function(){
		console.log("Serial message sent=" + msg);
	});
	
	arduinoPort.flush();
}

function sendNetworkMessage(msgStr)
{
	console.log("Sending network message=" + msgStr);
	var msgSend = JSON.parse(msgStr);
	msgSend.username = username;			//add username and send
	socket.emit("message", msgSend);
	
}