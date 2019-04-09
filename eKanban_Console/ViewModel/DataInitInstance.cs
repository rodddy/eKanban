using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eKanban_Console
{
    public class MaintainInfo{
        public int Department { get; set; }
        public string MaintainLog { get; set; }
        public string MaintainType { get; set; }
        public string MaintainUser { get; set; }
        public string MaintainTime { get; set; }
        public int Period { get; set; }
        

    }

    public class ContactInfo
    {
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactMail { get; set; }
        public string ContactMin { get; set; }
        public string ContactType { get; set; }
    }

    public class DeviceInfo {
        public string Name { set; get; }
        public string Catalog { set; get;}


        public static List<string> DepartmentList = new List<string>
        { "五车间", "六车间", "七车间", "八车间", "九车间" };

        public static List<string> Device_Set = new List<string>
        {"PRINTER","SPI","MT 1","MT 2","MT 3", "AOI" };

        public static List<DeviceInfo> Department_5 = new List<DeviceInfo>
        {
            new DeviceInfo { Name="SMT 09线",Catalog="五车间" },
            new DeviceInfo { Name="SMT 10线",Catalog="五车间" },
            new DeviceInfo { Name="SMT 11线",Catalog="五车间" },
            new DeviceInfo { Name="SMT 12线",Catalog="五车间" },
            new DeviceInfo { Name="SMT 13线",Catalog="五车间" },
            new DeviceInfo { Name="SMT 14线",Catalog="五车间" },
            new DeviceInfo { Name="SMT 15线",Catalog="五车间" },
            new DeviceInfo { Name="SMT 16线",Catalog="五车间" },
            new DeviceInfo { Name="SMT 17线",Catalog="五车间" },
            new DeviceInfo { Name="SMT 22线",Catalog="五车间" },
        };

        public static List<DeviceInfo> Department_6= new List<DeviceInfo>
        {
            new DeviceInfo { Name="SMT 01线",Catalog="六车间" },
            new DeviceInfo { Name="SMT 02线",Catalog="六车间" },
            new DeviceInfo { Name="SMT 03线",Catalog="六车间" },
            new DeviceInfo { Name="SMT 04线",Catalog="六车间" },
            new DeviceInfo { Name="SMT 05线",Catalog="六车间" },
            new DeviceInfo { Name="SMT 06线",Catalog="六车间" },
            new DeviceInfo { Name="SMT 07线",Catalog="六车间" },
            new DeviceInfo { Name="SMT 08线",Catalog="六车间" },
            new DeviceInfo { Name="SMT 09线",Catalog="六车间" },
            new DeviceInfo { Name="SMT 10线",Catalog="六车间" },
        };
        public static List<DeviceInfo> Department_7 = new List<DeviceInfo>
        {
            new DeviceInfo { Name="SMT 01线",Catalog="七车间" },
            new DeviceInfo { Name="SMT 02线",Catalog="七车间" },
            new DeviceInfo { Name="SMT 03线",Catalog="七车间" },
            new DeviceInfo { Name="SMT 04线",Catalog="七车间" },
            new DeviceInfo { Name="SMT 05线",Catalog="七车间" },
            new DeviceInfo { Name="SMT 06线",Catalog="七车间" },
            new DeviceInfo { Name="SMT 07线",Catalog="七车间" },
            new DeviceInfo { Name="SMT 08线",Catalog="七车间" },
            new DeviceInfo { Name="SMT 09线",Catalog="七车间" },
            new DeviceInfo { Name="SMT 10线",Catalog="七车间" },
        };
        public static List<DeviceInfo> Department_8 = new List<DeviceInfo>
        {
            new DeviceInfo { Name="SMT 01线",Catalog="八车间" },
            new DeviceInfo { Name="SMT 02线",Catalog="八车间" },
            new DeviceInfo { Name="SMT 03线",Catalog="八车间" },
            new DeviceInfo { Name="SMT 04线",Catalog="八车间" },
            new DeviceInfo { Name="SMT 05线",Catalog="八车间" },
            new DeviceInfo { Name="SMT 06线",Catalog="八车间" },
            new DeviceInfo { Name="SMT 07线",Catalog="八车间" },
            new DeviceInfo { Name="SMT 08线",Catalog="八车间" },
            new DeviceInfo { Name="SMT 09线",Catalog="八车间" },
            new DeviceInfo { Name="SMT 10线",Catalog="八车间" },
        };
    }


}
