var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);

var PORT = 8000;

var clients = [];

//http server
app.get('/', function(req, res)
{
	res.sendFile(__dirname + '/index.html');
});


//events
io.on('connection', function(socket)
{
	console.log('User connected');
	socket.on('disconnect', function() 
	{
		console.log('User disconnected');
		//remove user
		var clientIndex = clients.indexOf(socket);
		if (clientIndex >=0)
		{
			clients.splice(clientIndex, 1);
		}
	});
	
	socket.on('message', function(msg)
	{
		relayMessage(msg);				//forward to appropriate user
	});
	
	clients.push(socket);
	
});



//start server
http.listen(PORT, function()
{
	console.log('listening on *:' + PORT);
});





function relayMessage(msg)
{
	
	console.log('message recieved:' + msg);

	
	var username = msg['username'];
	var messageName = data['messageName'];
	var fields = data['fields'];
	
	
	for (var user in clients[username])
	{
		user.socket.emit('message', msg);
	}
}

function getDateString()
{
	
}