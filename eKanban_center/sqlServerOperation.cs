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

namespace eKanban_center
{
    public partial class MainWindow_Center : Window
    {


        public void insertIntoDB()
        {
            string sql = "insert into tb_device_status(Department, Line, UpdateTime) values(value1, value2)";

            sqlHelper.ExecuteNonQuery(sql);
                
        }

    }
}