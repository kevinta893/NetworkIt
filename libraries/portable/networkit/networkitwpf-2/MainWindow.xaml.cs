using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetworkItWPF;

namespace NetworkItWPF_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        Client testClient;
        Ellipse el;

        public MainWindow()
        {
            InitializeComponent();

            testClient = new Client("FFF", "581.cpsc.ucalgary.ca", 8000);
            testClient.Connected += testClient_Connected;
            testClient.Error += testClient_Error;
            this.testClient.MessageReceived += testClient_MessageReceived;

            el = new Ellipse()
            {
                Fill = Brushes.Red,
                Width = 100,
                Height = 100
            };

            this.Container.Children.Add(el);

        }

        void testClient_MessageReceived(object sender, NetworkItMessageEventArgs e)
        {
           // System.Diagnostics.Debug.WriteLine(e.ReceivedMessage.Fields);
            if (e.ReceivedMessage.Name == "POTATO")
            {
                int count = Int32.Parse(e.ReceivedMessage.GetField("value"));
                System.Diagnostics.Debug.WriteLine("The value is " + count);
            }
            if(e.ReceivedMessage.Name == "Pointer")
            {
                double x = Double.Parse(e.ReceivedMessage.GetField("x"));
                double y = Double.Parse(e.ReceivedMessage.GetField("y"));

                Dispatcher.Invoke(new Action(delegate
                    {
                        Canvas.SetLeft(this.el, x);
                        Canvas.SetTop(this.el, y);
                    }));


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
