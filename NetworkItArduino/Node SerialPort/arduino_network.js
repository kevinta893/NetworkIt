var username = 'demo_test_username';			//defaults
var url = 'http://localhost';
var port = 8000;

var socket = require('socket.io-client');
var serialport = require('serialport');

var arduinoPort;

var serialBuffer = '';

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
				if (serialBuffer[serialBuffer.length-1] == '\n'){
					serialBuffer = serialBuffer.substring(0, serialBuffer.length-2);		//strip out the new line
					console.log(serialBuffer);
					serialBuffer = '';
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


function sendMessageArduino(msg){
		
	arduinoPort.write(msg + '\n' ,function(){
		console.log("Serial message sent=" + msg);
	});
	
	arduinoPort.flush();
}