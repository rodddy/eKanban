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
using System.Configuration;
using System.Text.RegularExpressions;

namespace eKanban
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : Window
    {
        private int Delaysetting = 4;
        public Settings()
        {
            InitializeComponent();
            try {
                Delaysetting = int.Parse(ConfigurationManager.AppSettings["Delaysetting"].ToString());

            }
            catch (Exception err) { Console.WriteLine(err.ToString()); }
        }

        private void btn_settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Regex.IsMatch(txt_delay.Text, @"^[0-9]{1,3}$"))
                    Delaysetting = int.Parse(txt_delay.Text);
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings["Delaysetting"].Value = Delaysetting.ToString();
                
                cfa.Save();
                //ConfigurationManager.AppSettings.Set("Delaysetting", Delaysetting.ToString());
                //ConfigurationManager.AppSettings.Add("sets", Delaysetting.ToString());
                this.Close();
                MainWindow.frm.setDelay(Delaysetting);
            }
            catch (Exception err) { Console.WriteLine(err.ToString()); }
        }

        private void ScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
           
            Delaysetting = (int)scroll_value.Value;
            txt_delay.Text = Delaysetting.ToString();
        }

        private void txt_delay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                if (Regex.IsMatch(txt_delay.Text, @"^[0-9]{1,3}$"))
                    scroll_value.Value = int.Parse(txt_delay.Text);
            }
        }
    }
}
