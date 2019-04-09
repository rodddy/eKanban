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

using System.Data.SqlClient;

using System.Threading;
using System.IO;

namespace eKanban_Console
{
    /// <summary>
    /// OutPutParams_Win.xaml 的交互逻辑
    /// </summary>
    public partial class OutPutParams_Win : Window
    {

        private DateTime dt;
        private string start_class = " 08:00:00";
        private string end_class = " 20:00:00";
        private string start_date;
        private string end_date;
        private Thread thread;
        private List<DateTime> ls_dt = new List<DateTime>();
        private int currentDepartment = 0;

        // 每台设备gyr 时间
        private int[] arrActivation = new int[10];
        private int[] GYRTime = new int[30];

        private int[] GYRTimePerMechine = new int[190];
        private int[] arrActivationPerMechine = new int[60];


        private string[] Line_Sets;

        public OutPutParams_Win()
        {
            
            InitializeComponent();
            InitView();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //InitView();
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

        private void cmb_class_start_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_class_start.SelectedIndex == 0)
                start_class = " 08:00:00";
            else
                start_class = " 20:00:00";
        }

        private void cmb_class_end_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_class_end.SelectedIndex == 0)
                end_class = " 20:00:00";
            else
                end_class = " 08:00:00";
        }


        private void dp_start_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            start_date = ((DateTime)(dp_start.SelectedDate)).ToString("yyyy-MM-dd");

        }

        private void dp_end_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            end_date = ((DateTime)(dp_end.SelectedDate)).AddDays(1).ToString("yyyy-MM-dd");

        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {

            DateTime start_time = Convert.ToDateTime(start_date + start_class);
            DateTime end_time = Convert.ToDateTime(end_date + end_class);

            ls_dt = getDateList(start_time, end_time);


            thread = new Thread(UpdateData);
            thread.Start();
            thread.IsBackground = true;

            Console.WriteLine(ls_dt.ToString());
        }

        private void InitView()
        {
            dt = DateTime.Now;
            dp_start.SelectedDate = dt.AddDays(-7);
            dp_end.SelectedDate = dt;
            thread = new Thread(UpdateData);


            Line_Sets = new string[DeviceInfo.Department_5.Count()];
            for (int i = 0; i < DeviceInfo.Department_5.Count(); i++)
            {
                Line_Sets[i] = DeviceInfo.Department_5.ElementAt(i).Name;
            }

        }

        //private int gettime(DateTime dt)
        //{
        //    start_time = dt.ToString("yyyy-MM-dd") + " 08:00:00";
        //    DateTime start_dt = Convert.ToDateTime(start_time);
        //    end_time = dt.ToString("yyyy-MM-dd") + " 20:00:00";
        //    DateTime end_dt = Convert.ToDateTime(end_time);
        //    double ts_start = dt.Subtract(start_dt).TotalMinutes;
        //    double ts_end = dt.Subtract(end_dt).TotalMinutes;

        //    end_time = dt.ToString("yyyy-MM-dd HH:mm:ss");

        //    // 在8点-20点之间 白班
        //    if (ts_start >= 0 && ts_end < 0)
        //    {
        //        classes = 0;

        //        //totalmins = ts_start;
        //        return 0;
        //    }
        //    // 20点-24点之间 夜班
        //    else if (ts_end >= 0)
        //    {
        //        start_time = dt.ToString("yyyy-MM-dd") + " 20:00:00";
        //        //end_time = dt.ToString("yyyy-MM-dd HH:mm:ss");
        //        classes = 1;
        //        //totalmins = ts_end;
        //        return 1;
        //    }
        //    // 距离第二天早上8点 8小时以内
        //    else if (ts_start < 0 && ts_start > -480)
        //    {
        //        start_time = dt.AddDays(-1).ToString("yyyy-MM-dd") + " 20:00:00";
        //        //end_time = dt.ToString("yyyy-MM-dd HH:mm:ss");
        //        //totalmins = ts_end + 1440;
        //        classes = 1;
        //        return 2;
        //    }
        //    // 无效时间段
        //    else
        //        return 3;

        //}

        private List<DateTime> getDateList(DateTime dt_start_check, DateTime dt_end_check)
        {
            List<DateTime> ls_check = new List<DateTime>();

            string str_splitter = dt_start_check.ToString("yyyy-MM-dd") + " 08:00:00";
            DateTime dt_splitter = Convert.ToDateTime(str_splitter);

            if (dt_splitter.Subtract(dt_start_check).TotalSeconds > 0) ;
            else if (dt_splitter.AddHours(12).Subtract(dt_start_check).TotalSeconds > 0)
                dt_splitter = dt_splitter.AddHours(12);
            else
                dt_splitter = dt_splitter.AddHours(24); ;

            if (dt_end_check.Subtract(dt_start_check).TotalSeconds > 0)
            {
                ls_check.Add(dt_start_check);
                ls_check.Add(dt_end_check);
                while (dt_splitter.Subtract(dt_start_check).TotalSeconds > 0 && dt_end_check.Subtract(dt_splitter).TotalSeconds > 0)
                {
                    ls_check.Insert(ls_check.Count() - 1, dt_splitter);
                    dt_splitter = dt_splitter.AddHours(12);
                }

            }
            return ls_check;
        }

        private void setDepartment(int depart)
        {
            this.currentDepartment = depart;
        }


        // 查询之前班次的数据
        private void UpdateData()
        {
            try
            {
                SqlDataReader sr;

                if (ls_dt.Count() > 0)
                {

                    Microsoft.Win32.SaveFileDialog ofd = new Microsoft.Win32.SaveFileDialog();
                    ofd.DefaultExt = ".csv";
                    ofd.Filter = "文件(*.csv)|*.csv";
                    ofd.FileName = "五车间设备状态统计_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    string strFileName = ofd.FileName;
                    ofd.CheckFileExists = false;

                    ofd.ValidateNames = true;
                    if (ofd.ShowDialog() == true)
                    {
                        strFileName = ofd.FileName;

                        //其他代码
                    }
                    string extension = System.IO.Path.GetExtension(strFileName);
                    if (extension == ".csv")
                    {
                        FileInfo fi = new FileInfo(strFileName);
                        if (!fi.Directory.Exists)
                        {
                            fi.Directory.Create();
                        }
                        FileStream fs = new FileStream(strFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

                        //write columns header
                        string data = "时间,班次,项目," + string.Join(",", Line_Sets);
                        sw.WriteLine(data);


                        for (int i = 1; i < ls_dt.Count(); i++)
                        {
                            string str_dt = "";
                            string str_dt_end = "";
                            string cmd = "";
                            string gyr_str = "";
                            string arr_str = "";
                            int classes = 0;

                            if (ls_dt.ElementAt(i).Subtract(DateTime.Now).TotalSeconds < 10)
                            {


                                str_dt = ls_dt.ElementAt(i - 1).ToString("yyyy-MM-dd HH:mm:ss");
                                str_dt_end = ls_dt.ElementAt(i).ToString("yyyy-MM-dd HH:mm:ss");

                                cmd = string.Format("select top 1* from device_statusClassCount where UpdateTime = '{0}' and Department = {1}", str_dt, currentDepartment);
                                sr = sqlHelper.ExecuteReader(cmd);

                                if (ls_dt.ElementAt(i - 1).Hour == 8)
                                    classes = 0;
                                else
                                    classes = 1;


                                if (sr.Read())
                                {
                                    gyr_str = sr["GYRCount"].ToString();
                                }
                                else
                                {

                                    cmd = string.Format("select top 1* from device_statusCount where UpdateTime > '{0}' and UpdateTime < '{1}' order by UpdateTime desc",
                                                        str_dt, str_dt_end);
                                    sr = sqlHelper.ExecuteReader(cmd);

                                    if (sr.Read())
                                    {
                                        arr_str = sr["Activation"].ToString();
                                        gyr_str = sr["GYRCount"].ToString();

                                        cmd = string.Format("insert into device_statusClassCount (Department,Activation,GYRCount,UpdateTime,Classes)"
                                            + " values({0},'{1}','{2}','{3}',{4})", currentDepartment, arr_str, gyr_str, str_dt, classes);
                                        sr = sqlHelper.ExecuteReader(cmd);

                                    }
                                }

                                if (!string.IsNullOrEmpty(gyr_str))
                                {
                                    string[] arrGYR = gyr_str.Split(',');

                                    for (int k = 0; k < 10; k++)
                                    {
                                        for (int m = 0; m < 6; m++)
                                        {
                                            GYRTimePerMechine[k * 19 + m] = int.Parse(arrGYR[k * 19 + m]) / 60;
                                            GYRTimePerMechine[k * 19 + 6 + m] = int.Parse(arrGYR[k * 19 + m + 6]) / 60;
                                            GYRTimePerMechine[k * 19 + 12 + m] = int.Parse(arrGYR[k * 19 + m + 12]) / 60;

                                            arrActivationPerMechine[k * 6 + m] = (int)Math.Round(GYRTimePerMechine[k * 19 + m] * 100.0f / 720);

                                        }
                                        GYRTimePerMechine[k * 19 + 18] = int.Parse(arrGYR[k * 19 + 18]);
                                        GYRTime[k * 3] = GYRTimePerMechine[k * 19 + GYRTimePerMechine[k * 19 + 18]];
                                        GYRTime[k * 3 + 1] = GYRTimePerMechine[k * 19 + GYRTimePerMechine[k * 19 + 18] + 6];
                                        GYRTime[k * 3 + 2] = GYRTimePerMechine[k * 19 + GYRTimePerMechine[k * 19 + 18] + 12];
                                        arrActivation[k] = arrActivationPerMechine[k * 6 + GYRTimePerMechine[k * 19 + 18]];

                                    }

                                    //arrActivation = new int[10];

                                    string str_class = "白班";
                                    if(classes ==1)
                                        str_class = "夜班";

                                    data = str_dt+","+ str_class + ",稼动率," + string.Join(",", arrActivation);
                                    sw.WriteLine(data);

                                    //line light status ;GYRTime = new int[30];
                                    string[] arr_light = new string[3] { "正常时间(min)", "等待时间(min)", "故障时间(min)" };
                                    for (int k = 0; k < 3; k++)
                                    {
                                        data = ",," + arr_light[k] + ",";
                                        for (int m = 0; m < 10; m++)
                                        {
                                            data += GYRTime[m * 3 + k].ToString() + ",";
                                        }
                                        sw.WriteLine(data);
                                    }

                                    //each Mechine activiation  arrActivationPerMechine = new int[60];

                                    for (int k = 0; k < 6; k++)
                                    {
                                        data = ",," + DeviceInfo.Device_Set[k] + "稼动率,";
                                        for (int j = 0; j < 10; j++)
                                        {
                                            data += arrActivationPerMechine[j * 6 + k].ToString() + ",";
                                        }
                                        sw.WriteLine(data);
                                    }

                                    //each Mechine GYRTime, GYRTimePerMechine = new int[190];
                                    for (int k = 0; k < 6; k++)
                                    {
                                        data = ",," + DeviceInfo.Device_Set[k] + " " + arr_light[0] + ",";
                                        for (int m = 0; m < 10; m++)
                                        {
                                            data += GYRTimePerMechine[m * 19 + k].ToString() + ",";
                                        }
                                        sw.WriteLine(data);

                                        data = ",," + DeviceInfo.Device_Set[k] + " " + arr_light[1] + ",";
                                        for (int m = 0; m < 10; m++)
                                        {
                                            data += GYRTimePerMechine[m * 19 + k + 6].ToString() + ",";
                                        }
                                        sw.WriteLine(data);

                                        data = ",," + DeviceInfo.Device_Set[k] + " " + arr_light[2] + ",";
                                        for (int m = 0; m < 10; m++)
                                        {
                                            data += GYRTimePerMechine[m * 19 + k + 12].ToString() + ",";
                                        }
                                        sw.WriteLine(data);
                                    }

                                }

                            }
                        }

                        sw.Close();
                        fs.Close();
                        MessageBox.Show("导出数据成功");
                    }
                }
            }
            catch (Exception err) { MessageBox.Show(err.Message); }
        }

    }
}
