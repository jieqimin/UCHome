using System;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// DelSay 的摘要说明
    /// </summary>
    public class DelPersonNew : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid pid = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["pid"]))
                pid = new Guid(context.Request["pid"]);
            string uctype = string.Empty;
            if (!string.IsNullOrEmpty(context.Request["uctype"]))
                uctype = context.Request["uctype"];
            //isshare
            string isshare = string.Empty;
            if (!string.IsNullOrEmpty(context.Request["isshare"]))
                isshare = context.Request["isshare"];
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_PersonNew uchome = uc.UCHome_PersonNew.SingleOrDefault(n => n.PKID == pid);
            if (uchome != null)
            {
                if (!string.IsNullOrEmpty(uctype))
                    uchome.UCType = uctype;
                if (!string.IsNullOrEmpty(isshare))
                    uchome.IsShare = isshare;
                uc.SaveChanges();
                context.Response.ContentType = "text/plain";
                context.Response.Write("200");
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("404");
            }

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