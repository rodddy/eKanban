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
using System.Net.Sockets;
using System.IO;
using System.Windows.Threading;
using System.Threading;
using System.Data.SqlClient;

namespace eKanban
{

    public partial class MainWindow : Window
    {
        private Random rd = new Random();

        private List<TcpClient> client = new List<TcpClient>();

        private const int bufferSize = 20;
        private NetworkStream sendStream;
        private byte[] sendData = new byte[6] { 0xAA, 0, 0, 0, 0x0D, 0X0A };

        // return Index of Min
        private int IndexOfMin = 0;
        private int tempMin = 0;

        Thread thread;
        Thread th;
        Thread th_updateContact;

        Thread th_updateClassData;

        //public delegate void showData(string msg);

        private List<IpAndPort> ipAndportList = new List<IpAndPort>();
        private int count = 0;
        private int length = 20;
        public int m_interval = 1; // database insert interval

        //private int[] GYRTimePerMechine = new int[180];

        private int m_len = 10; // 产线条数
        private string[] Line_Sets = new string[10] { "SMT 09线", "SMT 10线", "SMT 11线", "SMT 12线", "SMT 13线", "SMT 14线", "SMT 15线", "SMT 16线", "SMT 17线", "SMT 22线" };
        private string[] Deivce_Sets = new string[6] { "PRINTER", "SPI", "MT 1", "MT 2", "MT 3", "AOI" };

        private bool[] bFlagArr = new bool[10] { false, false, false, false, false, false, false, false, false, false };//产线闪烁指示

        private byte[] channel_set = new byte[10] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 22 };

        private byte[,] DeviceStatus = new byte[10, 20];
        private byte[,] tempStatus = new byte[10, 20]; 

        private byte[,] DeviceOffFlag = new byte[10, 20];

        private byte[,] DeviceOffInitalFlag = new byte[10, 20];// cannot modify in software

        private DispatcherTimer timer_update = new DispatcherTimer();
        private DispatcherTimer timer_updateData = new DispatcherTimer();//写数据库
        private DispatcherTimer timer_updateClassData = new DispatcherTimer();//写班次数据

        private DateTime dt;
        private int totaltime = 1;
        private int totalsec = 1;
        private string classes = "";

        private bool bFlagRest = false;//判断是否在休息，休息则停止累加

        private string[] arrContact = new string[9];
        private int[,] arrAbnormal = new int[10, 12];
        private int threshold_contact1 = 900;
        private int threshold_contact2 = 1800;
        private int threshold_contact3 = 2700;

        private int[] arrSpan = new int[4] { 10800, 1800, 31200, 1200 };
        private string str_span = "";

        public struct IpAndPort
        {
            public string Ip;
            public int Port;
            public int LineStatus;
        }

        public void InitDeviceList()
        {
            dt = DateTime.Now;
            gettime(dt);
            if (tb_class.Text != classes)
            {

                tb_class.Text = classes;
                for (int i = 0; i < 190; i++)
                {
                    GYRTime[i] = 0;
                }
            }

            InitData();

            string InitialFile = "DeviceStatusConfig.txt";

            try
            {

                if (File.Exists(InitialFile))
                {
                    StreamReader sr = new StreamReader(InitialFile, Encoding.Default);
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] arr = line.Split(',');
                        int len = Math.Min(60, arr.Count());
                        for (int i = 0; i < len; i++)
                        {
                            DeviceOffInitalFlag[i / 6, 3 + i % 6] = (byte)int.Parse(arr[i]);
                            DeviceOffFlag[i / 6, 3 + i % 6] = (byte)int.Parse(arr[i]);

                        }
                    }
                    sr.Close();
                }

            }
            catch (Exception err) { Console.WriteLine(err.Message); }

            try
            {

                if (File.Exists(fileConfig))
                {
                    StreamReader sr = new StreamReader(fileConfig, Encoding.Default);
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] arr = line.Split(',');
                        int len = Math.Min(60, arr.Count());
                        for (int i = 0; i < len; i++)
                        {
                            DeviceOffFlag[i / 6, 3 + i % 6] = (byte)int.Parse(arr[i]);
                        }

                    }

                    sr.Close();
                }
                else
                {

                    string strConfig = "";
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 3; j < 9; j++)
                        {
                            strConfig += DeviceOffFlag[i, j] + ",";
                        }
                    }

                    FileStream fs = new FileStream(fileConfig, FileMode.Create, FileAccess.Write);//创建写入文件 
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(strConfig);//开始写入值
                    sw.Close();
                    fs.Close();
                }

            }
            catch (Exception err) { Console.WriteLine(err.Message); }

            string iplist = "iplist.txt";

            if (File.Exists(iplist))
            {
                StreamReader sr = new StreamReader(iplist, Encoding.Default);
                String line;
                IpAndPort ipAndport;
                ipAndportList = new List<IpAndPort>();
                while ((line = sr.ReadLine()) != null)
                {
                    string[] arr = line.Split(';');
                    ipAndport.Ip = arr[0];
                    ipAndport.Port = int.Parse(arr[1]);
                    ipAndport.LineStatus = int.Parse(arr[2]);
                    ipAndportList.Add(ipAndport);
                }
                sr.Close();

                int len = m_len;
                if (len > ipAndportList.Count())
                    len = ipAndportList.Count();
                //for (int i = 0; i < len; i++)
                //{
                //    for (int j = 0; j < length; j++)
                //    {
                //        DeviceStatus[i, j] = 0;
                //        tempStatus[i, j] = 0;
                //        DeviceOffFlag[i,j] = 0;
                //    }
                //}
                //设备未投入生产
                //DeviceOffFlag[0, 4] = 8; DeviceOffFlag[1, 3] = 8;
                //DeviceOffFlag[2, 3] = 8; DeviceOffFlag[3, 7] = 8;
                //DeviceOffFlag[4, 4] = 8; DeviceOffFlag[5, 4] = 8;
                //DeviceOffFlag[7, 4] = 8; DeviceOffFlag[8, 4] = 8;
                //DeviceOffFlag[9, 4] = 8; DeviceOffFlag[9, 7] = 8;

                //DeviceStatus[0, 4] = 8; DeviceStatus[3, 7] = 8;
                //DeviceStatus[4, 4] = 8; DeviceStatus[5, 4] = 8;
                //DeviceStatus[7, 4] = 8; DeviceStatus[8, 4] = 8;
                //DeviceStatus[9, 4] = 8; DeviceStatus[9, 7] = 8;

                //tempStatus[0, 4] = 8; tempStatus[3, 7] = 8;
                //tempStatus[4, 4] = 8; tempStatus[5, 4] = 8;
                //tempStatus[7, 4] = 8; tempStatus[8, 4] = 8;
                //tempStatus[9, 4] = 8; tempStatus[9, 7] = 8;

                arrContact = new string[9] { "demo", "demo", "demo", "demo", "demo", "demo", "demo", "demo", "demo" };

                if (len > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        try
                        {
                            //TcpClient c = new TcpClientWithTimeout(ipAndportList.ElementAt(i).Ip, ipAndportList.ElementAt(i).Port, 80).Connect();
                            TcpClient c = new TcpClient();
                            client.Add(c);
                            //sendStream.Add(c.GetStream());
                        }
                        catch (Exception err) {
                            Console.WriteLine(err.Message);
                        }
                    }

                    thread = new Thread(ListenerServer);
                    thread.Start();
                    thread.IsBackground = true;

                    timer_update.Interval = TimeSpan.FromMilliseconds(200);
                    timer_update.Tick += Timer_update_Tick;
                    timer_update.Start();

                    timer_updateData.Interval = TimeSpan.FromMilliseconds(1000);
                    timer_updateData.Tick += Timer_UpdateData_Tick;
                    timer_updateData.Start();

                    // 查询 更新上一班次数据
                    //timer_updateClassData.Interval= TimeSpan.FromMilliseconds(5400000);
                    timer_updateClassData.Interval = TimeSpan.FromMilliseconds(5000);
                    timer_updateClassData.Tick += Timer_updateClassData_Tick;
                    timer_updateClassData.Start();

                    th = new Thread(InsertDB);
                    th.IsBackground = true;
                    th.Start();

                    th_updateContact = new Thread(UpdateContact);
                    th_updateContact.IsBackground = true;
                    th_updateContact.Start();

                    th_updateClassData = new Thread(UpdateClassData);
                    th_updateClassData.IsBackground = true;

                }
            }

        }

        // 0 白班 1 2夜班 3 数据不在范围内
        private int gettime(DateTime dt)
        {
            string start_time = dt.ToString("yyyy-MM-dd") + " 08:00:00";
            DateTime start_dt = Convert.ToDateTime(start_time);
            string end_time = dt.ToString("yyyy-MM-dd") + " 20:00:00";
            DateTime end_dt = Convert.ToDateTime(end_time);
            double ts_start = dt.Subtract(start_dt).TotalSeconds;
            double ts_end = dt.Subtract(end_dt).TotalSeconds;


            // 在8点-20点之间 白班
            if (ts_start >= 0 && ts_end < 0)
            {
                classes = "白班";
                totaltime = (int)ts_start;
                return 0;
            }
            // 20点-24点之间 夜班
            else if (ts_end >= 0)
            {
                classes = "夜班";
                totaltime = (int)ts_end;
                return 1;
            }
            // 距离第二天早上8点 8小时以内
            else if (ts_start < 0 && ts_start > -28800) {
                totaltime = (int)(ts_end + 86400);
                classes = "夜班";
                return 2;
            }
            // 无效时间段
            else
                return 3;
            //if (tb_class.Text != classes) {

            //    tb_class.Text = classes;
            //    for (int i = 0; i < 30; i++) {
            //        GYRTime[i] = 0;
            //    }
            //}
               
        }
        //
        private void Timer_update_Tick(object sender, EventArgs e)
        {

            //if (ts.TotalMilliseconds > DelaySetting * 1000)
            //{

            try
            {
                string img_name = "";
                //for (int i = 0; i < ipAndportList.Count(); i++)
                //{
                //    for (int j = 0; j < length; j++)
                //    {
                //        DeviceStatus[i, j] = tempStatus[i, j];
                //    }
                //}

                for (int i = 0; i < m_len; i++)
                {
                    string str_sp = "st_" + (i + 1).ToString();
                    StackPanel st = FindName(str_sp) as StackPanel;
                    
                    if (bUpdateFlag[i] == 1)
                    {
                        
                        byte[] temp = new byte[21];

                        temp[0] = tempStatus[i, 0];
                        temp[1] = tempStatus[i, 1];
                        temp[2] = tempStatus[i, 2];
                        temp[3] = (byte)DelaySetting;
                        temp[20] = tempStatus[i, 19];
                        if (temp[0] == 170)
                        {
                            st.Dispatcher.BeginInvoke(new Action(() => { st.Background = Brushes.Transparent; }));
                            tb_status.Dispatcher.BeginInvoke(new Action(() => { tb_status.Text = "监控中"; }));
                            for (int j = 3; j < length - 7; j++)
                            {
                                temp[j + 1] = tempStatus[i, j];
                            }
                            //    try
                            //{
                            //    if (temp[0] == 170)
                            //    {

                            //        tcpClient = new TcpClientWithTimeout(ServerIP, ServerPort, 80).Connect();
                            //        sendToServerStream = tcpClient.GetStream();
                            //        if (sendToServerStream.CanWrite)
                            //            sendToServerStream.Write(temp, 0, length);
                            //    }

                            //}
                            //catch (Exception err) { Console.WriteLine(); }
                            for (int j = 4; j < 10; j++)
                            {
                                img_name = "Ellipse_" + (i + 1).ToString() + (j - 3).ToString();

                                Ellipse ell = FindName(img_name) as Ellipse;

                                // 0-灰；1-红；2，3-黄色；4-7绿色
                                switch (temp[j])
                                {
                                    case 8: ell.Dispatcher.Invoke(new Action(() => { ell.Fill = new SolidColorBrush(Colors.Transparent);})); break;
                                    case 4: ell.Dispatcher.Invoke(new Action(() => { ell.Fill = new SolidColorBrush(Colors.Green);})); break;
                                    case 3: ell.Dispatcher.Invoke(new Action(() => { ell.Fill = new SolidColorBrush(Colors.Yellow); })); break;
                                    case 2: ell.Dispatcher.Invoke(new Action(() => { ell.Fill = new SolidColorBrush(Colors.Yellow); })); break;
                                    case 1: ell.Dispatcher.Invoke(new Action(() => { ell.Fill = new SolidColorBrush(Colors.Red); })); break;
                                    case 0: ell.Dispatcher.Invoke(new Action(() => { ell.Fill = new SolidColorBrush(Colors.Gray); })); break;
                                    default: ell.Dispatcher.Invoke(new Action(() => { ell.Fill = new SolidColorBrush(Colors.Green); })); break;

                                }

                            }
                        }
                        else
                        {

                            tb_status.Dispatcher.BeginInvoke(new Action(() => { tb_status.Text = "监控异常"; }));
                            try
                            {
                                if (bFlagArr[i])
                                    st.Background = Brushes.Red;
                                else
                                    st.Background = Brushes.Transparent;
                                bFlagArr[i] = !bFlagArr[i];
                                //}));
                            }
                            catch (Exception err) { Console.WriteLine(err.Message); }
                        }

                    }

                    //for (int j = 0; j < 20; j++)
                    //{

                    //    tempStatus[i, j] = Math.Max((byte)0, DeviceOffFlag[i, j]); ;
                    //}
                }

            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        private void Timer_UpdateData_Tick(object sender, EventArgs e)
        {
            tb_clock.Dispatcher.BeginInvoke(new Action(() => { tb_clock.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }));
            TimeSpan ts = DateTime.Now - dt;
            int totalSecCheck = 1;
            gettime(dt);
            bFlagRest = false;
            if (classes == "夜班")
            {
                if (totaltime - arrSpan[0] > 0) {
                    if (totaltime - arrSpan[0] < arrSpan[1]) { 
                        totalsec = arrSpan[0];
                        bFlagRest = true;
                    }
                    else
                        totalsec = totaltime - arrSpan[1];
                }
                else {
                    totalsec = totaltime;
                }

                if (totaltime - arrSpan[2] > 0)
                {
                    if (totaltime - arrSpan[2] < arrSpan[3]) { 
                        totalsec = arrSpan[2] - arrSpan[1];
                        bFlagRest = true;
                    }
                    else
                        totalsec = totaltime - arrSpan[1] - arrSpan[3];
                }
                else
                {
                    totalsec = totaltime;
                }

            }
            if (tb_class.Text != classes)
            {

                tb_class.Text = classes;
                for (int i = 0; i < 190; i++)
                {
                    GYRTime[i] = 0;
                }
            }
            // 否 正常工作，是 休息
            if (!bFlagRest)
                m_interval = ts.Seconds;
            else
                m_interval = 0;

            for (int i = 0; i < 10; i++)
            {
                tempMin = 86400;
                for (int j = 3; j < 9; j++)
                {

                   // GYRTime[i * 19 + j - 3] = rd.Next(65,85)*totalsec/100;
                    if (bUpdateFlag[i] == 0)
                    {
                        arrAbnormal[i, j - 3] = 0;
                        arrAbnormal[i, j + 3] = 0;
                        GYRTime[i * 19 + j - 3] += m_interval;
                    }
                    else
                    {
                        switch (DeviceStatus[i, j])
                        {
                            case 8:
                                {
                                    arrAbnormal[i, j - 3] = 0;
                                    arrAbnormal[i, j + 3] = 0;
                                    GYRTime[i * 19 + j - 3] += m_interval;
                                    break;
                                }
                            case 4:
                                {
                                    GYRTime[i * 19 + j - 3] += m_interval;
                                    arrAbnormal[i, j - 3] = 0;
                                    arrAbnormal[i, j + 3] = 0;
                                    break;
                                }
                            case 2:
                                {
                                    GYRTime[i * 19 + j + 3] += m_interval;
                                    //arrAbnormal[i, j - 3] += m_interval;
                                    break;
                                }

                            case 1:
                                {
                                    GYRTime[i * 19 + j + 9] += m_interval;
                                    arrAbnormal[i, j - 3] += m_interval;
                                    break;
                                }
                            case 0:
                                {
                                    //arrAbnormal[i, j - 3] += m_interval;
                                    GYRTime[i * 19 + j + 3] += m_interval;
                                    break;
                                }

                            default: break;

                        }
                    }

                    if (arrAbnormal[i, j - 3] >= threshold_contact1 && arrAbnormal[i, j + 3] == 0)
                    {
                        arrAbnormal[i, j + 3] = 1;
                        try {
                           ssmsSend.sendMsg(arrContact[1],"五车间", Line_Sets[i] );
                            ssmsSend.sendEmail(arrContact[2], "设备异常，请工作人员检查 " + Line_Sets[i] + " 设备 " + Deivce_Sets[j - 3]);
                            
                        } catch (Exception err) { Console.WriteLine(err.Message); }
                        
                    }
                    else if (arrAbnormal[i, j - 3] >= threshold_contact2 && arrAbnormal[i, j + 3] == 1) {
                        arrAbnormal[i, j + 3] = 2;
                        try
                        {
                            ssmsSend.sendMsg(arrContact[4], "五车间", Line_Sets[i]);
                            ssmsSend.sendEmail(arrContact[5], "设备异常，请工作人员检查 " + Line_Sets[i] + " 设备 " + Deivce_Sets[j - 3]);

                        }
                        catch (Exception err) { Console.WriteLine(err.Message); }
                    }
                    else if (arrAbnormal[i, j - 3] >= threshold_contact3 && arrAbnormal[i, j + 3] == 2)
                    {
                        try
                        {
                            ssmsSend.sendMsg(arrContact[7], "五车间", Line_Sets[i]);
                            ssmsSend.sendEmail(arrContact[8], "设备异常，请工作人员检查 " + Line_Sets[i] + " 设备 " + Deivce_Sets[j - 3]);

                        }
                        catch (Exception err) { Console.WriteLine(err.Message); }
                        arrAbnormal[i, j + 3] = 0;
                        arrAbnormal[i, j - 3] = 0;
                    }

                    totalSecCheck = Math.Max(totalSecCheck, GYRTime[i * 19 + j - 3] + GYRTime[i * 19 + j + 3] + GYRTime[i * 19 + j + 9]);
                    if (GYRTime[i * 19 + j - 3] < tempMin && DeviceOffFlag[i, j] == 0)
                    {
                        IndexOfMin = j - 3;
                        tempMin = GYRTime[i * 19 + j - 3];

                    }

                }

                GYRTime[i * 19 + 18] = IndexOfMin;
                arrActivation[i] = (int)Math.Round(GYRTime[i * 19 + IndexOfMin] * 100f / totalsec);
                //arrActivation[i] = (int)Math.Round(GYRTime[i * 3] / (totalsec * 6.0f) * 100);
            }

            if (totalSecCheck > totalsec)
            {
                double rate = totalsec * 1.0f / totalSecCheck;
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        GYRTime[i * 19 + j] = (int)(GYRTime[i * 19 + j] * rate);
                        GYRTime[i * 19 + j + 6] = (int)(GYRTime[i * 19 + j + 6] * rate);
                        GYRTime[i * 19 + j + 12] = (int)(GYRTime[i * 19 + j + 12] * rate);
                       // GYRTime[i * 19 + j] = totalsec - GYRTime[i * 19 + j + 6] - GYRTime[i * 19 + j + 6];
                    }
                }
            }
            else if (totalSecCheck < totalsec -60 && totalSecCheck < totalsec * 0.95)
            {
                //totalSecCheck < totalsec && totalSecCheck > totalsec - 60;
                double rate = totalsec * 1.0f / totalSecCheck;
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        GYRTime[i * 19 + j] = (int)(GYRTime[i * 19 + j] * rate);
                        GYRTime[i * 19 + j + 6] = (int)(GYRTime[i * 19 + j + 6] * rate);
                        GYRTime[i * 19 + j + 12] = (int)(GYRTime[i * 19 + j + 12] * rate);
                        //GYRTime[i * 19 + j + 6] = (totalsec - GYRTime[i * 19 + j] - GYRTime[i * 19 + j + 12]);

                    }
                }
            }
            else
            {
                //for (int i = 0; i < 10; i++)
                //{
                //    for (int j = 0; j < 6; j++)
                //    {
                //        GYRTime[i * 19 + j + 6] = (totalsec - GYRTime[i * 19 + j] - GYRTime[i * 19 + j + 12]);
                        
                //    }
                //}
            }

            dt = DateTime.Now;

            try
            {
                if (!th.IsAlive)
                    th.Start();
                if (!thread.IsAlive)
                    thread.Start();
            }
            catch (Exception err) { Console.WriteLine(err.Message); }

        }


        private void Timer_updateClassData_Tick(object sender, EventArgs e)
        {
            try
            {
                th_updateClassData = new Thread(UpdateClassData);
                th_updateClassData.Start();
                th_updateClassData.IsBackground = true;
            }
            catch (Exception err) {
                Console.WriteLine(err.Message);
            }
        }

        private void InsertDB()
        {
            string str_device_status_count = "";
            string str_device_activation = "";
            string str_device_gyrcount = "";
            string command = "";
            while (true) {
                try
                {
                    str_device_status_count = "";
                    str_device_activation = "";
                    str_device_gyrcount = "";

                    for (int i = 0; i < 10; i++)
                    {
                        if (bUpdateFlag[i] == 0)
                            str_device_status_count += "8,8,8,8,8,8,";
                        else
                            str_device_status_count += DeviceStatus[i, 3] + "," + DeviceStatus[i, 4] + "," + DeviceStatus[i, 5] + "," + DeviceStatus[i, 6] + "," + DeviceStatus[i, 7] + "," + DeviceStatus[i, 8] + ",";

                    }
                    //for (int i = 0; i < 190; i++) {
                    //    str_device_gyrcount += (86400).ToString() + ",";
                    //}
                    str_device_activation = string.Join(",", arrActivation);
                    str_device_gyrcount = string.Join(",", GYRTime);
                    //str_device_gyrcount = "60000,3600,1200,50000,7200,7600,62000,1600,1200,62000,1600,1200,62000,1600,1200,62000,1600,1200,62000,1600,1200,62000,1600,1200,62000,1600,1200,62000,1600,1200";
                    command = string.Format("insert into device_statusCount (Department, DeviceStatus, Activation, GYRCount, UpdateTime, Flag) "
                         + " values ({0}, '{1}', '{2}', '{3}', '{4}',{5})"
                        , Department, str_device_status_count, str_device_activation, str_device_gyrcount, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), m_flag);
                    sqlHelper.ExecuteNonQuery(command);

                    for (int i = 0; i < 10; i++)
                    {
                        DeviceStatus[i, 3] = Math.Max(tempStatus[i, 3], DeviceOffFlag[i, 3]);
                        DeviceStatus[i, 4] = Math.Max(tempStatus[i, 4], DeviceOffFlag[i, 4]);
                        DeviceStatus[i, 5] = Math.Max(tempStatus[i, 5], DeviceOffFlag[i, 5]);
                        DeviceStatus[i, 6] = Math.Max(tempStatus[i, 6], DeviceOffFlag[i, 6]);
                        DeviceStatus[i, 7] = Math.Max(tempStatus[i, 7], DeviceOffFlag[i, 7]);
                        DeviceStatus[i, 8] = Math.Max(tempStatus[i, 8], DeviceOffFlag[i, 8]);
                    }
                    tb_status.Dispatcher.BeginInvoke(new Action(() => { tb_status.Text = ""; }));

                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    tb_status.Dispatcher.BeginInvoke(new Action(() => { tb_status.Text += ", 数据库连接异常！"; }));
                }
                Thread.Sleep(1000);
            }
        }

        public void UpdateClassData()
        {
            // 查询上一班次数据是否存在

            string str_class = "";
            string str_class_end = "";
            int m_classes = 0;
            if (classes == "白天") { 
                str_class = dt.AddDays(-1).ToString("yyyy-MM-dd") + " 20:00:00";
                str_class_end = Convert.ToDateTime(str_class).AddHours(12).ToString("yyyy-MM-dd HH:mm:ss");
                m_classes = 1;
            }
            else {
                str_class = dt.AddHours(-12).ToString("yyyy-MM-dd") + " 08:00:00";
                str_class_end = Convert.ToDateTime(str_class).AddHours(12).ToString("yyyy-MM-dd HH:mm:ss");
                m_classes = 0;
            }

            try
            {
                string cmd = string.Format("select top 1* from device_statusClassCount where UpdateTime = '{0}' and Department = {1}", str_class, Department);
                SqlDataReader sr = sqlHelper.ExecuteReader(cmd);
                if (!sr.Read())
                {
                    cmd = string.Format("select top 1* from device_statusCount where UpdateTime > '{0}' and UpdateTime < '{1}' order by UpdateTime desc",
                    str_class, str_class_end);
                    sr = sqlHelper.ExecuteReader(cmd);

                    if (sr.Read())
                    {
                        string arr_str = sr["Activation"].ToString();
                        string gyr_str = sr["GYRCount"].ToString();

                        cmd = string.Format("insert into device_statusClassCount (Department,Activation,GYRCount,UpdateTime,Classes)"
                            + " values({0},'{1}','{2}','{3}',{4})", Department, arr_str, gyr_str, str_class, m_classes);
                        sr = sqlHelper.ExecuteReader(cmd);

                    }
                }

            }
            catch (Exception err) { Console.WriteLine(err.Message); }
        }

        private string[] arrStrSpan;

        public void UpdateContact()
        {
            string command = "";
            while (true)
            {
                command = "select top 1* from ContactInfo";
                try
                {
                    //ls_contact = new List<ContactInfo>();
                    //ContactInfo cInfo = new ContactInfo();
                    SqlDataReader sr = sqlHelper.ExecuteReader(command);
                    if (sr.Read())
                    {

                        arrContact[0] = sr["Phone4"].ToString();
                        arrContact[1] = sr["Phone1"].ToString();
                        arrContact[2] = sr["Mail1"].ToString();

                        arrContact[3] = sr["Phone5"].ToString();
                        arrContact[4] = sr["Phone2"].ToString();
                        arrContact[5] = sr["Mail2"].ToString();

                        arrContact[6] = sr["Phone6"].ToString();
                        arrContact[7] = sr["Phone3"].ToString();
                        arrContact[8] = sr["Mail3"].ToString();

                        threshold_contact1 = int.Parse(sr["Mail4"].ToString()) * 60;
                        threshold_contact2 = int.Parse(sr["Mail5"].ToString()) * 60;
                        threshold_contact3 = int.Parse(sr["Mail6"].ToString()) * 60;

                        m_flag = int.Parse(sr["Flag"].ToString());

                        str_span = sr["Settings1"].ToString();
                        arrStrSpan = str_span.Split(',');
                        arrSpan[0] = int.Parse(arrStrSpan[0]);
                        arrSpan[1] = int.Parse(arrStrSpan[1]);
                        arrSpan[2] = int.Parse(arrStrSpan[2]);
                        arrSpan[3] = int.Parse(arrStrSpan[3]);

                        if (arrSpan[0] <= 0)
                            arrSpan[0] = 10800;
                        if (arrSpan[1] <= 0)
                            arrSpan[1] = 1800;
                        if (arrSpan[2] <= 0)
                            arrSpan[2] = 31200;
                        if (arrSpan[3] <= 0)
                            arrSpan[3] = 1200;

                    }
                }
                catch (Exception err) { Console.WriteLine(err.Message); }
                Thread.Sleep(2000);
            }
        }
        public void ListenerServer()
        {
            do
            {
                //if (ipAndportList.ElementAt(count).LineStatus == 1)
                // {
                try
                {
 
                    client[count] = new TcpClientWithTimeout(ipAndportList.ElementAt(count).Ip, ipAndportList.ElementAt(count).Port, 100).Connect();
                    client[count].ReceiveTimeout = 40;
                    
                    //获取用于发送数据的传输流
                    sendStream = client[count].GetStream();//count += 1;

                    int readSize;
                    byte[] buffer = new byte[bufferSize];
                    //string str = "";
                    lock (sendStream)
                    {
                        readSize = sendStream.Read(buffer, 0, bufferSize);
                        Console.WriteLine(count.ToString());

                        if (readSize >= 20 && buffer[0] == 170)
                        {
                            // 0 头，1，车间，2，生产线
                            int index =Math.Max(  Array.IndexOf(channel_set,buffer[2]),0);
                            int head = buffer[2] - 1;
                            tempStatus[index, 0] = buffer[0];
                            tempStatus[index, 1] = buffer[1];
                            tempStatus[index, 2] = buffer[2];

                            DeviceStatus[index, 0] = buffer[0];
                            DeviceStatus[index, 1] = buffer[1];
                            DeviceStatus[index, 2] = buffer[2];

                            for (int i = 3; i < length - 1; i++)
                            {
                                if (Math.Floor(buffer[i] / 4.0f) == 1)
                                    tempStatus[index, i] = 4;
                                else if (Math.Floor(buffer[i] / 2.0f) == 1)
                                    tempStatus[index, i] = 2;
                                else
                                    tempStatus[index, i] = buffer[i];
                                tempStatus[index, i] = Math.Max(tempStatus[index, i], DeviceOffFlag[index, i]);//设备关闭状态数据重置
                                DeviceStatus[index, i] = Math.Max(tempStatus[index, i], DeviceStatus[index, i]);//去除闪烁

                            }
                        }

                        sendData[1] = buffer[1];
                        sendData[2] = buffer[2];
                        sendData[3] = (byte)count;

                        sendStream.Write(sendData, 0, 6);
                        sendStream.Close();
                    }
                    client[count].Close();
                    //if (readSize == 0)
                    //    return;

                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);

                }
                //}

                if (count >= ipAndportList.Count() - 1)
                {
                    count = 0;
                }
                else
                    count += 1;
            } while (true);
        }
    }
}
