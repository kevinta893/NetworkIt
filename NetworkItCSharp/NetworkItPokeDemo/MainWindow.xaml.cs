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
using Microsoft.Win32;

namespace NetworkItPokeDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Client client;
        private const int DEFAULT_PORT = 8000;

        private int messageCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            enableConnectButton(true);
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {

            Message m = new Message("Poke!");
            m.DeliverToSelf = chkDeliverToSelf.IsChecked ?? false;
            m.AddField("num1", 3);
            m.AddField("num2", 4);
            m.AddField("count", messageCount++);
            client.SendMessage(m);

        }




        private void btnSaveLog_Click(object sender, RoutedEventArgs e)
        {
            if (lblLog.Text.Length <= 0)
            {
                MessageBox.Show("No log to save!", "Information", MessageBoxButton.OK);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text file (*.txt,*.log)|*.txt;*.log";
            if (saveDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveDialog.FileName, lblLog.Text);
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (btnConnect.Content.Equals("Connect"))
            {

                int port = -1;
                int.TryParse(txtPort.Text, out port);
                port = port == -1 ? DEFAULT_PORT : port;

                WriteLogLine("Attempting to connect to: " + txtUsername.Text + "@" + txtURL.Text + ":" + port);

                client = new Client(txtUsername.Text, txtURL.Text, port);
                client.Error += Client_Error;
                client.MessageReceived += Client_MessageReceived;
                client.Connected += Client_Connected;
                client.Disconnected += Client_Disconnected;
                client.StartConnection();

                enableConnectButton(true);
            }
            else if (btnConnect.Content.Equals("Disconnect"))
            {
                enableConnectButton(false);
                client.CloseConnection();
            }
        }

        //changes interface for when connected or not
        private void enableConnectButton(bool enabled)
        {
            if (enabled == true)
            {
                btnConnect.Content = "Connect";
                btnSend.ToolTip = "Please connect to a server first";
                btnSend.IsEnabled = false;
            }
            else
            {
                btnConnect.Content = "Disconnect";
                btnSend.ToolTip = "";
                btnSend.IsEnabled = true;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Clear the log?","Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //clear log
                lblLog.Text = "";
            }
        }

        #region Network Events

        private void Client_Disconnected(object sender, object e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                elpStatus.Fill = new SolidColorBrush(Colors.Red);
                enableConnectButton(true);

                WriteLogLine("Client Disconnected");
                client.CloseConnection();
            }));
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                elpStatus.Fill = new SolidColorBrush (Colors.ForestGreen);
                enableConnectButton(false);
            }));
            WriteLogLine("Connection Successful");
        }

        private void Client_Error(object sender, Exception e)
        {
            WriteLogLine(e.Message + "\n" + e.StackTrace);
        }

        private void Client_MessageReceived(object sender, NetworkItMessageEventArgs e)
        {
            WriteLogLine(e.ReceivedMessage.ToString());
        }

        #endregion





        #region Utility Functions

        private void WriteLogLine(string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                lblLog.Text += "[" + TimeStamp() + "] " + message + "\n";
                ScrollLogToBottom();
            }));

        }


        private string TimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd H:mm.ss");            //24 Hour format
        }



        private void ScrollLogToBottom()
        {
            scrLog.UpdateLayout();
            scrLog.ScrollToVerticalOffset(scrLog.ScrollableHeight);
        }

        #endregion

    }
}
