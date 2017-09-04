using System;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;

namespace NetworkIt
{
    public class NetworkItMessageEventArgs : EventArgs
    {
        public Message ReceivedMessage { get; set; }

        public NetworkItMessageEventArgs(Message receivedMessage)
        {
            this.ReceivedMessage = receivedMessage;
        }
    }

    public class Client
    {
        private string username = "null";
        private string ipAddress = "581.cpsc.ucalgary.ca";
        private int port = 8000;
        private string address;
        private Socket client;

        public string IPAddress
        {
            get
            {
                return this.ipAddress;
            }
            private set
            {
                this.ipAddress = value;
            }
        }

        public int Port
        {
            get
            {
                return this.port;
            }
            private set
            {
                this.port = value;
            }
        }

        public string Username
        {
            get
            {
                return this.username;
            }
            set
            {
                this.username = value;
            }
        }

        public void StartConnection()
        {
            //ensure only one client is open at a time
            if (this.client != null)
            {
                this.client.Close();
            }

            
            this.client = IO.Socket("http://localhost:8000");

            this.client.On(Socket.EVENT_ERROR, (e) =>
            {
                RaiseError((Exception) e);
            });

            this.client.On(Socket.EVENT_MESSAGE, (e) =>
            {
                RaiseMessageReceived(new NetworkItMessageEventArgs(new Message("HEYYYYY")));
            });

            this.client.On(Socket.EVENT_CONNECT, (fn) =>
            {
                System.Diagnostics.Debug.WriteLine("Connection Successful");

                this.client.Emit("ClientConnect", new
                {
                    this.username
                });

                RaiseConnected(new EventArgs());

            });

            this.client.On(Socket.EVENT_DISCONNECT, (fn) =>
            {
                RaiseDisconnected();
            });
        }


        public void CloseConnection()
        {
            this.client.Close();
        }

        public void SendMessage(Message message)
        {
            string objStr = JsonConvert.SerializeObject(new
            {
                username = this.username,
                messageName = message.Name,
                fields = message.Fields
            });

            this.client.Emit("message", objStr);
        }
  /*
        void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Event == "message")
            {
                Message myMessage = e.Message.Json.GetFirstArgAs<Message>();
                RaiseMessageReceived(new NetworkItMessageEventArgs(myMessage));
            }
        }

        void OnError(object sender, ErrorEventArgs e)
        {
            Exception eExtra = new Exception(e.Message, e.Exception);
            Console.WriteLine("ERROR! :(");
            Console.WriteLine(eExtra);


            RaiseError(eExtra);
        }
        */


        #region Custom Events

        public event EventHandler<EventArgs> Connected;

        private void RaiseConnected(EventArgs e)
        {
            if (Connected != null)
            {
                Connected(this, e);
            }
        }

        public event EventHandler<Exception> Error;

        private void RaiseError(Exception e)
        {
            if (Error != null)
            {
                Error(this, e);
            }
        }

        public event EventHandler<NetworkItMessageEventArgs> MessageReceived;

        private void RaiseMessageReceived(NetworkItMessageEventArgs e)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, e);
            }
        }

        public event EventHandler<object> Disconnected;

        private void RaiseDisconnected()
        {
            if (Disconnected != null)
            {
                Disconnected(this, null);
            }
        }



        #endregion

        public Client()
        {
            this.address = "http://" + ipAddress + ":" + port;
        }

        public Client(string username, string ipAddress, int port)
        {
            this.username = username;
            this.ipAddress = ipAddress;
            this.port = port;
            this.address = "http://" + ipAddress + ":" + port;
        }



    }
}
