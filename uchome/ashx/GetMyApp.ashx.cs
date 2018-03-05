using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Web;
using UCHome.Model;
namespace UCHome.ashx
{
    /// <summary>
    /// GetResoucesByType 的摘要说明
    /// </summary>
    public class GetMyApp : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["ID"]))
                id = new Guid(context.Request["ID"]);
            Guid typeid = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["typeid"]))
                typeid = new Guid(context.Request["typeid"]);
            int top = 5;
            if (!string.IsNullOrEmpty(context.Request["pagesize"]))
                top = int.Parse(context.Request["pagesize"]);
            int pageindex = 1;
            if (!string.IsNullOrEmpty(context.Request["curpage"]))
                pageindex = int.Parse(context.Request["curpage"]);

            UCHomeEntities uc = new UCHomeEntities();
            List<UCHome_App_My> apps = uc.UCHome_App_My.Where(m => m.UserID == id && m.STATUS == "1").OrderBy(m => m.AppFrom).Skip(top * (pageindex - 1)).Take(top).ToList();
            string Newjson = "";
            string split = string.Empty;
            foreach (UCHome_App_My item in apps)
            {
                var ismy = item.AppFrom == Guid.Empty ? "true" : "false";
                Newjson += string.Format("{11}{{\"pkid\":\"{0}\",\"appname\":\"{1}\",\"appurl\":\"{2}\",\"apptarget\":\"{3}\",\"appicon\":\"{4}\",\"appcolor\":\"{5}\",\"appmemo\":\"{6}\",\"apphelpchmurl\":\"{7}\",\"ismy\":\"{8}\",\"downloads\":\"{9}\",\"appfrom\":\"{10}\"}}",
                    item.PKID, item.AppName, item.AppUrl, item.AppTarget, item.AppIcon, item.AppColor, item.memo, item.helpchmurl, ismy, item.Downloads, item.AppFrom, split);
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