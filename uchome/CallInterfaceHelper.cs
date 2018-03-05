using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;

namespace Wicresoft.CalendarMgt
{
    public class CallInterfaceHelper
    {
        public enum nodeType
        {
            User,
            Group,
            Role,
            Department,
            Other
        }

        public enum msgType
        {
            Web = 0,
            Mail = 1,
            SMS = 2,
            Lync = 3
        }

        /// <summary>
        /// 发消息（发给个人）
        /// </summary>
        /// <param name="names">用户username,用逗号隔开</param>
        /// <param name="title">标题</param>
        /// <param name="body">内容</param>
        /// <param name="msgType">消息发送类型，用逗号隔开。共4种，例如：Web,Lync,Mail,SMS</param>
        /// <param name="CurrentUserID">发消息的用户ID</param>
        public static void SendMessage(string names, string ApplierDisplayName, string title, string body, string msgType, Guid CurrentUserID)
        {
            string[] DisplayNameArray = ApplierDisplayName.Split(',');
            string[] nameArray = names.Split(',');
            string[] nodeArray = new string[nameArray.Length];
            for (int i = 0; i < nodeArray.Length; i++)
            {
                nodeArray[i] = nodeType.User.ToString();
            }
            string jsonstr = CallInterfaceHelper.ParseJSON(nodeArray, nameArray, DisplayNameArray, title, body, msgType, "admin");
            CallInterfaceHelper.CallInterfaceService("Interface.MSG.URL", CurrentUserID, jsonstr);
        }

        public static string ParseJSON(string[] nodeType, string[] name, string[] DisplayNameArray, string title, string body, string msgType, string account)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbTo = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i < nodeType.Length; i++)
            {
                if (i == 0)
                    sbTo.Append("{");
                else
                    sbTo.Append(",{");
                sbTo.AppendFormat("\"nodeType\":\"{0}\",\"name\":\"{1}\",\"displayName\":\"{2}\"", nodeType[i], name[i], DisplayNameArray[i]);
                sbTo.Append("}");
            }
            sb.AppendFormat("\"account\":\"{0}\",\"to\":[{1}],\"title\":\"{2}\",\"body\":\"{3}\",\"msgType\":\"{4}\"", account, sbTo.ToString(), title, body, msgType);
            sb.Append("}");
            return sb.ToString();
        }

        public static string CallInterfaceService(string urlName, Guid userid, string data)
        {
            string url = string.Format(System.Configuration.ConfigurationManager.AppSettings[urlName], userid);
            //string data = "pageSize=10&currentPage=1&startDate=2012-11-01&endDate=2012-11-30&title=请假申请&type=公文";
            string resultJson = string.Empty; ;
            //发起请求              ;
            string postData = "postdata=" + HttpUtility.UrlEncode(data);

            Uri uri = new Uri(url);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            webRequest.Timeout = 300000;
            byte[] paramBytes = Encoding.UTF8.GetBytes(postData);
            webRequest.ContentLength = paramBytes.Length;


            using (Stream requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(paramBytes, 0, paramBytes.Length);
            }
            //响应
            WebResponse webResponse = webRequest.GetResponse();
            using (StreamReader myStreamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
            {
                resultJson = myStreamReader.ReadToEnd();
            }

            return resultJson;
        }

    }
}
