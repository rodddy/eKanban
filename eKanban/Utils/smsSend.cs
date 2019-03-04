using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;

namespace eKanban
{
    class smsSend
    {

        static string apikey = "91c2bde41df974e97a768f5ae170231f";

        public static void smsPost(string DeviceId,string[] phonenum)
        {
            // 设置为您的apikey(https://www.yunpian.com)可查

            // 发送的手机号
            string mobile = "18550425629,18914065062,17706203191";
            // 发送模板内容
            if (phonenum.Count() < 1)
                return;
            else {
                for (int i = 0; i < phonenum.Count(); i++) { 
                    mobile += HttpUtility.UrlEncode(phonenum[i].ToString(), Encoding.UTF8) + ",";
                }
            }
            
            string tpl_value = HttpUtility.UrlEncode(
            HttpUtility.UrlEncode("#number#", Encoding.UTF8) + "=" +
            HttpUtility.UrlEncode(DeviceId.ToString(), Encoding.UTF8) + "&" +
            HttpUtility.UrlEncode("王计平", Encoding.UTF8), Encoding.UTF8);

            // 发送内容
            string text = "【王计平】A区域设备1234状态异常，请工作人员及时前往排查设备异常";

            // 智能模板发送短信url
            string url_send_sms = "https://sms.yunpian.com/v2/sms/batch_send.json";

            string data_send_sms = "apikey=" + apikey + "&mobile=" + mobile + "&text=" + text;

            HttpPost(url_send_sms, data_send_sms);

        }

        public static void HttpPost(string Url, string postDataStr)
        {
            byte[] dataArray = Encoding.UTF8.GetBytes(postDataStr);
            // Console.Write(Encoding.UTF8.GetString(dataArray));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = dataArray.Length;
            //request.CookieContainer = cookie;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader =
                    new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                String res = reader.ReadToEnd();
                reader.Close();
                Console.Write("\nResponse Content:\n" + res + "\n");
            }
            catch (WebException e)
            {
                Console.Write(e.Message + e.ToString());
                Stream stream = e.Response.GetResponseStream();

                StreamReader reader =
                    new StreamReader(stream, Encoding.UTF8);
                String res = reader.ReadToEnd();
                reader.Close();
                Console.Write("\nResponse Content:\n" + res + "\n");
            }
        }
    }
}
    
