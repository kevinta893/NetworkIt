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
using System.IO;

using NetworkIt;

namespace NetworkItPokeDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Client client;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            client = new Client(txtUsername.Text, txtURL.Text, 8000);
            client.Error += Client_Error;
            client.MessageReceived += Client_MessageReceived;
            client.StartConnection();


            Message m = new Message("Poke!");
            m.AddField("num1", 3);
            m.AddField("num2", 4);

            client.SendMessage(m);
        }



        private void Client_Error(object sender, Exception e)
        {
            WriteLogLine(e.Message + "\n" + e.StackTrace);
        }

        private void Client_MessageReceived(object sender, NetworkItMessageEventArgs e)
        {
            WriteLogLine(e.ReceivedMessage.Fields.ToString());
        }


        private void ScrollLogToBottom()
        {
            scrLog.UpdateLayout();
            scrLog.ScrollToVerticalOffset(scrLog.ScrollableHeight);
        }

        private void WriteLogLine(string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                lblLog.Text += "[" + TimeStamp()+ "] " + message + "\n";
                ScrollLogToBottom();
            }));
            
        }


        private string TimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd H:mm.ss");            //24 Hour format
        }
    }
}
