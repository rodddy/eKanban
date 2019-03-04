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
    public partial class Chart_Activation : UserControl
    {
        public Chart_Activation()
        {
            InitializeComponent();

            SeriesCollection_Activation = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "稼动率",
                    Fill =new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)),
                    MaxColumnWidth = 30,
                    Values = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    DataLabels=true,
                    FontSize =16,
                    LabelPoint = point => point.Y.ToString()
                }
            };

            Labels = new[] {"SMT 09线", "SMT10线", "SMT11线", "SMT12线", "SMT13线", "SMT14线", "SMT15线", "SMT16线", "SMT17线", "SMT22线" };
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

        public SeriesCollection SeriesCollection_Activation { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

    }
}
