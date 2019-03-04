using System;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;

namespace eKanban_Console
{
    public partial class Chart_YR_Line : UserControl
    {
        public Chart_YR_Line()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "等待",
                    Fill =new SolidColorBrush(Color.FromArgb(255, 255, 185, 15)),
                    MaxColumnWidth = 30,
                    DataLabels=true,
                    FontSize=16,
                    LabelPoint = point => point.Y.ToString(),
                    Values = new ChartValues<double> { 10, 50, 39, 50, 48, 85 }
                    
                }
            };

            //adding series will update and animate the chart automatically
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "故障",
                Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                MaxColumnWidth = 30,
                DataLabels = true,
                FontSize = 16,
                LabelPoint = point => point.Y.ToString(),
                Values = new ChartValues<double> { 11, 56, 42, 67, 76, 54}
            });

            //also adding values updates and animates the chart automatically
            //SeriesCollection[1].Values.Add(48d);

            Labels = new[] { "PRINTER", "SPI", "MT 1", "MT 2", "MT 3", "AOI" };
            Formatter = value => value.ToString("F0");

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

    }
}
