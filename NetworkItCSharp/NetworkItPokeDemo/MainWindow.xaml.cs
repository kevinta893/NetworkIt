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

        private void btnPoke_Click(object sender, RoutedEventArgs e)
        {
            client = new Client(txtUsername.Text, txtURL.Text, 8000);
            client.Error += Client_Error;
        }

        private void Client_Error(object sender, EventArgs e)
        {
            lblLog.Text += e.ToString() + "\n";
        }
    }
}
