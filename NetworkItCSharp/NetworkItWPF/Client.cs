using System;
using System.Diagnostics;
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
        private string url = "http://581.cpsc.ucalgary.ca";
        private int port = 8000;
        private Socket client;

        #region Fields
        public string URL
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

        #endregion


        public void StartConnection()
        {
            //ensure only one client is open at a time
            if (this.client != null)
            {
                Debug.WriteLine("Already connected!");
                return;
            }


            this.client = IO.Socket(URL);

            this.client.On(Socket.EVENT_CONNECT, () =>
            {
                Debug.WriteLine("Connection Successful");

                this.client.Emit("client_connect", JObject.FromObject(new
                {
                    this.username
                }));

                RaiseConnected(new EventArgs());

            });

            this.client.On("message", (data) =>
            {
                Debug.WriteLine("Message Recieved");
                Message recv = ((JObject)data).ToObject<Message>();
                Debug.WriteLine("Message Recieved" + data.ToString());
                RaiseMessageReceived(new NetworkItMessageEventArgs(recv));
            });

            this.client.On(Socket.EVENT_ERROR, (e) =>
            {
                Debug.WriteLine("Error!");
                RaiseError((Exception)e);
            });

            this.client.On(Socket.EVENT_DISCONNECT, (fn) =>
            {
                Debug.WriteLine("Client Disconnected");
                RaiseDisconnected();
            });


        }


        public void CloseConnection()
        {
            this.client.Close();
        }

        public void SendMessage(Message message)
        { 
            this.client.Emit("message", JObject.FromObject(new
            {
                username = this.username,
                deliverToSelf = message.DeliverToSelf,
                subject = message.Subject,
                fields = message.Fields
            }));
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

        public event EventHandler<Exception> Error;

        private void RaiseError(Exception e)
        {
            Debug.WriteLine("Error! :(");
            Debug.WriteLine(e.StackTrace);

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

            this.url = url + ":" + port;
        }

    }
}
