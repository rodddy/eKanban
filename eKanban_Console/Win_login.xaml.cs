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

using System.IO;

namespace eKanban_Console
{
    /// <summary>
    /// Win_login.xaml 的交互逻辑
    /// </summary>
    public partial class Win_login : Window
    {
        string InfoFile = "userinfo.txt";
        //string InfoFile = "LiveCharts.xml"; 

        private string[] strInfoList = new string[40];
        public Win_login()
        {
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitData();
        }

        private void InitData()
        {
            //string InfoFile = Environment.CurrentDirectory + "\\userInfo.txt";

            string data = "";
            try
            {

                if (File.Exists(InfoFile))
                {
                    StreamReader sr = new StreamReader(InfoFile, Encoding.Default);
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        data += line;
                    }
                    strInfoList = data.Split(';');
                    sr.Close();
                }
                else {

                }

            }
            catch (Exception err) { Console.WriteLine(err.Message); }

        }

        private void img_close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }

        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            string str = tb_usr.Text + "," + tb_psd.Password;
            if (Array.IndexOf(strInfoList, str) >= 0)
            {
                Setting_Win_V2 setting_frm = new Setting_Win_V2();
                setting_frm.Show();
                this.Close();
            }
            else
                MessageBox.Show("用户名或密码错误！");
        }

    }
}
