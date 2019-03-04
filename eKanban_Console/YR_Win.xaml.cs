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

using LiveCharts;

namespace eKanban_Console
{
    /// <summary>
    /// YR_Win.xaml 的交互逻辑
    /// </summary>
    public partial class YR_Win : Window
    {

        public int[] yr_array = new int[12];
        public YR_Win()
        {
            InitializeComponent();
        }

        public void setParams(string title,int[] Status, int[] YRValues)
        {
            tb_line.Text = title + "产线异常时间统计";
            List<string> ls_labels = new List<string>();
            ChartValues<double> y_values = new ChartValues<double>();
            ChartValues<double> r_values = new ChartValues<double>();

            for (int i = 0; i < 6; i++)
            {
                if (Status[i] != 8)
                {

                    ls_labels.Add(DeviceInfo.Device_Set.ElementAt(i));
                    y_values.Add(YRValues[i]);
                    r_values.Add(YRValues[i + 6]);
                }
            }

            chart_YR.SeriesCollection.ElementAt(0).Values = y_values;
            chart_YR.SeriesCollection.ElementAt(1).Values = r_values;
            chart_YR.Labels = ls_labels.ToArray();

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
    }
}
