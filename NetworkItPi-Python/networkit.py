import json
import logging
from threading import Thread
#Uses socket.io v0.7.5- Nexus (i.e. a beta for Socket.io 2.0 servers)
#https://github.com/invisibleroads/socketIO-client
#https://github.com/nexus-devs/socketIO-client      (beta branch)





class Client:

    username = "demo_test_username"
    url = "581.cpsc.ucalgary.ca"
    port = 8000

    socket = None
    socket_thread = None

    EVENT_MESSAGE = "message"
    EVENT_CONNECT = "connect"
    EVENT_DISCONNECT = "disconnect"
    EVENT_ERROR = "error"

    def __init__(self):
        # client uses default settings for connection

        # Setup events
        self.subscribers = {}
        self.subscribers[self.EVENT_MESSAGE] = []
        self.subscribers[self.EVENT_CONNECT] = []
        self.subscribers[self.EVENT_DISCONNECT] = []
        self.subscribers[self.EVENT_ERROR] = []


    def __init__(self, username, url, port):

        self.username = username
        self.url = url
        self.port = port


        #Setup events
        self.subscribers = {}
        self.subscribers[self.EVENT_MESSAGE] = []
        self.subscribers[self.EVENT_CONNECT] = []
        self.subscribers[self.EVENT_DISCONNECT] = []
        self.subscribers[self.EVENT_ERROR] = []



    def start_connection(self):
        #if already connected, disconnect

        if (self.socket != None):
            print "Already connected!"
            return;

        # start up logger
        logging.getLogger('socketIO-client').setLevel(logging.DEBUG)
        logging.basicConfig()

        # setup networking
        from socketIO_client_nexus import SocketIO, LoggingNamespace, BaseNamespace
        self.socket = SocketIO(self.url, self.port, BaseNamespace, wait_for_connection=False)


        #events
        self.socket.on('connect', self.on_connect)
        self.socket.on('disconnect', self.on_disconnect)
        self.socket.on('error', self.on_socket_error)
        self.socket.on('message', self.on_message)

        self.socket.emit('client_connect', {
            'username': self.username,
            'platform': "Python 2.7"
        })
        self.socket_thread = Thread(target = self.wait_on_socket)
        self.socket_thread.start()

    def close_connection(self):
        if (self.socket != None):
            self.socket.disconnect()

    def wait_on_socket(self):
        self.socket.wait()
        print "SocketIO client thread terminated"

    def send_message(self, message):

        sendFields = []
        for i in range(0, len(message.fields)):
            sendFields.append({'key': message.fields[i].key, 'value' : message.fields[i].value})

        sendMessage = {                                      #Must be here, problem with library being beta.
            'username' : self.username,
            'deliverToSelf' : message.deliverToSelf,
            'subject' : message.subject,
            'fields' : sendFields
        }
        self.socket.emit('message', sendMessage)

    #==========================
    #Events

    def on_connect(self, args):
        print "Connection Sucessful"
        #self.socket.emit('client_connect', {
        #    'username': self.username,
        #    'platform': "Python 2.7"
        #})
        self.emit_event(self.EVENT_CONNECT, None)



    def on_disconnect(self, args):
        print "Client Disconnected"
        self.emit_event(self.EVENT_DISCONNECT, None)

    def on_socket_error(self, err):
        print "Error!"
        print err
        self.emit_event(self.EVENT_ERROR, err)



    def on_message(self, data):

        #For some reason JSON data looks like: '2["message",{"username":"demo_test_username","deliverToSelf":true,"subject":"Poke!","fields":[{"key":"num1","value":"3"},{"key":"num2","value":"4"},{"key":"count","value":"6"}]}]'
        #Only accept this message as an object
        messageStr = str(data)

        #invalid message
        if (messageStr.find("2[") != 0):
            return


        messageStr = messageStr[1:]
        messageObj = json.loads(messageStr)[1]

        #convert to message object
        message = Message(messageObj['subject'])
        message.deliverToSelf = messageObj['deliverToSelf']

        #Transfer all fields
        fieldsDict = messageObj['fields']
        for i in range(0, len(messageObj['fields'])):
            message.add_field(fieldsDict[i]['key'], fieldsDict[i]['value'])

        self.emit_event(self.EVENT_MESSAGE, message)



    #=========
    #events external


    def register_listener(self, event_name, callback):
        self.subscribers[event_name].append(callback)

    def emit_event(self, event_name, args):
        for i in range(0, len(self.subscribers[event_name])):
            self.subscribers[event_name][i](args)



#=======================


class Message:

    fields = []

    def __init__(self, subject):
        self.subject = subject
        self.deliverToSelf = False
        self.fields = []

    def add_field(self, key, value):
        self.fields.append(Field(key, value))

    def get_field(self, key):
        for i in range(0, len(self.fields)):
            if (self.fields[i].key == key):
                return self.fields[i].value

        return None

    def to_string(self):
        return "NOT IMPLEMENTED YET"



class Field:

    def __init__(self, key, value):
        self.key = key
        self.value = value

    def to_string(self):
        return self.key + " : " + str(self.value);
