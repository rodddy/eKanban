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
using System.Threading;

using System.Data.SqlClient;

namespace eKanban_Console
{
    /// <summary>
    /// MaintainLog_Win.xaml 的交互逻辑
    /// </summary>
    public partial class MaintainLog_Win : Window
    {
        private List<MaintainInfo> ls_logInfo = new List<MaintainInfo>();
        private DateTime dt;
        private string str_start;
        private string str_end;
        private int currentDepartment = 5;
        private int currentLine = 0;
        private string strLine = "SMT 09线";
        private Thread thread;

        public MaintainLog_Win()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt = DateTime.Now;
            str_start = dt.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss");
            str_end = dt.ToString("yyyy-MM-dd HH:mm:ss");

            dt_start.setDateTime(dt.AddDays(-1));
            dt_end.setDateTime(dt);
            thread = new Thread(InitData);
            thread.Start();
            thread.IsBackground = true;
        }

        private void InitData()
        {
            string command = string.Format("select * from MaintainLog where MaintainTime >= '{0}' and MaintainTime <= '{1}' and Department={2} and Line = {3}", str_start, str_end, currentDepartment, currentLine);
            SqlDataReader sr = sqlHelper.ExecuteReader(command);
            MaintainInfo info = new MaintainInfo();
            ls_logInfo = new List<MaintainInfo>();
            while (sr.Read())
            {
                try
                {
                    info = new MaintainInfo();
                    info.Department = int.Parse(sr["Department"].ToString());
                    info.MaintainLog = sr["MaintainLog"].ToString();
                    info.MaintainTime = sr["MaintainTime"].ToString();
                    info.MaintainUser = sr["MaintainUser"].ToString();
                    info.MaintainType = sr["Type"].ToString();
                    info.Period = int.Parse(sr["Period"].ToString());

                    ls_logInfo.Add(info);

                }
                catch (Exception err) { Console.WriteLine(err.Message); }
            }
            if (ls_logInfo.Count() > 0)
                dataLog.Dispatcher.BeginInvoke(new Action(() => { dataLog.ItemsSource = ls_logInfo; }));
        }

        private void btn_check_Click(object sender, RoutedEventArgs e)
        {
            //dt_end.setDateTime(DateTime.Now);
            str_start = dt_start.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            str_end = dt_end.DateTime.ToString("yyyy-MM-dd HH:mm:ss");

            thread = new Thread(InitData);
            thread.Start();
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {

            try {
                string str_dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string strlog = "";
                string user = "";
                string type = "";
                int period = 0;

                if (!string.IsNullOrEmpty(tb_type.Text))
                    type = tb_type.Text;
                if(!string.IsNullOrEmpty(tb_log.Text))
                    strlog = tb_log.Text;
                if (!string.IsNullOrEmpty(tb_user.Text))
                    user = tb_user.Text;
                if (!string.IsNullOrEmpty(tb_min.Text))
                    period = int.Parse(tb_min.Text);
                string command = string.Format("insert into MaintainLog"
                    +"(Department,Line,MaintainLog,MaintainTime,MaintainUser,Type,Period)"
                    +" values({0},{1},'{2}','{3}','{4}','{5}',{6})",
                    currentDepartment, currentLine, strlog,str_dt, user, type, period);
                sqlHelper.ExecuteNonQuery(command);

            }
            catch (Exception err) { Console.WriteLine(err.Message); }
            str_end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            thread = new Thread(InitData);
            thread.Start();
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (dataLog.SelectedIndex >= 0) {
                try {

                    MaintainInfo info = dataLog.SelectedItem as MaintainInfo;

                    string command = string.Format("delete from MaintainLog where MaintainTime='{0}'", info.MaintainTime);
                    sqlHelper.ExecuteNonQuery(command);

                    str_end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    thread = new Thread(InitData);
                    thread.Start();

                }
                catch (Exception err) { Console.WriteLine(err.Message); }
            }
        }

        private void btn_modify_Click(object sender, RoutedEventArgs e)
        {
            if (dataLog.SelectedIndex >= 0)
            {
                try
                {
                    MaintainInfo info = dataLog.SelectedItem as MaintainInfo;

                    if (!string.IsNullOrEmpty(tb_type.Text))
                        info.MaintainType = tb_type.Text;
                    if (!string.IsNullOrEmpty(tb_log.Text))
                        info.MaintainLog = tb_log.Text;
                    if (!string.IsNullOrEmpty(tb_user.Text))
                        info.MaintainUser = tb_user.Text;
                    if (!string.IsNullOrEmpty(tb_min.Text))
                        info.Period = int.Parse(tb_min.Text);

                    string command = string.Format("update MaintainLog set Department = {0}, Line = {1},"
                    + "MaintainLog = '{2}',MaintainUser = '{3}',Type = '{4}', Period = {5} where MaintainTime='{6}'",
                    currentDepartment, currentLine, info.MaintainLog, info.MaintainUser, info.MaintainType, info.Period, info.MaintainTime);
                    sqlHelper.ExecuteNonQuery(command);

                    str_end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    thread = new Thread(InitData);
                    thread.Start();
                }
                catch (Exception err) { Console.WriteLine(err.Message); }
            }
        }
        private void dataLog_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dataLog.SelectedIndex >= 0)
            {
                MaintainInfo info = dataLog.SelectedItem as MaintainInfo;

                try
                {
                    tb_type.Text = info.MaintainType;
                    tb_log.Text = info.MaintainLog;
                    tb_user.Text = info.MaintainUser;
                    tb_min.Text = info.Period.ToString();
                }
                catch (Exception err) { Console.WriteLine(err.Message); }
            }
        }

        private void img_close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (thread.IsAlive)
                thread.Abort();
            this.Close();
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }

        }

        public void setParams(int Department, int Line) {
            this.currentDepartment = Department;
            this.currentLine = Line;

            switch (Department)
            {
                case 5: strLine = DeviceInfo.Department_5.ElementAt(Line).Name; break;
                case 6: strLine = DeviceInfo.Department_6.ElementAt(Line).Name; break;
                case 7: strLine = DeviceInfo.Department_7.ElementAt(Line).Name; break;
                case 8: strLine = DeviceInfo.Department_8.ElementAt(Line).Name; break;
                default: strLine = DeviceInfo.Department_5.ElementAt(Line).Name;break;
            }
            tb_line.Text = strLine + "维修日志";
        }

    }
}
