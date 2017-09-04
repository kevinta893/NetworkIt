var socket = io.connect('http://localhost:8000');

socket.on('connect', function (data) {
	console.log('Connected to Server. ' + socket.id);
	document.getElementById("status").innerHTML = 'Connected to Server.';
	document.getElementById("status_img").src = 'images/green_circle.png';
	socket.emit('my other event', { my: 'data' });
	
});
