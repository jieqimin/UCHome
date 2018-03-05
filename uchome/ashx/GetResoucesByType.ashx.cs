using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;
namespace UCHome.ashx
{
    /// <summary>
    /// GetResoucesByType 的摘要说明
    /// </summary>
    public class GetResoucesByType : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            
            UCResourceEntities ucr = new UCResourceEntities();
            string learntype = HttpUtility.UrlDecode(context.Request.QueryString["learntype"]);
            string top = context.Request.QueryString["top"];
            int _top = Convert.ToInt32(top);
            List<V_ResourceByLearn> resouces = ucr.V_ResourceByLearn.Where(r => r.learntype.Contains(learntype)).OrderByDescending(r => r.CreateTime).Take(_top).ToList();
            string Newjson = "";
            string split = string.Empty;
            foreach (V_ResourceByLearn item in resouces)
            {
                Newjson += string.Format("{3}{{\"rname\":\"{0}\",\"rdate\":\"{1}\",\"rid\":\"{2}\"}}", item.Name,
                    item.CreateTime, item.id, split);
                split = ",";
            }
            Newjson = string.Format("[{0}]", Newjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newjson);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}