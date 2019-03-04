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

using System.Windows.Threading;
using System.Threading;

using System.Data.SqlClient;

namespace eKanban_Console
{
    /// <summary>
    /// Setting_Win.xaml 的交互逻辑
    /// </summary>
    public partial class Setting_Win : Window
    {

        private Thread thread;
        private int currentDepartment = 5;
        private string Name1, Name2, Name3, Phone1, Phone2, Phone3;
        private string Mail1, Mail2, Mail3;

        private bool bflag = false;
        private int ModelSet = 0;
        private int Min1 = 15;
        private int Min2 = 30;
        private int Min3 = 45;
        public Setting_Win()
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

            thread = new Thread(UpdateData);
            thread.Start();
        }

        private void UpdateData()
        {
            string command = string.Format("select top 1* from ContactInfo where Department={0}", currentDepartment);
            SqlDataReader sr = sqlHelper.ExecuteReader(command);
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

                }
                catch (Exception err) { Console.WriteLine(err.Message); }
                bflag = true;
            }

            this.Dispatcher.BeginInvoke(new Action(() =>
            {

                if (!string.IsNullOrEmpty(Name1))
                    tb_contact_1.Text = Name1;
                if (!string.IsNullOrEmpty(Name2))
                    tb_contact_2.Text = Name2;
                if (!string.IsNullOrEmpty(Name3))
                    tb_contact_3.Text = Name3;

                if (!string.IsNullOrEmpty(Phone1))
                    tb_phone_1.Text = Phone1;
                if (!string.IsNullOrEmpty(Phone2))
                    tb_phone_2.Text = Phone2;
                if (!string.IsNullOrEmpty(Phone3))
                    tb_phone_3.Text = Phone3;

                if (!string.IsNullOrEmpty(Mail1))
                    tb_mail_1.Text = Mail1;
                if (!string.IsNullOrEmpty(Mail2))
                    tb_mail_2.Text = Mail2;
                if (!string.IsNullOrEmpty(Mail3))
                    tb_mail_3.Text = Mail3;

                tb_time_1.Text = Min1.ToString();
                tb_time_2.Text = Min2.ToString();
                tb_time_3.Text = Min3.ToString();

                if (ModelSet == 0)
                    rb_norm.IsChecked = true;
                else
                    rb_demo.IsChecked = true;

            }));
        }

        private void UpdataDB(bool flag) {

            if (!string.IsNullOrEmpty(tb_contact_1.Text))
                Name1 = tb_contact_1.Text;
            if (!string.IsNullOrEmpty(tb_contact_2.Text))
                Name2 = tb_contact_2.Text;
            if (!string.IsNullOrEmpty(tb_contact_3.Text))
                Name3 = tb_contact_3.Text;

            if (!string.IsNullOrEmpty(tb_phone_1.Text))
                Phone1 = tb_phone_1.Text;
            if (!string.IsNullOrEmpty(tb_phone_2.Text))
                Phone2 = tb_phone_2.Text;
            if (!string.IsNullOrEmpty(tb_phone_3.Text))
                Phone3 = tb_phone_3.Text;

            if (!string.IsNullOrEmpty(tb_mail_1.Text))
                Mail1 = tb_mail_1.Text;
            if (!string.IsNullOrEmpty(tb_mail_2.Text))
                Mail2 = tb_mail_2.Text;
            if (!string.IsNullOrEmpty(tb_mail_3.Text))
                Mail3 = tb_mail_3.Text;

            string command = "";

            if (!flag)
                command = string.Format("insert into ContactInfo(Department,"
                    + "Phone1, Phone2, Phone3, Phone4, Phone5, Phone6, Mail1, Mail2, Mail3,Flag,Mail4, Mail5, Mail6)"
                    + "values({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10},'{11}','{12}','{13}')",
                    currentDepartment, Phone1, Phone2, Phone3, Name1, Name2, Name3, Mail1, Mail2, Mail3, ModelSet, Min1.ToString(), Min2.ToString(), Min3.ToString());
            else
                command = string.Format("UPDATE ContactInfo set Phone1='{0}', Phone2='{1}', Phone3='{2}', Phone4='{3}', Phone5='{4}', Phone6='{5}', Mail1='{6}', Mail2='{7}', Mail3 = '{8}',Mail4 = '{9}',Mail5 = '{10}',Mail6 = '{11}', Flag = {12} where Department={13}",
                    Phone1, Phone2, Phone3, Name1, Name2, Name3, Mail1, Mail2, Mail3, Min1.ToString(), Min2.ToString(), Min3.ToString(), ModelSet, currentDepartment);
            sqlHelper.ExecuteNonQuery(command);
        }

        public void setParams(int Department) {
            this.currentDepartment = Department;
        }
    }
}
