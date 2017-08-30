﻿using System;
using SocketIOClient.WinRT;
using System.Diagnostics;


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
        private SocketIOClient.WinRT.Client client;

        public string IPAddress
        {
            get
            {
                return this.ipAddress;
            }
            set
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
            set
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

        public async void StartConnection()
        {
            this.client = new SocketIOClient.WinRT.Client(this.address);
            this.client.Error += OnError;
            this.client.Message += OnMessage;

            this.client.On("connect", (fn) =>
            {
                System.Diagnostics.Debug.WriteLine("Connection Successful");

                this.client.Emit("ClientConnect", new
                {
                    this.username
                });

                RaiseConnected(new EventArgs());

            });
            await this.client.ConnectAsync();
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
            Debug.WriteLine("ERROR! :(");
            Debug.WriteLine(e.Exception);
            RaiseError(new EventArgs());
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

        public event EventHandler<EventArgs> Error;

        private void RaiseError(EventArgs e)
        {
            if(Error != null)
            {
                Error(this, e);
            }
        }

        public event EventHandler<NetworkItMessageEventArgs> MessageReceived;

        private void RaiseMessageReceived(NetworkItMessageEventArgs e)
        {
            if(MessageReceived != null)
            {
                MessageReceived(this, e);
            }
        }

        #endregion

        public Client()
        {
            this.address = "http://" + ipAddress + ":" + port;
            StartConnection();
        }

        public Client(string username, string ipAddress, int port)
        {
            this.username = username;
            this.ipAddress = ipAddress;
            this.port = port;
            this.address = "http://" + ipAddress + ":" + port;
            StartConnection();
        }

    }
}
