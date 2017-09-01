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
            //client.Error += Client_Error;
            client.MessageReceived += Client_MessageReceived;
            client.StartConnection();


            Message m = new Message("Poke!");
            m.AddField("num1", 3);
            m.AddField("num2", 4);

            client.SendMessage(m);
        }



        private void Client_Error(object sender, Exception e)
        {
            WriteLog(e.Message + "\n" + e.StackTrace + "\n");
        }

        private void Client_MessageReceived(object sender, NetworkItMessageEventArgs e)
        {
            WriteLog(e.ReceivedMessage.Fields.ToString());
        }


        private void WriteLog(string message)
        {
            lblLog.Text += "[" + DateTime.Now + "] " + message + "\n";
        }
    }
}
