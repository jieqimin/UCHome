using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;

namespace UCHome.Models
{
    public class APIHelper
    {
        public static string GetApiUrl(string key)
        {
            string url = "";
            XmlTextReader reader = new XmlTextReader(HttpContext.Current.Server.MapPath("~\\API.config"));
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            try
            {
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node.Attributes["key"].Value == key)
                    {
                        url = node.Attributes["url"].Value;
                        break;
                    }
                }
                return url;
            }
            catch (Exception)
            {
                return "读取配置文件出错";
            }
        }
        public static string GetApiUrl(string key, string configfile)
        {
            string url = "";
            XmlTextReader reader = new XmlTextReader(configfile);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            try
            {
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node.Attributes["key"].Value == key)
                    {
                        url = node.Attributes["url"].Value;
                        break;
                    }
                }
                return url;
            }
            catch (Exception)
            {
                return "读取配置文件出错";
            }
        }
    }
}