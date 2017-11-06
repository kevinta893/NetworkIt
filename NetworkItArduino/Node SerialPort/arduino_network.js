var username = 'demo_test_username';			//defaults
var url = 'http://581.cpsc.ucalgary.ca';
var port = 8000;

var SERIAL_BAUD_RATE = 115200;					//make sure this is the same in Arduino and Serial Monitor
var LINE_TERMINATOR = '\n';

var socket = require('socket.io-client');
var serialport = require('serialport');

var arduinoPort;

var serialBuffer = '';

var NETWORK_SEND_HEADER = 'send_networkit_message=';
var NETWORK_SEND_END_HEADER = '=send_end';

//=================================
//Stage 1: get serial port COM name from user

//get args if any
var argv = process.argv.slice(2);

//use the first arg as the serial port name
if (argv.length >= 1)
{
	//attempt to connect and run to the com port requested
	initSerialPort(argv[0]);
}
else{
	//get list of all Arduinos ports
	serialport.list(function(err, ports)
	{
		
		var arduinoList = [];
		ports.forEach(function(p)
		{
			if(p["manufacturer"] && p["manufacturer"].includes("Arduino"))
			{
				arduinoList.push(p);
			}
		});
		
		//list collected, now get user's input if arduino > 2
		if (arduinoList.length <= 0)
		{
			console.log("No Arduinos connected or available, check to see if they have started a serial port (e.g. Serial.begin("+SERIAL_BAUD_RATE+")) and not in use (e.g. Arduino IDE's Serial Monitor).");
			console.log("Quitting...");
			process.exit(0);
		}
		else if (arduinoList.length == 1)
		{
			//only one arduino, connect to that automatically
			console.log("One Arduino found. Connecting...");
			initSerialPort(arduinoList[0]['comName']);
		}
		else
		{
			//multiple Arduinos. Let user choose
			console.log("Multiple Arduinos found (n=" + arduinoList.length +"). Choose one of the following options:");
			for (var i =0 ; i < arduinoList.length ; i++)
			{
				console.log(i + ".\t" + arduinoList[i]['comName']);
			}
			console.log(arduinoList.length + ".\tExit Program")
			
			//command line ask
			var minOption = 0;
			var maxOption = arduinoList.length;
			
			process.stdin.resume();
			process.stdin.setEncoding('utf-8');
			
			var optionNum = -1;
			
			process.stdin.on('data', function(input){		
				optionNum = parseInt(input);
				
				if (optionNum >= minOption && optionNum <=maxOption)
				{
					if (optionNum == maxOption)
					{
						//quit option
						console.log("Quitting...");
						process.exit();
					}
					
					//start the serial port
					process.stdin.destroy();
				}
				else
				{
					console.log("Please enter an option between " + minOption + "-" + maxOption);
				}
			});
			
			process.stdin.on('close', function ()
			{
				//input entry is done, now hookup the rest of the program
				var comName = arduinoList[optionNum]['comName'];
				console.log("Connecting to Arduino at " + comName);
				initSerialPort(comName);
			});
			
		}
	});
}




//============================================
// Stage 2: Open a serial port to arduino


//opens a serial port connection with the COM name, starts socket.io connection when done
function initSerialPort(comName)
{
	var sPort = new serialport(comName, 
	{
		baudRate: SERIAL_BAUD_RATE, 
	});

	sPort.on('open', function(data)
	{
		console.log("Arduino serialport connected.");
		
		//serial port is ready, now start network connection
		initSocketIO();
		
	});
	
	sPort.on('data', function(data)
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

	sPort.on('close', function(data)
	{
		console.log("Serial port closed.");
	});
	
	sPort.on('error', function(data)
	{
		console.log("Serial port error!");
		console.log(data);
	});
	
	
	arduinoPort = sPort;
	return;
}



//=================================
//Stage 3: Establish connection to NetworkIt

function initSocketIO()
{
	console.log("Connecting to NetworkIt server: " + username + "@" + url + ":" + port.toString());
	socket = socket(url + ':' + port);

	socket.on('connect', function (data) 
	{
		socket.emit('client_connect', 
		{ 
			username: username,
			platform : 'Arduino'
		});
		console.log('Connected to NetworkItServer. ' + socket.id);			
		console.log('NetworkIt is ready and is listening for messages.');
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
}

//=======================================
//NetworkIt-serial functions

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
		
	arduinoPort.write(msg + LINE_TERMINATOR ,function(){
		console.log("Serial message sent=" + msg);
	});
	
	arduinoPort.flush();
}

function sendNetworkMessage(msgStr)
{
	console.log("Sending network message=" + msgStr);
	try{
		var msgSend = JSON.parse(msgStr);
		msgSend.username = username;			//add username and send
		socket.emit("message", msgSend);
	} catch (e) {
		console.log(e);
	}
}