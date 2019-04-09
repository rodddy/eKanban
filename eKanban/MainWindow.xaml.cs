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
using System.Configuration;

using System.Data.SqlClient;
using System.IO;

namespace eKanban
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow frm = null;
        private int DelaySetting = 5; //默认时间5s

        private string ServerIP = "172.20.145.1";
        private int ServerPort = 5000;

        private int[] bUpdateFlag = new int[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        private int m_flag = 0;

        private int[] arrActivation = new int[10] { 50, 50, 50, 50, 50, 60, 60, 60, 60, 60 };
        private int[] GYRTime = new int[190];
        private int Department = 5;

        private string fileConfig = "DeviceStatusLog.txt";

        public MainWindow()
        {
            InitializeComponent();
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            frm = this;
            InitView();
            InitDeviceList();
            //InitData();

            //this.DataContext = new TestApplication.Shared.TestPageViewModel();
            
            //Console.WriteLine(DataContext.ToString());
        }

        public void InitView()
        {
            try
            {
                DelaySetting = int.Parse(ConfigurationManager.AppSettings["DelaySetting"].ToString());
                Console.WriteLine(DelaySetting.ToString());

                ServerIP = ConfigurationManager.AppSettings["ServerIP"].ToString();
                ServerPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"].ToString());

            }
            catch (Exception err) { MessageBox.Show("没有相应配置文件！"); }
        }

        private void InitData() {
            dt = DateTime.Now;
            string start_time = dt.ToString("yyyy-MM-dd") + " 08:00:00";
            DateTime start_dt = Convert.ToDateTime(start_time);
            string end_time = dt.ToString("yyyy-MM-dd") + " 20:00:00";
            DateTime end_dt = Convert.ToDateTime(end_time);

            if (DateTime.Compare(start_dt, dt) < 0 && DateTime.Compare(end_dt, dt) > 0)
                ;
            else
            {
                start_time = end_time;
                end_time = dt.AddDays(1).ToString("yyyy-MM-dd") + " 08:00:00";
            }
            string temp_sr = "";
            string command = "select top 1* from device_statusCount order by UpdateTime desc";
            SqlDataReader sr =  sqlHelper.ExecuteReader(command);
            while (sr.Read()) {
                try
                {

                    temp_sr = sr["UpdateTime"].ToString();

                    int flag = gettime(Convert.ToDateTime(temp_sr));
                    if (flag != 3)
                    {

                        temp_sr = sr["GYRCount"].ToString();
                        string[] arr = temp_sr.Split(',');
                        for (int i = 0; i < 190; i++)
                        {
                            GYRTime[i] = int.Parse(arr[i]);
                        }
                    }
                }
                catch (Exception err) { Console.WriteLine(err.Message); }

            }

        }

        public void setDelay(int delay)
        {
            DelaySetting = delay;
        }

        private void img_settings_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Settings frm = new Settings();
            frm.Show();
        }

        private void img_close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (thread.IsAlive)
                    thread.Abort();
                if (th.IsAlive)
                    th.Abort();
                if (timer_update.IsEnabled)
                    timer_update.Stop();
                if (timer_updateData.IsEnabled)
                    timer_updateData.Stop();
                base.Close();
            }
            catch (Exception err) { Console.WriteLine(err.Message); }
            this.Close();
        }

        private void st_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StackPanel st = sender as StackPanel;
            try
            {
                string[] arr = st.Name.Split('_');
                int channel = int.Parse(arr[1]);
                string img = "img_" + channel.ToString();
                Image image = FindName(img) as Image;
                if (bUpdateFlag[channel - 1] == 1)
                {
                    bUpdateFlag[channel - 1] = 0;
                    image.Source = new BitmapImage(new Uri("Images/forbidden.png", UriKind.RelativeOrAbsolute));

                    st.Background = Brushes.Transparent;

                   for(int i = 0; i < 10; i++)
                    {
                        //DeviceStatus[channel - 1, i + 3] = Math.Max((byte)0, DeviceOffFlag[channel - 1, i + 3]);
                        string str = "Ellipse_" + channel.ToString() + (i + 1).ToString();
                        Ellipse ell = FindName(str) as Ellipse;
                        if (DeviceOffFlag[channel - 1, i+3] == 0)
                            ell.Fill = new SolidColorBrush(Colors.Gray);
                        else
                            ell.Fill = new SolidColorBrush(Colors.Transparent);
                    }

                }
                else
                {
                    bUpdateFlag[channel - 1] = 1;
                    image.Source = new BitmapImage(new Uri("Images/running.png", UriKind.RelativeOrAbsolute));

                }
            }
            catch(Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        private void chk_flag_Checked(object sender, RoutedEventArgs e)
        {
            if (m_flag == 1)
                m_flag = 0;
            else
                m_flag = 1;
        }

        private void img_minimize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse ell = sender as Ellipse;
            int Line = 0;
            int IndexOfDevice = 0;

            try
            {
                
                string[] arr_name = ell.Name.Split('_');
                if ((arr_name[1]).Length == 2)
                {
                    Line = int.Parse(arr_name[1].Substring(0, 1)) - 1;
                    IndexOfDevice = int.Parse(arr_name[1].Substring(1, 1)) + 2;
                    if (DeviceOffInitalFlag[Line, IndexOfDevice] == 0 && bUpdateFlag[Line] == 1)
                    {
                        if (DeviceOffFlag[Line, IndexOfDevice] == 0)
                            DeviceOffFlag[Line, IndexOfDevice] = 8;
                        else
                            DeviceOffFlag[Line, IndexOfDevice] = 0;
                    }
                    Console.WriteLine(Line.ToString() + ";" + IndexOfDevice.ToString() + ";");
                }
                else if ((arr_name[1]).Length == 3)
                {
                    Line = int.Parse(arr_name[1].Substring(0, 2)) - 1;
                    IndexOfDevice = int.Parse(arr_name[1].Substring(2, 1)) + 2;

                    if (DeviceOffInitalFlag[Line, IndexOfDevice] == 0 && bUpdateFlag[Line] == 1)
                    {
                        if (DeviceOffFlag[Line, IndexOfDevice] == 0)
                            DeviceOffFlag[Line, IndexOfDevice] = 8;
                        else
                            DeviceOffFlag[Line, IndexOfDevice] = 0;
                    }

                    Console.WriteLine(Line.ToString() + ";" + IndexOfDevice.ToString() + ";");
                }
                else
                {
                    Console.WriteLine("格式错误");
                }
                Console.WriteLine(Line.ToString() + ";" + IndexOfDevice.ToString() + ";");


                string strConfig = "";
                for (int i = 0; i < 10; i++) {
                    for (int j = 3; j < 9; j++) {
                        strConfig += DeviceOffFlag[i, j] + ",";
                    }
                }

                if (!File.Exists(fileConfig))
                {
                    FileStream fs = new FileStream(fileConfig, FileMode.Create, FileAccess.Write);//创建写入文件 
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(strConfig);//开始写入值
                    sw.Close();
                    fs.Close();
                }

                else
                {
                    FileStream fs = new FileStream(fileConfig, FileMode.Open, FileAccess.Write);
                    StreamWriter sr = new StreamWriter(fs);
                    sr.WriteLine(strConfig);//开始写入值
                    sr.Close();
                    fs.Close();
                }

            }
            catch (Exception err) { Console.WriteLine(err.Message); }


        }
    }
}
