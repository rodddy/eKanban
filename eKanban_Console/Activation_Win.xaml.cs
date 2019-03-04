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
    /// Activation_Win.xaml 的交互逻辑
    /// </summary>
    public partial class Activation_Win : Window
    {
        public Activation_Win()
        {
            InitializeComponent();
        }
        public void setParams(string title,int[] Status,int[] data)
        {
            tb_line.Text = title + "设备稼动率";
            ChartValues<double> act_values = new ChartValues<double>();
            List<string> ls_labels = new List<string>(); ;
            for (int i = 0; i < data.Count(); i++) {
                if (Status[i] != 8) {
                    act_values.Add((double)data[i]);
                    ls_labels.Add(DeviceInfo.Device_Set.ElementAt(i));
                }
            }
            chart_Activation_Line.SeriesCollection_Activation.ElementAt(0).Values = act_values;
            chart_Activation_Line.Labels = ls_labels.ToArray();
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
