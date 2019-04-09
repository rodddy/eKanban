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
using System.Collections.ObjectModel;

using System.Windows.Threading;
using System.Threading;

using System.Data.SqlClient;

namespace eKanban_Console
{
    /// <summary>
    /// Setting_Win.xaml 的交互逻辑
    /// </summary>
    public partial class Setting_Win_V2 : Window
    {

        private Thread thread;
        private int currentDepartment = 5;

        private string Name1, Name2, Name3, Phone1, Phone2, Phone3;
        private string Mail1, Mail2, Mail3;
        private string Settings1;

        private ObservableCollection<ContactInfo> ls_info = new ObservableCollection<ContactInfo>();

        private bool bflag = false;
        private int ModelSet = 0;
        private int Min1 = 15;
        private int Min2 = 30;
        private int Min3 = 45;
        private int index = -1;

        private int totalsec1 = 0;
        private int totalsec2 = 0;
        private int spansec1 = 0;
        private int spansec2 = 0;

        private int totalsec = 0;
        private DateTime dt_now;

        private string str_start1 = "";
        private string str_start2 = "";
        private string str_end1 = "";
        private string str_end2 = "";

        public Setting_Win_V2()
        {
            
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitView();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Content.ToString() == "正常")
                ModelSet = 0;
            else
                ModelSet = 1;
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {

            try {
                Min1 = int.Parse(tb_time_1.Text);
                if (Min1 < 0 || Min1 > 900)
                    Min1 = 15;
            }
            catch (Exception err) { Console.WriteLine(err.Message); }

            try
            {
                Min2 = int.Parse(tb_time_2.Text);
                if (Min2 < Min1 || Min2 > 915)
                    Min2 = Min1 + 15;
            } catch (Exception err) {

                Min2 = Min1 + 15;
                Console.WriteLine(err.Message);
            }

            try
                {
                    Min3 = int.Parse(tb_time_3.Text);
                if (Min3 < Min2 || Min3 > 999)
                    Min3 = Min2 + 15;
            } catch (Exception err) {

                Min3 = Min2 + 15;
                Console.WriteLine(err.Message);
            }

            tb_time_1.Text = Min1.ToString();
            tb_time_2.Text = Min2.ToString();
            tb_time_3.Text = Min3.ToString();

            UpdataDB(bflag);
            //thread.Start();
            this.Close();
        }

        private void btn_setting_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Min1 = int.Parse(tb_time_1.Text);
                if (Min1 < 0 || Min1 > 900)
                    Min1 = 15;
            }
            catch (Exception err) { Console.WriteLine(err.Message); }

            try
            {
                Min2 = int.Parse(tb_time_2.Text);
                if (Min2 < Min1 || Min2 > 915)
                    Min2 = Min1 + 15;
            }
            catch (Exception err)
            {

                Min2 = Min1 + 15;
                Console.WriteLine(err.Message);
            }

            try
            {
                Min3 = int.Parse(tb_time_3.Text);
                if (Min3 < Min2 || Min3 > 999)
                    Min3 = Min2 + 15;
            }
            catch (Exception err)
            {

                Min3 = Min2 + 15;
                Console.WriteLine(err.Message);
            }
            try
            {
                tb_time_1.Text = Min1.ToString();
                tb_time_2.Text = Min2.ToString();
                tb_time_3.Text = Min3.ToString();
                for (int i = 0; i < ls_info.Count(); i++)
                {
                    switch (ls_info.ElementAt(i).ContactType)
                    {

                        case "0": ls_info.ElementAt(i).ContactMin = Min1.ToString(); break;
                        case "1": ls_info.ElementAt(i).ContactMin = Min2.ToString(); break;
                        case "2": ls_info.ElementAt(i).ContactMin = Min3.ToString(); break;
                        default: ls_info.ElementAt(i).ContactMin = Min1.ToString(); break;
                    }
                }
                UpdataDB(bflag);
            }
            catch (Exception err) { Console.WriteLine(err.Message); }
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            ContactInfo info = new ContactInfo();
            try
            {
                info.ContactName = tb_name.Text;
                info.ContactPhone = tb_phone.Text;
                info.ContactMail = tb_mail.Text;
                switch (cmb_type.SelectedIndex)
                {
                    case 0:
                        info.ContactType = "0"; info.ContactMin = Min1.ToString(); break;
                    case 1:
                        info.ContactType = "1"; info.ContactMin = Min2.ToString(); break;
                    case 2:
                        info.ContactType = "2"; info.ContactMin = Min3.ToString(); break;
                    default:
                        info.ContactType = "0"; info.ContactMin = Min1.ToString(); break;
                }
                ls_info.Add(info);
                UpdataDB(bflag);
            }
            catch (Exception err) { Console.WriteLine(err.Message); }

        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ls_info.RemoveAt(index);
                UpdataDB(bflag);
            } catch (Exception err) { Console.WriteLine(err.Message); }
            
        }

        private void btn_modify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ls_info.ElementAt(index).ContactName = tb_name.Text;
                ls_info.ElementAt(index).ContactPhone = tb_phone.Text;
                ls_info.ElementAt(index).ContactMail = tb_mail.Text;
                switch (cmb_type.SelectedIndex)
                {
                    case 0:
                        ls_info.ElementAt(index).ContactType = "0"; ls_info.ElementAt(index).ContactMin = Min1.ToString(); break;
                    case 1:
                        ls_info.ElementAt(index).ContactType = "1"; ls_info.ElementAt(index).ContactMin = Min2.ToString(); break;
                    case 2:
                        ls_info.ElementAt(index).ContactType = "2"; ls_info.ElementAt(index).ContactMin = Min3.ToString(); break;
                    default:
                        ls_info.ElementAt(index).ContactType = "0"; ls_info.ElementAt(index).ContactMin = Min1.ToString(); break;
                }

                UpdataDB(bflag);

            }
            catch (Exception err) { Console.WriteLine(err.Message); }
        }

        private void cmb_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmb_type.SelectedIndex) {
                case 0:
                    tb_time.Text = Min1.ToString();
                    break;
                case 1:
                    tb_time.Text = Min2.ToString(); break;
                case 2:
                    tb_time.Text = Min3.ToString(); break;
                default:
                    tb_time.Text = Min1.ToString(); break;
            }
        }

        private void btn_sleep_Click(object sender, RoutedEventArgs e)
        {
            InitDate();
            UpdataDB(bflag);

        }

        private void dataSettings_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dataSettings.SelectedIndex >= 0)
            {
                
                ContactInfo info = dataSettings.SelectedItem as ContactInfo;

                try
                {
                    tb_name.Text = info.ContactName;
                    tb_phone.Text = info.ContactPhone;
                    tb_mail.Text = info.ContactMail;

                    tb_time.Text = info.ContactMin.ToString();
                    cmb_type.SelectedIndex = int.Parse(info.ContactType);

                    index = dataSettings.SelectedIndex;
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

        private void InitView()
        {

            dt_now = DateTime.Now;

            int classes = gettime(DateTime.Now);
            if (classes == 0)
            {
                str_start1 = dt_now.ToString("yyyy-MM-dd") + " 23:00:00";
                str_end1 = dt_now.ToString("yyyy-MM-dd") + " 23:30:00";

                str_start2 = dt_now.AddDays(1).ToString("yyyy-MM-dd") + " 04:40:00";
                str_end2 = dt_now.AddDays(1).ToString("yyyy-MM-dd") + " 05:00:00";

            }
            else if (classes == 1)
            {
                str_start1 = dt_now.ToString("yyyy-MM-dd") + " 23:00:00";
                str_end1 = dt_now.ToString("yyyy-MM-dd") + " 23:30:00";

                str_start2 = dt_now.AddDays(1).ToString("yyyy-MM-dd") + " 04:40:00";
                str_end2 = dt_now.AddDays(1).ToString("yyyy-MM-dd") + " 05:00:00";

            }
            else
            {
                str_start1 = dt_now.AddDays(-1).ToString("yyyy-MM-dd") + " 23:00:00";
                str_end1 = dt_now.AddDays(-1).ToString("yyyy-MM-dd") + " 23:30:00";

                str_start2 = dt_now.ToString("yyyy-MM-dd") + " 04:40:00";
                str_end2 = dt_now.ToString("yyyy-MM-dd") + " 05:00:00";

            }

            dt1_start.setDateTime(Convert.ToDateTime(str_start1));
            dt1_end.setDateTime(Convert.ToDateTime(str_end1));
            dt2_start.setDateTime(Convert.ToDateTime(str_start2));
            dt2_end.setDateTime(Convert.ToDateTime(str_end2));

            UpdateData();
            thread = new Thread(UpdateData);
           // thread.Start();
        }

        private void UpdateData()
        {
            string command = string.Format("select top 1* from ContactInfo where Department={0}", currentDepartment);
            SqlDataReader sr = sqlHelper.ExecuteReader(command);
            ls_info = new ObservableCollection<ContactInfo>();
            while (sr.Read())
            {
                try
                {
                    Name1 = sr["Phone4"].ToString();
                    Name2 = sr["Phone5"].ToString();
                    Name3 = sr["Phone6"].ToString();
                    Phone1 = sr["Phone1"].ToString();
                    Phone2 = sr["Phone2"].ToString();
                    Phone3 = sr["Phone3"].ToString();
                    Mail1 = sr["Mail1"].ToString();
                    Mail2 = sr["Mail2"].ToString();
                    Mail3 = sr["Mail3"].ToString();
                    Min1 = int.Parse(sr["Mail4"].ToString());
                    Min2 = int.Parse(sr["Mail5"].ToString());
                    Min3 = int.Parse(sr["Mail6"].ToString());
                    ModelSet = int.Parse(sr["Flag"].ToString());

                    string[] arrName = Name1.Split(',');
                    string[] arrPhone = Phone1.Split(',');
                    string[] arrMail = Mail1.Split(',');
                    ContactInfo info = new ContactInfo();
                    
                    for (int i = 0; i < arrName.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(arrMail[i]) || !string.IsNullOrEmpty(arrPhone[i]))
                        {
                            info = new ContactInfo();
                            info.ContactName = arrName[i];
                            info.ContactPhone = arrPhone[i];
                            info.ContactMail = arrMail[i];
                            info.ContactMin = Min1.ToString();
                            info.ContactType = "0";
                            ls_info.Add(info);
                        }
                    }

                    arrName = Name2.Split(',');
                    arrPhone = Phone2.Split(',');
                    arrMail = Mail2.Split(',');

                    for (int i = 0; i < arrName.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(arrMail[i]) || !string.IsNullOrEmpty(arrPhone[i]))
                        {
                            info = new ContactInfo();
                            info.ContactName = arrName[i];
                            info.ContactPhone = arrPhone[i];
                            info.ContactMail = arrMail[i];
                            info.ContactMin = Min2.ToString();
                            info.ContactType = "1";
                            ls_info.Add(info);
                        }
                    }

                    arrName = Name3.Split(',');
                    arrPhone = Phone3.Split(',');
                    arrMail = Mail3.Split(',');

                    for (int i = 0; i < arrName.Count(); i++)
                        {
                            if (!string.IsNullOrEmpty(arrMail[i]) || !string.IsNullOrEmpty(arrPhone[i]))
                            {
                                info = new ContactInfo();
                                info.ContactName = arrName[i];
                                info.ContactPhone = arrPhone[i];
                                info.ContactMail = arrMail[i];
                                info.ContactMin = Min3.ToString();
                                info.ContactType = "2";
                                ls_info.Add(info);
                            }
                    }
                    //dataSettings.Dispatcher.Invoke(new Action(()=> { dataSettings.ItemsSource = ls_info; }));
                    if (ls_info.Count() > 0)
                        dataSettings.Dispatcher.BeginInvoke(new Action(() => { dataSettings.ItemsSource = ls_info; }));

                    //this.DataContext = this;

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {


                        tb_time_1.Text = Min1.ToString();
                        tb_time_2.Text = Min2.ToString();
                        tb_time_3.Text = Min3.ToString();

                        if (ModelSet == 0)
                            rb_norm.IsChecked = true;
                        else
                            rb_demo.IsChecked = true;

                    }));

                }
                catch (Exception err) { Console.WriteLine(err.Message); }
                bflag = true;
            }

        }
        private void InitDate()
        {

            str_start1 = dt1_start.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            str_end1 = dt1_end.DateTime.ToString("yyyy-MM-dd HH:mm:ss");

            str_start2 = dt2_start.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            str_end2 = dt2_end.DateTime.ToString("yyyy-MM-dd HH:mm:ss");


            int Type1 = gettime(dt1_start.DateTime);
            int Type2 = gettime(dt1_end.DateTime);
            int Type3 = gettime(dt2_start.DateTime);
            int Type4 = gettime(dt2_end.DateTime);

            int span = (int)(dt2_start.DateTime.Subtract(dt1_end.DateTime).TotalSeconds);
            spansec1= (int)(dt1_end.DateTime.Subtract(dt1_start.DateTime).TotalSeconds);
            spansec2 = (int)(dt2_end.DateTime.Subtract(dt2_start.DateTime).TotalSeconds);

            if ((Type1 == 1 || Type1 == 2) && (Type2 == 1 || Type2 == 2) && (Type3 == 1 || Type3 == 2) && (Type4 == 1 || Type4 == 2) && span > 0 && spansec1 > 0 && spansec2 > 0)
            {
                return;
            }

            dt_now = DateTime.Now;

            int classes = gettime(DateTime.Now);
            if (classes == 0)
            {
                str_start1 = dt_now.ToString("yyyy-MM-dd") + " 23:00:00";
                str_end1 = dt_now.ToString("yyyy-MM-dd") + " 23:30:00";

                str_start2 = dt_now.AddDays(1).ToString("yyyy-MM-dd") + " 04:40:00";
                str_end2 = dt_now.AddDays(1).ToString("yyyy-MM-dd") + " 05:00:00";

            }
            else if (classes == 1)
            {
                str_start1 = dt_now.ToString("yyyy-MM-dd") + " 23:00:00";
                str_end1 = dt_now.ToString("yyyy-MM-dd") + " 23:30:00";

                str_start2 = dt_now.AddDays(1).ToString("yyyy-MM-dd") + " 04:40:00";
                str_end2 = dt_now.AddDays(1).ToString("yyyy-MM-dd") + " 05:00:00";

            }
            else
            {
                str_start1 = dt_now.AddDays(-1).ToString("yyyy-MM-dd") + " 23:00:00";
                str_end1 = dt_now.AddDays(-1).ToString("yyyy-MM-dd") + " 23:30:00";

                str_start2 = dt_now.ToString("yyyy-MM-dd") + " 04:40:00";
                str_end2 = dt_now.ToString("yyyy-MM-dd") + " 05:00:00";

            }

            dt1_start.setDateTime(Convert.ToDateTime(str_start1));
            dt1_end.setDateTime(Convert.ToDateTime(str_end1));
            dt2_start.setDateTime(Convert.ToDateTime(str_start2));
            dt2_end.setDateTime(Convert.ToDateTime(str_end2));
        }

        private void UpdataDB(bool flag)
        {

            string command = "";

            Name1 = ""; Name2 = ""; Name3 = "";
            Phone1 = ""; Phone2 = ""; Phone3 = "";
            Mail1 = ""; Mail2 = ""; Mail3 = "";

            try
            {
                dataSettings.Dispatcher.BeginInvoke(new Action(() => { dataSettings.ItemsSource = null; }));

                if (ls_info.Count() > 0)
                    dataSettings.Dispatcher.BeginInvoke(new Action(() => { dataSettings.ItemsSource = ls_info; }));

                for (int i = 0; i < ls_info.Count(); i++) {
                    switch (ls_info.ElementAt(i).ContactType) {
                        case "0":
                            Name1 += ls_info.ElementAt(i).ContactName + ",";
                            Phone1 += ls_info.ElementAt(i).ContactPhone + ",";
                            Mail1 += ls_info.ElementAt(i).ContactMail + ",";
                            break;
                        case "1":
                            Name2 += ls_info.ElementAt(i).ContactName + ",";
                            Phone2 += ls_info.ElementAt(i).ContactPhone + ",";
                            Mail2 += ls_info.ElementAt(i).ContactMail + ",";
                            break;
                        case "2":
                            Name3 += ls_info.ElementAt(i).ContactName + ",";
                            Phone3 += ls_info.ElementAt(i).ContactPhone + ",";
                            Mail3 += ls_info.ElementAt(i).ContactMail + ",";
                            break;
                        default:break;
                    }

                }

                InitDate();

                gettime(Convert.ToDateTime(str_start1));
                totalsec1 = totalsec;

                gettime(Convert.ToDateTime(str_start2));
                totalsec2 = totalsec;

                spansec1 = (int)(Convert.ToDateTime(str_end1).Subtract(Convert.ToDateTime(str_start1)).TotalSeconds);
                spansec2 = (int)(Convert.ToDateTime(str_end2).Subtract(Convert.ToDateTime(str_start2)).TotalSeconds);

                Settings1 = totalsec1.ToString() + "," + spansec1.ToString() + "," + totalsec2.ToString() + "," + spansec2.ToString();

                if (!flag)
                    command = string.Format("insert into ContactInfo(Department,"
                        + "Phone1, Phone2, Phone3, Phone4, Phone5, Phone6, Mail1, Mail2, Mail3,Flag,Mail4, Mail5, Mail6, Settings1)"
                        + "values({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10},'{11}','{12}','{13}','{14}')",
                        currentDepartment, Phone1, Phone2, Phone3, Name1, Name2, Name3, Mail1, Mail2, Mail3, ModelSet, Min1.ToString(), Min2.ToString(), Min3.ToString(), Settings1);
                else
                    command = string.Format("UPDATE ContactInfo set Phone1='{0}', Phone2='{1}', Phone3='{2}', Phone4='{3}', Phone5='{4}', Phone6='{5}', Mail1='{6}', Mail2='{7}', Mail3 = '{8}',Mail4 = '{9}',Mail5 = '{10}',Mail6 = '{11}', Flag = {12} , Settings1 = '{13}' where Department={14}",
                        Phone1, Phone2, Phone3, Name1, Name2, Name3, Mail1, Mail2, Mail3, Min1.ToString(), Min2.ToString(), Min3.ToString(), ModelSet, Settings1, currentDepartment);
                sqlHelper.ExecuteNonQuery(command);
            }
            catch (Exception err) { Console.WriteLine(err.Message); }
        }

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
                totalsec = (int)ts_start;
                return 0;
            }
            // 20点-24点之间 夜班
            else if (ts_end >= 0)
            {
                totalsec = (int)ts_end;
                return 1;
            }
            // 距离第二天早上8点 8小时以内
            else if (ts_start < 0 && ts_start > -28800)
            {
                totalsec = (int)(ts_end + 86400);
                return 2;
            }
            // 无效时间段
            else
                return 3;
        }


        public void setParams(int Department) {
            this.currentDepartment = Department;
        }
    }
}
