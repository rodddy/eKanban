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
using System.Windows.Shapes;

namespace eKanban_Console
{
    /// <summary>
    /// OutputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OutputDialog : Window
    {
        public int status = 0;
        public OutputDialog()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            status = 2;
            this.Close();
        }

        private void btn_no_Click(object sender, RoutedEventArgs e)
        {
            status = 1;
            this.Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            status = 0;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow_Console.frm.status = status;
        }
    }
}
