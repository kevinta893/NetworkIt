using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NetworkIt;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetworkItTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Client testClient;
        public MainPage()
        {
            this.InitializeComponent();
            testClient = new Client("FFF", "581.cpsc.ucalgary.ca", 8000);
            testClient.Connected += testClient_Connected;
            testClient.Error += testClient_Error;
            this.testClient.MessageReceived += testClient_MessageReceived;

            this.Container.PointerMoved += Container_PointerMoved;
        }

        void Container_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Message message = new Message("Pointer");
            message.AddField<double>("x", e.GetCurrentPoint(this.Container).Position.X);
            message.AddField<double>("y", e.GetCurrentPoint(this.Container).Position.Y);
            this.testClient.SendMessage(message);
           // System.Diagnostics.Debug.WriteLine(e.GetCurrentPoint(this.Container).Position.X);
        }

        void testClient_MessageReceived(object sender, NetworkItMessageEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ReceivedMessage.Fields);
            if(e.ReceivedMessage.Name == "POTATO")
            {
               // int count = Int32.Parse(e.ReceivedMessage.GetField("value"));
               // System.Diagnostics.Debug.WriteLine("The value is " + count);
            }
        }

        void testClient_Error(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ERROR!");
        }

        void testClient_Connected(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("CONNECTED :D");
            Message potato = new Message("POTATO");
            potato.AddField<int>("value", 5);
            this.testClient.SendMessage(potato);
        }
    }
}
