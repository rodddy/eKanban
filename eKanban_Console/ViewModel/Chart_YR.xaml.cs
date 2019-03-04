using System;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;

namespace eKanban_Console
{
    public partial class Chart_YR : UserControl
    {
        public Chart_YR()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "等待",
                    Fill =new SolidColorBrush(Color.FromArgb(255, 255, 185, 15)),
                    MaxColumnWidth = 35,
                    Values = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    DataLabels=true,
                    FontSize=16,
                    LabelPoint = point => point.Y.ToString()
                }
            };

            //adding series will update and animate the chart automatically
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "故障",
                Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                MaxColumnWidth = 35,
                DataLabels = true,
                FontSize = 16,
                LabelPoint = point => point.Y.ToString(),
                Values = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            });

            //also adding values updates and animates the chart automatically
            //SeriesCollection[1].Values.Add(48d);

            Labels = new[] { "SMT 09线", "SMT10线", "SMT11线", "SMT12线", "SMT13线", "SMT14线", "SMT15线", "SMT16线", "SMT17线", "SMT22线" };
            Formatter = value => value.ToString("F0");

            DataContext = this;
        }
        public void SaveImage(string filename)
        {
            //var viewbox = new Viewbox();
            //viewbox.Child = chart_export;
            //viewbox.Measure(chart_export.RenderSize);
            //viewbox.Arrange(new Rect(new Point(0, 0), chart_export.RenderSize));
            chart_export.Update(true, true); //force chart redraw
            //viewbox.UpdateLayout();

            SaveToPng(chart_export, filename);
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

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

    }
}
