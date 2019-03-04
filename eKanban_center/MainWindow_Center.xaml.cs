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

namespace eKanban_center
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow_Center : Window
    {
        
        public MainWindow_Center()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTcpServer();
        }
        private void img_settings_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void img_close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try {
                if (tcpServer.IsStarted)
                    tcpServer.Stop();
            } catch(Exception err) { Console.WriteLine(err.Message); }
            this.Close();
        }

    }
}
