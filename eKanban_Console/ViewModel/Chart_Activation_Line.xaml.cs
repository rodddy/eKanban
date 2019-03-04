using System;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;

namespace eKanban_Console
{
    public partial class Chart_Activation_Line : UserControl
    {
        public Chart_Activation_Line()
        {
            InitializeComponent();

            SeriesCollection_Activation = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "稼动率",
                    Fill =new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)),
                    MaxColumnWidth = 35,
                    DataLabels=true,
                    FontSize=16,
                    LabelPoint = point => point.Y.ToString(),
                    Values = new ChartValues<double> { 10, 11, 10, 11, 10, 11 }

                }
            };

            Labels = new[] {"PRINTER", "SPI", "MT 1", "MT 2", "MT 3", "AOI" };
            Formatter = value => value.ToString("F0");

            DataContext = this;
        }

        public SeriesCollection SeriesCollection_Activation { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

    }
}
