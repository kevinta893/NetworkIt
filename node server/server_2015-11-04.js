var app = require('http').createServer(handler),
	io = require('socket.io').listen(app),
	static = require('node-static'),
	fs = new static.Server();

// If the URL of the server is opened in a browser.
function handler(request, response)
{
	request.addListener('end', function() {
		fs.serve(request, response);
	}).resume();
}

app.listen(8000);

io.set('log level', 1);

console.log('Server started. [' + (new Date()).toString() + ']');


/* CLIENT SOCKET SESSION IDs */
var clients = {};
var numClients = {};


io.sockets.on('connection', function(socket)
{
	socket.on('ClientConnect', function(data)
	{
		var username = data['username'];

		console.log((new Date()).toString() + ': ' + username + ' connected, given socket.id ' + socket.id);

		if (!clients[username])
		{
			clients[username] = {};
			numClients[username] = 0;
		}
		clients[username][numClients[username]] = socket.id;
		numClients[username]++;
	});

	socket.on('message', function(data)
	{
		var username = data['username'];
		var messageName = data['messageName'];
		var fields = data['fields'];

		console.log((new Date()).toString() + ': ' + messageName + ' message sent by ' + username);

		console.log('-=-=-=-=-=-=');

		console.log(data);

		for (var key in clients[username])
		{
			var client = clients[username][key];

			console.log('socket.id = ' + client);
			io.sockets.socket(client).emit('message', data);
		}
	});

	socket.on('disconnect', function(data)
	{
			for(var client in clients)
			{
				for(var item in client)
				{
					console.log(client[item]);
				}
				console.log("----");
			}
			console.log('disconnected ' + socket.id);

	});

});
