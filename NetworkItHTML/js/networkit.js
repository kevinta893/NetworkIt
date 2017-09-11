class Message
{
	constructor (subject)
	{
		this.subject = subject;
		this.fields = [];
		this.deliverToSelf = false;
	}
	
	addField (key, value)
	{
		var f = new Field (key,value)
		this.fields.push(f);
	}
	
	getField (key)
	{
		for (var i = 0 ; i < fields.length ; i++)
		{
			if (fields[i].key == key)
			{
				return fields[i];
			}
		}
	}
	
	toString(){
		return JSON.stringify(this);
	}
}

class Field
{
	constructor(key, value)
	{
		this.key = key;
		this.value = value;
	}
	
	toString()
	{
		return key + ' : ' + value;
	}
}


//============================
//socket.io required

class Client
{
	constructor(username, url, port){
	
		//event listeners
		this.eventListeners = 
		{
			message : [],
			connect : [],
			disconnect : [],
			error : []
		};
		
		//setup socket.io
		this.username = 'demo_test_username';			//defaults
		this.url = 'http://localhost';
		this.port = 8000;
		
		var socket = io.connect(url + ':' + port);
		
		var parent = this;
		
		socket.on('connect', function (data) 
		{
			console.log('Connected to Server. ' + socket.id);			
			socket.emit('client_connect', 
			{ 
				username: username 
			});
		
			parent.emitEvent('connect', data);
		});

		socket.on('disconnect', function(data)
		{
			parent.emitEvent('disconnect', data);
		});

		socket.on('message', function (data) 
		{
			//console.log('Message Recieved:' + JSON.stringify(data));
			console.log('Message Recieved.');
			parent.emitEvent('message', data);
		});

		socket.on('error', function (err)
		{
			console.log ('Error! ' + err);
			parent.emitEvent('error', err);
		});
		
		this.socket = socket;
	}
	
	closeConnection()
	{
		this.socket.close();
	}
	
	
	
	
	sendMessage(message){
		var sendMsg = 
		{
			username : this.username,
			subject : message.subject,
			deliverToSelf : message.deliverToSelf,
			fields : message.fields
		};
		
		this.socket.emit('message', sendMsg);
	}
	
	
	//Event handlers
	emitEvent(eventHeader, arg)
	{
		var callbackList = this.eventListeners[eventHeader];
		
		for (var i = 0 ; i < callbackList.length ; i++ )
		{
			callbackList[i](arg);
		}
	}
	
	addMessageListener(callback)
	{
		this.eventListeners['message'].push(callback);
	}

	addConnectListener(callback)
	{
		this.eventListeners['connect'].push(callback);
	}
	
	addDisconnectListener(callback)
	{
		this.eventListeners['disconnect'].push(callback);
	}

	addErrorListener(callback)
	{
		this.eventListeners['error'].push(callback);
	}
	

}
