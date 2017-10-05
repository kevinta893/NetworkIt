var http = require('http').createServer(handler)
var io = require('socket.io')(http);
var dateFormat = require('dateformat');


var PORT = 8000;

var clients = [];


//redirect html server
function handler (req, res) {
  fs.readFile(__dirname + '/index.html',
  function (err, data) {
    if (err) {
      res.writeHead(500);
      return res.end('Error loading index.html');
    }

    res.writeHead(200);
    res.end(data);
  });
}


//eventsio
io.on('connection', function(socket)
{
    writeLog('Connection recieved. ' + socket.request.connection.remoteAddress + ":" + socket.request.connection.remotePort + "?id=" + socket.id);
	
	//all clients are required to identify themselves with a username
	socket.on("client_connect", function(data) 
    {
		var user = {};
		user.socket = socket;
		user.socketId = socket.id;
        user.username = data['username'];
		user.platform = data['platform'];
		clients.push(user);
		
		writeLog('Client registered. ' + user.username + '@' + socket.request.connection.remoteAddress + ":" + socket.request.connection.remotePort + "?id=" + socket.id + '?platform=' + user.platform);
	});
	
	//client events
	socket.on('disconnect', function() 
    {


        //remove user
        for (var i = 0; i < clients.length; i++)
        {

			if (clients[i].socketId === socket.id)
            {
				var user =  clients[i];
				writeLog('Client disconnected. ' + user.username + '@' + socket.request.connection.remoteAddress + ":" + socket.request.connection.remotePort + "?id=" + socket.id + '?platform=' + user.platform);
				clients.splice(i, 1);
				return;
			}
		}

        //should never be here if client registered
		writeLog("Warning, could not find disconnected user. id=" + socket.id);
    });


	//message from client
	socket.on('message', function(msg)
    {

        writeLog('Message recieved: ' + JSON.stringify(msg));


        var username = msg['username'];
        var subject = msg['subject'];
        var deliverToSelf = msg['deliverToSelf'];
        var fields = msg['fields'];

        //send to self
        if (deliverToSelf == true)
        {
            socket.emit('message', msg);
        }


        //forward to appropriate users
        for (var i = 0; i < clients.length; i++)
        {
            var user = clients[i];

            //send to other devices of same username but not to self
            if (user.username == username && user.socketId != socket.id)
            {
                //writelog('Forwarding to socket.id = ' + user.socketId);
                user.socket.emit('message', msg);
            }
        }

	});
	
	
});



//start server
http.listen(PORT, function()
{
	console.log('listening on *:' + PORT);
});


//===============================
//Utility Functions

function writeLog(msg) {
    console.log('[' + timeStamp() + '] ' + msg);
}

function timeStamp()
{
    return dateFormat(new Date(), 'yyyy-mm-dd HH:MM.ss.l');
}