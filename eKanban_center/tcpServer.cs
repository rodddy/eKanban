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
using System.Net;
using System.Net.Sockets;
using SimpleTCP;

namespace eKanban_center
{
    public partial class MainWindow_Center : Window
    {

        public delegate void showData(string msg);//委托,防止跨线程的访问控件，引起的安全异常
        private SimpleTcpServer tcpServer;
        private SimpleTcpClient tcpClient;

        public void InitTcpServer()
        {
            tcpServer = new SimpleTcpServer();

            if (!tcpServer.IsStarted)
                tcpServer.Start(5000);

            tcpServer.Delimiter = 0x13;
            tcpServer.DelimiterDataReceived += Server_DelimiterDataReceived;
            tcpServer.ClientConnected += Server_ClientConnected;
            tcpServer.ClientDisconnected += Server_ClientDisconnected;
            tcpServer.DataReceived += TcpServer_DataReceived;

        }

        public void TcpServer_DataReceived(object sender, Message msg)
        {
            byte[] data = msg.Data;
            if (data.Length >= 19)
            {
                int head = data[2];
                string str=""; Ellipse ell;
                string command = "";

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //try
                    //{
                    command = string.Format("insert into tb_device_status (Department, Line, Interval, Device1Status,"
                     + "Device2Status, Device3Status, Device4Status, Device5Status, Device6Status, Device7Status,"
                     + "Device8Status, Device9Status, Device10Status, Device11Status, Device12Status, Device13Status, Device14Status, Device15Status, UpdateTime) "
                     + " values ('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9},{10},{11},{12},{13},{14},{15},{16}, '{17}')"
                    , data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    //command = string.Format("insert into test_table(sampleTime) values ('{0}')", DateTime.Now.ToString());
                    sqlHelper.ExecuteNonQuery(command);

                    //}
                    //catch (Exception err) { Console.WriteLine(err.Message); };
                }));

                this.Dispatcher.BeginInvoke(new Action(() =>
                {

                    try
                    {
                        for (int i = 3; i < 13; i++)
                        {
                            str = "Ellip_" + head.ToString() + (i - 2).ToString();
                            ell = FindName(str) as Ellipse;
                            switch (data[i])
                            {
                                case 8: ell.Fill = new SolidColorBrush(Colors.Transparent); break;
                                case 4: ell.Fill = new SolidColorBrush(Colors.Green); break;
                                case 3: ell.Fill = new SolidColorBrush(Colors.Yellow); break;
                                case 2: ell.Fill = new SolidColorBrush(Colors.Yellow); break;
                                case 1: ell.Fill = new SolidColorBrush(Colors.Red); break;
                                case 0: ell.Fill = new SolidColorBrush(Colors.Gray); break;
                                default: ell.Fill = new SolidColorBrush(Colors.Green); break;

                            }

                        }
                    }
                    catch (Exception err) { Console.WriteLine(err.Message); }
                }));

            }
        }

        private void Server_ClientDisconnected(object sender, TcpClient client)
        {
           // txt_message_show.Dispatcher.Invoke(new showData(txt_message_show.AppendText), client.ToString() + "客户端连接断开！");//AcceptTcpClient 是同步方法，会阻塞进程，得到连接对象后才会执行这一步  

            //throw new NotImplementedException();
        }

        private void Server_ClientConnected(object sender, TcpClient client)
        {
           // txt_message_show.Dispatcher.Invoke(new showData(txt_message_show.AppendText), client.ToString() + "客户端请求连接，连接建立！");//AcceptTcpClient 是同步方法，会阻塞进程，得到连接对象后才会执行这一步  

            //throw new NotImplementedException();
        }

        private void Server_DelimiterDataReceived(object sender, Message msg)
        {
            //msg.ReplyLine("You said: " + msg.MessageString);
            //txt_message_show.Dispatcher.Invoke(new showData(txt_message_show.AppendText), msg.MessageString);//AcceptTcpClient 是同步方法，会阻塞进程，得到连接对象后才会执行这一步  

            //throw new NotImplementedException();
        }
    }
}
