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
using System.Windows.Threading;
using System.Threading;
using System.Data;

using System.IO;
using System.Xml;

using System.Data.SqlClient;
using LiveCharts;
using LiveCharts.Wpf;

namespace eKanban_Console
{
    /// <summary>
    /// MainWindowConsole.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow_Console : Window
    {
        DispatcherTimer timer_status = new DispatcherTimer();
        DispatcherTimer timer_update = new DispatcherTimer();

        DateTime dt;

        private string start_time;
        private string end_time;
        private double totalmins = 1;
        private bool bFlagUpdateData = false;
        private DateTime dt_updatetime = DateTime.Now;

        private int m_Interval = 1000;

        private int[] GYRTime = new int[30];
        private int[] arrActivation = new int[10];

        private int[] GYRTimePerMechine = new int[190];
        private int[] arrActivationPerMechine = new int[60];
        private int[] deviceStatus = new int[60];

        private string[] Line_Sets = new string[10] { "SMT 09线","SMT 10线", "SMT 11线", "SMT 12线", "SMT 13线", "SMT 14线", "SMT 15线", "SMT 16线", "SMT 17线", "SMT 22线"};

        private ChartValues<double> act_value = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private ChartValues<double> y_value = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private ChartValues<double> r_value = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private int currentDepartment = 5;
        private int currentLine = 0;

        private string classes = "";
        private Thread thread;

        private ListViewItem itm_selected;// selected listview item

        public static MainWindow_Console frm = null;
        public int status = 0;

        public MainWindow_Console()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            frm = this;
            InitView();

        }
        private void InitView()
        {

            dt = DateTime.Now;
            gettime(dt);

            timer_status.Interval = TimeSpan.FromMilliseconds(1000);
            timer_status.Tick += Timer_status_Tick;
            timer_status.Start();

            timer_update.Interval = TimeSpan.FromMilliseconds(60000);
            timer_update.Tick += Timer_update_Tick;
            timer_update.Start();

            thread = new Thread(UpdateData);
            thread.Start();
            thread.IsBackground = true;

            dt_start.setDateTime(Convert.ToDateTime(start_time));

            cmb_department.ItemsSource = DeviceInfo.DepartmentList;

        }

        private void cmb_department_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmb_department.SelectedIndex) {
                case 0: lv_Lines.ItemsSource = DeviceInfo.Department_5;break;
                case 1: lv_Lines.ItemsSource = DeviceInfo.Department_6; break;
                case 2: lv_Lines.ItemsSource = DeviceInfo.Department_7; break;
                case 3: lv_Lines.ItemsSource = DeviceInfo.Department_8; break;
            }
            
        }
        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                itm_selected = sender as ListViewItem;
                DeviceInfo info = itm_selected.Content as DeviceInfo;
                currentLine = Array.IndexOf(Line_Sets, info.Name);
                Console.WriteLine(currentLine);
            }
            catch (Exception err) { Console.WriteLine(err.Message); }

        }
        private void ListViewItem_MouseLeave(object sender, MouseEventArgs e)
        {
            itm_selected = null;
        }
        private void img_minimize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void img_activation_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!bFlagUpdateData)
            {
                MessageBox.Show("当前时间段没有数据！");
                return;
            }

            if (currentLine >= 0)
            {
                Activation_Win act_win = new Activation_Win();
                int[] arr = new int[6] ;
                int[] arrStatus = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    arr[i] = arrActivationPerMechine[currentLine * 6 + i];
                    arrStatus[i] = deviceStatus[currentLine * 6 + i];
                }
                if (arrStatus.Sum() == 48)
                    MessageBox.Show("该产线没有数据！");
                else
                {
                    act_win.setParams(Line_Sets[currentLine], arrStatus, arr);
                    act_win.Show();
                }
            }
            else {
                MessageBox.Show("该产线不存在！");
            }
        }

        private void img_yr_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!bFlagUpdateData) {
                MessageBox.Show("当前时间段没有数据！");
                return;
            }
            if (currentLine >= 0)
            {
                YR_Win yr_win = new YR_Win();
                int[] arr = new int[12];
                int[] arrStatus = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    arr[i] = GYRTimePerMechine[currentLine * 19 + 6 + i];
                    arr[i + 6] = GYRTimePerMechine[currentLine * 19 + 12 + i];
                    arrStatus[i] = deviceStatus[currentLine * 6 + i];
                }
                //arr = new int[12] { 15, 85, 48, 96, 45, 85, 74, 48, 34, 55, 48, 48 };
                if (arrStatus.Sum() == 48)
                    MessageBox.Show("该产线没有数据！");
                else
                {
                    yr_win.setParams(Line_Sets[currentLine], arrStatus, arr);
                    yr_win.Show();
                }
            }
            else
            {
                MessageBox.Show("该产线不存在！");
            }
        }

        private void img_log_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (currentLine >= 0)
            {
                MaintainLog_Win log_win = new MaintainLog_Win();
                log_win.setParams(currentDepartment, currentLine);
                log_win.Show();
            }
            else { MessageBox.Show("该产线不存在！"); }
        }

        private void btn_check_Click(object sender, RoutedEventArgs e)
        {
            //start_time = dt_start.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            //end_time = dt_end.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            timer_update.Stop();
            tb_status.Text = "历史数据查询中... ...";

            totalmins = dt_end.DateTime.Subtract(dt_start.DateTime).TotalMinutes;
            try
            {
                thread = new Thread(CheckData);
                thread.Start();
                thread.IsBackground = true;
            }
            catch (Exception err) { Console.WriteLine(err.Message); }
        }
        
        private void btn_return_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                thread = new Thread(UpdateData);
                thread.Start();
                thread.IsBackground = true;
            }
            catch (Exception err) { Console.WriteLine(err.Message); }
        }
        
        private void btn_setting_Click(object sender, RoutedEventArgs e)
        {
            //Setting_Win setting_frm = new Setting_Win();
            //setting_frm.Show();

            Win_login log_frm = new Win_login();
            log_frm.Show();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            //string messageBoxText = "导出历史数据";
            //string caption = "导出数据";
            //MessageBoxButton button = MessageBoxButton.YesNoCancel;
            //MessageBoxImage icon = MessageBoxImage.Warning;
            //MessageBoxResult rlt= MessageBox.Show(messageBoxText, caption, button, icon);
            //switch (rlt)
            //{
            //    case MessageBoxResult.Yes:
            //        // ...                      
            //        break;
            //    case MessageBoxResult.No:
            //        // ...                      
            //        break;
            //    case MessageBoxResult.Cancel:
            //        // ...                     
            //        break;
            //}

            OutputDialog dlg = new OutputDialog();
            dlg.ShowDialog();

            switch (frm.status) {

                case 2:
                    {
                        if (!bFlagUpdateData)
                        {
                            MessageBox.Show("当前时间段没有数据！");
                            return;
                        }

                        Microsoft.Win32.SaveFileDialog ofd = new Microsoft.Win32.SaveFileDialog();
                        ofd.DefaultExt = ".csv";
                        ofd.Filter = "文件(*.csv)|*.csv|图像文件|*.bmp";
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
                            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

                            //write columns header
                            string data = "项目," + string.Join(",", Line_Sets);
                            sw.WriteLine(data);


                            //arrActivation = new int[10];
                            data = "稼动率," + string.Join(",", arrActivation);
                            sw.WriteLine(data);

                            //line light status ;GYRTime = new int[30];
                            string[] arr_light = new string[3] { "正常时间(min)", "等待时间(min)", "故障时间(min)" };
                            for (int i = 0; i < 3; i++)
                            {
                                data = arr_light[i] + ",";
                                for (int j = 0; j < 10; j++)
                                {
                                    data += GYRTime[j * 3 + i].ToString() + ",";
                                }
                                sw.WriteLine(data);
                            }

                            //each Mechine activiation  arrActivationPerMechine = new int[60];

                            for (int i = 0; i < 6; i++)
                            {
                                data = DeviceInfo.Device_Set[i] + " 稼动率,";
                                for (int j = 0; j < 10; j++)
                                {
                                    data += arrActivationPerMechine[j * 6 + i].ToString() + ",";
                                }
                                sw.WriteLine(data);
                            }

                            //each Mechine GYRTime, GYRTimePerMechine = new int[190];
                            for (int i = 0; i < 6; i++)
                            {
                                data = DeviceInfo.Device_Set[i] + " " + arr_light[0] + ",";
                                for (int j = 0; j < 10; j++)
                                {
                                    data += GYRTimePerMechine[j * 19 + i].ToString() + ",";
                                }
                                sw.WriteLine(data);

                                data = DeviceInfo.Device_Set[i] + " " + arr_light[1] + ",";
                                for (int j = 0; j < 10; j++)
                                {
                                    data += GYRTimePerMechine[j * 19 + i + 6].ToString() + ",";
                                }
                                sw.WriteLine(data);

                                data = DeviceInfo.Device_Set[i] + " " + arr_light[2] + ",";
                                for (int j = 0; j < 10; j++)
                                {
                                    data += GYRTimePerMechine[j * 19 + i + 12].ToString() + ",";
                                }
                                sw.WriteLine(data);
                            }
                            sw.Close();
                            fs.Close();

                        }
                        else if (extension == ".bmp")
                        {
                            GetPicFromControl(grid_export, "BMP", strFileName);
                        }
                        else
                            MessageBox.Show("文件类型错误，请重新选择！");

                        break;
                    }
                    
                case 1:
                    {
                        OutPutParams_Win out_win = new OutPutParams_Win();
                        out_win.Show();
                        break;
                    }
                case 0: break;
                default:break;


            }



        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            
            if (timer_status.IsEnabled)
                timer_status.Stop();
            if (timer_update.IsEnabled)
                timer_update.Stop();
            if (thread.IsAlive)
                thread.Abort();
            this.Close();
        }

        //截屏函数
        private void GetPicFromControl(FrameworkElement element, String type, String outputPath)
        {
            //96为显示器DPI

            RenderTargetBitmap bitmapRender = new RenderTargetBitmap((int)(this.ActualWidth), (int)((this.ActualHeight - 130) * 70 / 83 + 100), 96, 96, PixelFormats.Default);//位图 宽度  高度   水平DPI  垂直DPI  位图的格式    高度+100保证整个图都能截取

            //控件内容渲染RenderTargetBitmap

            bitmapRender.Render(element);
            BitmapEncoder encoder = null;
            //选取编码器
            switch (type.ToUpper())
            {
                case "BMP":
                    encoder = new BmpBitmapEncoder();
                    break;
                case "GIF":
                    encoder = new GifBitmapEncoder();
                    break;
                case "JPEG":
                    encoder = new JpegBitmapEncoder();
                    break;
                case "PNG":
                    encoder = new PngBitmapEncoder();
                    break;
                case "TIFF":
                    encoder = new TiffBitmapEncoder();
                    break;
                default:
                    break;
            }
            //对于一般的图片，只有一帧，动态图片是有多帧的。

            //CroppedBitmap cbmp = new CroppedBitmap(bitmapRender, new Int32Rect((int)(this.ActualWidth * 0.3), 100, (int)(this.ActualWidth * 0.7), (int)(this.Height - 100)));
            BitmapFrame frame = BitmapFrame.Create(bitmapRender);
            CroppedBitmap cbmp = new CroppedBitmap(frame, new Int32Rect((int)(frame.Width * 0.3), 100, (int)(frame.Width * 0.7), (int)(frame.Height - 100)));
            BitmapFrame frames = BitmapFrame.Create(cbmp);

            encoder.Frames.Add(frames);//添加图

            if (!Directory.Exists(System.IO.Path.GetDirectoryName(outputPath)))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outputPath));
            using (var file = File.Create(outputPath))//存储文件
                encoder.Save(file);
        }

        private void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            EncodeVisual(visual, fileName, encoder);
        }

        private static void EncodeVisual(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using (var stream = File.Create(fileName)) encoder.Save(stream);
        }


        // Update Data Per 5 min
        private void Timer_update_Tick(object sender, EventArgs e)
        {
            //for (int i = 0; i < 10; i++)
            //{
            //    statusdata.Activation_Series.ElementAt(i).Items[0].Number = arrActivation[i];
            //    statusdata.YR_Series.ElementAt(i).Items[0].Number = GYRTime[i * 3 + 1];
            //    statusdata.YR_Series.ElementAt(i).Items[1].Number = GYRTime[i * 3 + 2];
            //}
            dt = DateTime.Now;
            gettime(dt);
            try {
                //if (thread.IsAlive)
                //    thread.Abort();
                
                thread = new Thread(UpdateData);
                thread.Start();
                thread.IsBackground = true;

                this.Dispatcher.BeginInvoke(new Action(() => {

                    //gettime(DateTime.Now);
                    dt_start.setDateTime(Convert.ToDateTime(start_time));
                    dt_end.setDateTime(DateTime.Now);

                }));


            } catch (Exception err) { Console.WriteLine(err.Message); }

        }

        // update system status per 1 sec
        private void Timer_status_Tick(object sender, EventArgs e)
        {
            dt = DateTime.Now;
            gettime(dt);
            if (tb_class.Text != classes)
            {
                tb_class.Text = classes;

                // Data List Reset
            //    act_value = new ChartValues<double>();
            //    y_value = new ChartValues<double>();
            //    r_value = new ChartValues<double>();
            //    for (int i = 0; i < 10; i++) {
            //        act_value.Add(0);
            //        y_value.Add(0);
            //        r_value.Add(0);
            //    }

            //    this.Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        chart_Activation.SeriesCollection_Activation.ElementAt(0).Values = act_value;
            //        chart_YR.SeriesCollection.ElementAt(0).Values = y_value;
            //        chart_YR.SeriesCollection.ElementAt(1).Values = r_value;
            //    }));
            }

            tb_clock.Dispatcher.BeginInvoke(new Action(() => { tb_clock.Text = dt.ToString("yyyy-MM-dd HH:mm:ss"); }));
            
        }

        // 0 白班 1 2夜班 3 数据不在范围内
        private int gettime(DateTime dt)
        {
            start_time = dt.ToString("yyyy-MM-dd") + " 08:00:00";
            DateTime start_dt = Convert.ToDateTime(start_time);
            end_time = dt.ToString("yyyy-MM-dd") + " 20:00:00";
            DateTime end_dt = Convert.ToDateTime(end_time);
            double ts_start = dt.Subtract(start_dt).TotalMinutes;
            double ts_end = dt.Subtract(end_dt).TotalMinutes;

            end_time = dt.ToString("yyyy-MM-dd HH:mm:ss");

            // 在8点-20点之间 白班
            if (ts_start >= 0 && ts_end < 0)
            {
                classes = "白班";

                totalmins = ts_start;
                return 0;
            }
            // 20点-24点之间 夜班
            else if (ts_end >= 0)
            {
                start_time = dt.ToString("yyyy-MM-dd") + " 20:00:00";
                //end_time = dt.ToString("yyyy-MM-dd HH:mm:ss");
                classes = "夜班";
                totalmins = ts_end;
                return 1;
            }
            // 距离第二天早上8点 8小时以内
            else if (ts_start < 0 && ts_start > -480)
            {
                start_time = dt.AddDays(-1).ToString("yyyy-MM-dd") + " 20:00:00";
                //end_time = dt.ToString("yyyy-MM-dd HH:mm:ss");
                totalmins = ts_end + 1440;
                classes = "夜班";
                return 2;
            }
            // 无效时间段
            else
                return 3;

        }

        // return classes
        private List<DateTime> getDateList(DateTime dt_start_check,DateTime dt_end_check) {
            List<DateTime> ls_check = new List<DateTime>();

            string str_splitter = dt_start_check.ToString("yyyy-MM-dd") + " 08:00:00";
            DateTime dt_splitter = Convert.ToDateTime(str_splitter);

            if (dt_splitter.Subtract(dt_start_check).TotalSeconds > 0) ;
            else if (dt_splitter.AddHours(12).Subtract(dt_start_check).TotalSeconds > 0)
                dt_splitter = dt_splitter.AddHours(12);
            else
                dt_splitter = dt_splitter.AddHours(24); ;

            if (dt_end_check.Subtract(dt_start_check).TotalSeconds > 0) { 
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

        private double getStartInterval(DateTime dt_start_check)
        {
            string str_startInterval = dt_start_check.ToString("yyyy-MM-dd") + " 20:00:00";
            DateTime dt_startInterval = Convert.ToDateTime(str_startInterval);

            if (dt_start_check.Subtract(dt_startInterval).TotalSeconds >= 0) ;
            else if(dt_start_check.Subtract(dt_startInterval.AddHours(-12)).TotalSeconds >= 0)
                dt_startInterval = dt_startInterval.AddHours(-12);
            else
                dt_startInterval = dt_startInterval.AddHours(-24);

            return dt_start_check.Subtract(dt_startInterval).TotalSeconds;
        }


        string[] arrStatus = new string[60];
        string[] arrGYR = new string[200];
        //string[] arrAct = new string[60];
        string UpdateTime;
        //
        private void UpdateData()
        {

            string temp_sr = "";

            try
            {
                string command = string.Format("select top 1* from device_statusCount where UpdateTime >= dateadd(mi,-30,getdate()) order by UpdateTime desc");
                SqlDataReader sr = sqlHelper.ExecuteReader(command);
                if (sr.Read())
                {
                    temp_sr = sr["DeviceStatus"].ToString();
                    arrStatus = temp_sr.Split(',');

                    temp_sr = sr["GYRCount"].ToString();
                    arrGYR = temp_sr.Split(',');
                    //temp_sr = sr["Activation"].ToString();
                    //arrAct = temp_sr.Split(',');
                    UpdateTime = sr["UpdateTime"].ToString();
                    DateTime dt_update = Convert.ToDateTime(UpdateTime);
                    gettime(dt_update);
                    //act_value = new ChartValues<double>();
                    //y_value = new ChartValues<double>();
                    //r_value = new ChartValues<double>();

                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            GYRTimePerMechine[i * 19 + j] = int.Parse(arrGYR[i * 19 + j]) / 60;
                            GYRTimePerMechine[i * 19 + 6 + j] = int.Parse(arrGYR[i * 19 + 6 + j]) / 60;
                            GYRTimePerMechine[i * 19 + 12 + j] = int.Parse(arrGYR[i * 19 + 12 + j]) / 60;

                            deviceStatus[i * 6 + j] = int.Parse(arrStatus[i * 6 + j]);
                            arrActivationPerMechine[i * 6 + j] = (int)Math.Round(100f * GYRTimePerMechine[i * 19 + j] / totalmins);
                        }
                        GYRTimePerMechine[i * 19 + 18] = int.Parse(arrGYR[i * 19 + 18]);
                        GYRTime[i * 3 + 0] = GYRTimePerMechine[i * 19 + GYRTimePerMechine[i * 19 + 18]];
                        GYRTime[i * 3 + 1] = GYRTimePerMechine[i * 19 + GYRTimePerMechine[i * 19 + 18] + 6];
                        GYRTime[i * 3 + 2] = GYRTimePerMechine[i * 19 + GYRTimePerMechine[i * 19 + 18] + 12];

                        arrActivation[i] = arrActivationPerMechine[i * 6 + GYRTimePerMechine[i * 19 + 18]];

                        //act_value.Add((double)arrActivation[i]);
                        //y_value.Add((double)GYRTime[3 * i + 1]);
                        //r_value.Add((double)GYRTime[3 * i + 2]);

                        act_value[i] = (double)arrActivation[i];
                        y_value[i] = (double)GYRTime[3 * i + 1];
                        r_value[i] = (double)GYRTime[3 * i + 2];

                    }
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        chart_Activation.SeriesCollection_Activation.ElementAt(0).Values = act_value;
                        chart_YR.SeriesCollection.ElementAt(0).Values = y_value;
                        chart_YR.SeriesCollection.ElementAt(1).Values = r_value;

                        tb_activation.Text = "当前车间设备稼动率 (平均稼动率为：" + ((int)(act_value.Average() * 10) / 10f).ToString() + "% )";
                        tb_yrtime.Text = "当前车间产线异常时间统计 (总计：等待时间 " + ((int)y_value.Sum()).ToString() + " 分钟，故障时间 " + ((int)r_value.Sum()).ToString() + " 分钟)";
                        tb_status.Text = "";
                    }));

                    bFlagUpdateData = true;

                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("该时间段没有数据！");
                    bFlagUpdateData = false;
                }

            }
            catch (Exception err)
            {
                tb_status.Dispatcher.BeginInvoke(new Action(() => {
                    tb_status.Text = err.Message;
                }));
            }

            
        }


        // 查询数据

        private void CheckData()
        {

            string temp_sr = "";

            double totalInterval = dt_end.DateTime.Subtract(dt_start.DateTime).TotalMinutes;

            //string filename = DateTime.Now.ToString("yyyyMMddHHmmss")+"_"+((int)totalInterval).ToString() + ".txt";
            //FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);//创建写入文件

            //StreamWriter sw = new StreamWriter(fs);


            List<DateTime> ls_check = getDateList(dt_start.DateTime, dt_end.DateTime);
            int ls_count = 0;
            
            //for (int i = 0; i < 10; i++)
            //{
            //    for (int j = 0; j < 6; j++)
            //    {
            //        GYRTimePerMechine[i * 19 + j] = 1;
            //        GYRTimePerMechine[i * 19 + 6 + j] = 3;
            //        GYRTimePerMechine[i * 19 + 12 + j] = 0;

            //        arrActivationPerMechine[i * 6 + j] = 0;

            //    }
            //    GYRTimePerMechine[i * 19 + 18] = 0;
            //    GYRTime[i * 3 + 0] = 0;
            //    GYRTime[i * 3 + 1] = 4;
            //    GYRTime[i * 3 + 2] = 0;

            //    arrActivation[i] = 0;
            //}

            GYRTime = new int[30];
            GYRTimePerMechine = new int[190];
            arrActivation = new int[10];
            arrActivationPerMechine = new int[60];

            //act_value = new ChartValues<double>();
            //y_value = new ChartValues<double>();
            //r_value = new ChartValues<double>();

            int[] arrGYTTimeClass = new int[190];
            int IndexOfMinGreen = 0;
            int tempMin = 999999;

            bFlagUpdateData = false;
            try
            {
                while (ls_count < ls_check.Count() - 1 && ls_check != null)
                {

                    double startInterval = getStartInterval(ls_check[ls_count]);
                    int startFlag = 0;//初始化数据初始化标志位

                    string command = string.Format("select top 1* from device_statusCount where UpdateTime > '{0}' and UpdateTime < '{1}' order by UpdateTime asc",
                        ls_check[ls_count].ToString("yyyy-MM-dd HH:mm:ss"), ls_check[ls_count + 1].ToString("yyyy-MM-dd HH:mm:ss"));
                    SqlDataReader sr = sqlHelper.ExecuteReader(command);

                    if (sr.Read())
                    {

                        temp_sr = sr["GYRCount"].ToString();

                        //sw.WriteLine(temp_sr);//开始写入值

                        string[] arrGYR = temp_sr.Split(',');
                        temp_sr = sr["Activation"].ToString();
                        string[] arrAct = temp_sr.Split(',');

                        for (int i = 0; i < 10; i++)
                        {
                            for (int j = 0; j < 6; j++)
                            {

                                arrGYTTimeClass[i * 19 + j] = int.Parse(arrGYR[i * 19 + j]) ;
                                arrGYTTimeClass[i * 19 + 6 + j] = int.Parse(arrGYR[i * 19 + j + 6]);
                                arrGYTTimeClass[i * 19 + 12 + j] = int.Parse(arrGYR[i * 19 + j + 12]);
                                if (arrGYTTimeClass[i * 19 + j] + arrGYTTimeClass[i * 19 + 6 + j] + arrGYTTimeClass[i * 19 + 12 + j] > startInterval) {
                                    startFlag = 1;
                                }

                            }
                        }
                    }
                    if (startFlag == 1)
                        arrGYTTimeClass = new int[190];
                    startFlag = 0;

                    command = string.Format("select top 1* from device_statusCount where UpdateTime > '{0}' and UpdateTime <= '{1}' order by UpdateTime desc",
                             ls_check[ls_count].ToString("yyyy-MM-dd HH:mm:ss"), ls_check[ls_count + 1].ToString("yyyy-MM-dd HH:mm:ss"));
                    sr = sqlHelper.ExecuteReader(command);

                    if (sr.Read())
                    {

                        temp_sr = sr["GYRCount"].ToString();
                        //sw.WriteLine(temp_sr);//开始写入值


                        string[] arrGYR = temp_sr.Split(',');
                        temp_sr = sr["Activation"].ToString();
                        string[] arrAct = temp_sr.Split(',');

                        for (int i = 0; i < 10; i++)
                        {
                            for (int j = 0; j < 6; j++)
                            {
                                
                                arrGYTTimeClass[i * 19 + j] = int.Parse(arrGYR[i * 19 + j]) - arrGYTTimeClass[i * 19 + j];
                                arrGYTTimeClass[i * 19 + 6 + j] = int.Parse(arrGYR[i * 19 + j + 6]) - arrGYTTimeClass[i * 19 + 6 + j];
                                arrGYTTimeClass[i * 19 + 12 + j] = int.Parse(arrGYR[i * 19 + j + 12]) - arrGYTTimeClass[i * 19 + 12 + j];

                                GYRTimePerMechine[i * 19 + j] += arrGYTTimeClass[i * 19 + j];
                                GYRTimePerMechine[i * 19 + 6 + j] += arrGYTTimeClass[i * 19 + 6 + j];
                                GYRTimePerMechine[i * 19 + 12 + j] += arrGYTTimeClass[i * 19 + 12 + j];
                            }
                        }
                        bFlagUpdateData = true;
                    }

                   // sw.WriteLine("GYRTimePerMechine," + string.Join(",", GYRTimePerMechine));
                    

                    arrGYTTimeClass = new int[190];

                    ls_count += 1;
                }

                if (bFlagUpdateData)
                {

                    for (int i = 0; i < 10; i++)
                    {
                        tempMin = 999999;
                        for (int j = 0; j < 6; j++)
                        {
                            GYRTimePerMechine[i * 19 + j] = (int)(GYRTimePerMechine[i * 19 + j] / 60);
                            GYRTimePerMechine[i * 19 + 6 + j] = (int)(GYRTimePerMechine[i * 19 + 6 + j] / 60);
                            GYRTimePerMechine[i * 19 + 12 + j] = (int)(GYRTimePerMechine[i * 19 + 12 + j] / 60);

                            if (tempMin >= GYRTimePerMechine[i * 19 + j] && deviceStatus[i * 6 + j] != 8)
                            {

                                IndexOfMinGreen = j;
                                tempMin = GYRTimePerMechine[i * 19 + j];
                            }
                            arrActivationPerMechine[i * 6 + j] = (int)Math.Round(GYRTimePerMechine[i * 19 + j] * 100f / totalInterval);
                        }
                        GYRTimePerMechine[i * 19 + 18] = IndexOfMinGreen;
                        GYRTime[i * 3 + 0] = GYRTimePerMechine[i * 19 + GYRTimePerMechine[i * 19 + 18]];
                        GYRTime[i * 3 + 1] = GYRTimePerMechine[i * 19 + GYRTimePerMechine[i * 19 + 18] + 6];
                        GYRTime[i * 3 + 2] = GYRTimePerMechine[i * 19 + GYRTimePerMechine[i * 19 + 18] + 12];

                        arrActivation[i] = (int)Math.Round(GYRTime[i * 3 + 0] * 100 / totalInterval);

                        //act_value.Add((double)arrActivation[i]);
                        //y_value.Add((double)GYRTime[3 * i + 1]);
                        //r_value.Add((double)GYRTime[3 * i + 2]);
                        act_value[i] = (double)arrActivation[i];
                        y_value[i] = (double)GYRTime[3 * i + 1];
                        r_value[i] = (double)GYRTime[3 * i + 2];
                    }
                    //sw.WriteLine("GYRTimePerMechine," + string.Join(",", GYRTimePerMechine));

                    //sw.WriteLine("arrActivationPerMechine," + totalInterval.ToString() + ","+ string.Join(",", arrActivationPerMechine));
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        //for (int j = 0; j < 6; j++)
                        //{
                        //    GYRTimePerMechine[i * 19 + j] = 0;
                        //    GYRTimePerMechine[i * 19 + 6 + j] = 0;
                        //    GYRTimePerMechine[i * 19 + 12 + j] = 0;
                        //    arrActivationPerMechine[i * 6 + j] = 0;
                        //}
                        
                        //GYRTime[i * 3 + 0] = 0;
                        //GYRTime[i * 3 + 1] = 0;
                        //GYRTime[i * 3 + 2] = 0;
                        arrActivation[i] = 0;
                        //act_value.Add((double)arrActivation[i]);
                        //y_value.Add((double)GYRTime[3 * i + 1]);
                        //r_value.Add((double)GYRTime[3 * i + 2]);
                        act_value[i] = (double)arrActivation[i];
                        y_value[i] = (double)GYRTime[3 * i + 1];
                        r_value[i] = (double)GYRTime[3 * i + 2];
                    }
                    GYRTimePerMechine = new int[190];
                    GYRTime = new int[30];

                    MessageBox.Show("该时间段没有数据");
                }

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    chart_Activation.SeriesCollection_Activation.ElementAt(0).Values = act_value;
                    chart_YR.SeriesCollection.ElementAt(0).Values = y_value;
                    chart_YR.SeriesCollection.ElementAt(1).Values = r_value;

                    tb_activation.Text = "当前车间设备稼动率 (平均稼动率为：" + ((int)act_value.Average()).ToString() + "% )";
                    tb_yrtime.Text = "当前车间产线异常时间统计 (总计：等待时间 " + ((int)y_value.Sum()).ToString() + " 分钟，故障时间 " + ((int)r_value.Sum()).ToString() + " 分钟)";
                    tb_status.Text = "";
                }));

                GC.Collect();

            }
            catch (Exception err)
            {
                tb_status.Dispatcher.BeginInvoke(new Action(() => {
                    tb_status.Text = err.Message;
                }));
                //Console.WriteLine(err.Message);
            }

            //sw.Close();
            //fs.Close();

            timer_update.Start();
        }

        //private void CheckData()
        //{

        //    string temp_sr = "";

        //    for (int i = 0; i < 10; i++)
        //    {
        //        for (int j = 0; j < 6; j++)
        //        {
        //            GYRTimePerMechine[i * 19 + j] = 0;
        //            GYRTimePerMechine[i * 19 + 6 + j] = 0;
        //            GYRTimePerMechine[i * 19 + 12 + j] = 0;

        //            arrActivationPerMechine[i * 6 + j] = 0;

        //        }
        //        GYRTimePerMechine[i * 19 + 18] = 0;
        //        GYRTime[i * 3 + 0] = 0;
        //        GYRTime[i * 3 + 1] = 0;
        //        GYRTime[i * 3 + 2] = 0;

        //        arrActivation[i] = 0;
        //    }

        //    bFlagUpdateData = false;
        //    try
        //    {
        //        string command = string.Format("select * from device_statusCount where UpdateTime >= '{0}' and UpdateTime<='{1}' order by UpdateTime", start_time, end_time);
        //        SqlDataReader sr = sqlHelper.ExecuteReader(command);

        //        act_value = new ChartValues<double>();
        //        y_value = new ChartValues<double>();
        //        r_value = new ChartValues<double>();

        //        while (sr.Read())
        //        {
        //            temp_sr = sr["DeviceStatus"].ToString();
        //            string[] arrStatus = temp_sr.Split(',');

        //            //temp_sr = sr["GYRCount"].ToString();
        //            //string[] arrGYR = temp_sr.Split(',');
        //            //temp_sr = sr["Activation"].ToString();
        //            //string[] arrAct = temp_sr.Split(',');
        //            string UpdateTime = sr["UpdateTime"].ToString();
        //            DateTime dt_update = Convert.ToDateTime(UpdateTime);

        //            m_Interval = (int)Math.Abs(dt_update.Subtract(dt_updatetime).TotalMilliseconds);
        //            if (m_Interval > 10000)
        //                m_Interval = 1000;
        //            dt_updatetime = dt_update;
        //            for (int i = 0; i < 10; i++)
        //            {
        //                for (int j = 0; j < 6; j++)
        //                {
        //                    deviceStatus[i * 6 + j] = int.Parse(arrStatus[i * 6 + j]);

        //                    switch (deviceStatus[i * 6 + j]) {
        //                        case 4: GYRTimePerMechine[i * 19 + j] += m_Interval; break; 
        //                        case 2: GYRTimePerMechine[i * 19 + 6 + j] += m_Interval; break;
        //                        case 1: GYRTimePerMechine[i * 19 + 12 + j] += m_Interval; break;
        //                    }
        //                }
        //            }


        //            bFlagUpdateData = true;

        //        }
        //        int IndexOfMinGreen = 0;
        //        int tempMin = 999999;
        //        if (bFlagUpdateData)
        //        {

        //            for (int i = 0; i < 10; i++)
        //            {
        //                tempMin = 999999;
        //                for (int j = 0; j < 6; j++)
        //                {
        //                    GYRTimePerMechine[i * 19 + j] = (int)(GYRTimePerMechine[i * 19 + j] / 60000);
        //                    GYRTimePerMechine[i * 19 + 6 + j] = (int)(GYRTimePerMechine[i * 19 + 6 + j] / 60000);
        //                    GYRTimePerMechine[i * 19 + 12 + j] = (int)(GYRTimePerMechine[i * 19 + 12 + j] / 60000);

        //                    if (tempMin >= GYRTimePerMechine[i * 19 + j] && deviceStatus[i * 6 + j] != 8)
        //                    {

        //                        IndexOfMinGreen = j;
        //                        tempMin = GYRTimePerMechine[i * 19 + j];
        //                    }
        //                    arrActivationPerMechine[i * 6 + j] = (int)Math.Round(GYRTimePerMechine[i * 19 + j] * 100 / totalmins);

        //                }
        //                GYRTimePerMechine[i * 19 + 18] = IndexOfMinGreen;
        //                GYRTime[i * 3 + 0] = GYRTimePerMechine[i * 19 + GYRTimePerMechine[i * 19 + 18]];
        //                GYRTime[i * 3 + 1] = GYRTimePerMechine[i * 19 + GYRTimePerMechine[i * 19 + 18] + 6];
        //                GYRTime[i * 3 + 2] = GYRTimePerMechine[i * 19 + GYRTimePerMechine[i * 19 + 18] + 12];

        //                arrActivation[i] = (int)(GYRTime[i * 3 + 0] * 100 / totalmins);

        //                act_value.Add((double)arrActivation[i]);
        //                y_value.Add((double)GYRTime[3 * i + 1]);
        //                r_value.Add((double)GYRTime[3 * i + 2]);
        //            }


        //        }
        //        else
        //        {
        //            for (int i = 0; i < 10; i++)
        //            {
        //                for (int j = 0; j < 6; j++)
        //                {
        //                    GYRTimePerMechine[i * 19 + j] = 0;
        //                    GYRTimePerMechine[i * 19 + 6 + j] = 0;
        //                    GYRTimePerMechine[i * 19 + 12 + j] = 0;
        //                    arrActivationPerMechine[i * 6 + j] = 0;
        //                }
        //                GYRTime[i * 3 + 0] = 0;
        //                GYRTime[i * 3 + 1] = 0;
        //                GYRTime[i * 3 + 2] = 0;
        //                arrActivation[i] = 0;
        //                act_value.Add((double)arrActivation[i]);
        //                y_value.Add((double)GYRTime[3 * i + 1]);
        //                r_value.Add((double)GYRTime[3 * i + 2]);
        //            }
        //            MessageBox.Show("该时间段没有数据");
        //        }

        //        this.Dispatcher.BeginInvoke(new Action(() =>
        //        {
        //            chart_Activation.SeriesCollection_Activation.ElementAt(0).Values = act_value;
        //            chart_YR.SeriesCollection.ElementAt(0).Values = y_value;
        //            chart_YR.SeriesCollection.ElementAt(1).Values = r_value;
        //        }));

        //    }
        //    catch (Exception err) { Console.WriteLine(err.Message); }

        //    timer_update.Start();
        //}
    }
}
