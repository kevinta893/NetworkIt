using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private string username = "demo_test_username";
        private string url = "581.cpsc.ucalgary.ca";
        private int port = 8000;
        private string address;
        private Socket client;

        public string IPAddress
        {
            get
            {
                return this.url;
            }
        }

        public int Port
        {
            get
            {
                return this.port;
            }
        }

        public string Username
        {
            get
            {
                return this.username;
            }
        }

        public void StartConnection()
        {
            //ensure only one client is open at a time
            if (this.client != null)
            {
                client.Close();
            }


            this.client = IO.Socket(url + ":" + port);

            this.client.On(Socket.EVENT_CONNECT, (fn) =>
            {
                System.Diagnostics.Debug.WriteLine("Connection Successful");

                JObject connectJson = new JObject();
                connectJson.Add("username", username);
                this.client.Emit("client_connect", connectJson);

                RaiseConnected(new EventArgs());

            });

            this.client.On("disconnect", (data) =>
            {
                RaiseDisconnected(new EventArgs());
            });

            this.client.On(Socket.EVENT_ERROR, (e) =>
            {
                //RaiseError(new Exception("Oh no something awful"));
            });

            this.client.On(Socket.EVENT_MESSAGE, (e) =>
            {
                
                RaiseMessageReceived(new NetworkItMessageEventArgs(new Message("HEYYYYY")));
            });

            

        }

        public void CloseConnection()
        {
            this.client.Close();
        }

        public void SendMessage(Message message)
        {
            this.client.Emit("message",
                new
                {
                    username = this.username,
                    messageName = message.Name,
                    fields = message.Fields
                });
        }


        #region Custom Events

        public event EventHandler<EventArgs> Connected;

        private void RaiseConnected(EventArgs e)
        {
            if (Connected != null)
            {
                Connected(this, e);
            }
        }

        public event EventHandler<EventArgs> Disconnected;

        private void RaiseDisconnected(EventArgs e)
        {
            if (Connected != null)
            {
                Disconnected(this, e);
            }
        }

        public event EventHandler<EventArgs> Error;

        private void RaiseError(EventArgs e)
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

        #endregion


        /// <summary>
        /// Use the default client settings
        /// </summary>
        public Client()
        {
            this.Initialize(this.username, this.url, this.port);
        }


        /// <summary>
        /// Connect client to the server using specified settings
        /// </summary>
        /// <param name="username"></param>
        /// <param name="ipAddress">Must include the http protocol "http://"</param>
        /// <param name="port"></param>
        public Client(string username, string url, int port)
        {
            this.Initialize(username, url, port);
        }

        private void Initialize(string username, string url, int port)
        {
            this.username = username;
            this.port = port;

            if (url.IndexOf("http://") != 0)
                throw new ArgumentException("URL must start with http://");

            this.url = url;
        }



    }
}
