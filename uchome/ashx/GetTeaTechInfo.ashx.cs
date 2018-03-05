using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;
namespace UCHome.ashx
{
    /// <summary>
    /// GetTeaTechInfo 的摘要说明
    /// </summary>
    public class GetTeaTechInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid idGuid = Guid.Empty;
            string xnxq = "class";
            if (!string.IsNullOrEmpty(context.Request.QueryString["id"]))
                idGuid = new Guid(context.Request.QueryString["id"]);
            if (!string.IsNullOrEmpty(context.Request.QueryString["xnxqid"]))
                xnxq = context.Request.QueryString["xnxqid"];
            UCHomeEntities uc = new UCHomeEntities();
            List<View_Class_TeaInfo> techlist =
                uc.View_Class_TeaInfo.Where(t => t.jsid == idGuid && t.xnxqid == xnxq).ToList();
            string techjson = "";
            string split = string.Empty;
            foreach (View_Class_TeaInfo item in techlist)
            {
                techjson += string.Format("{5}{{\"teacher\":\"{0}\",\"kcmc\":\"{1}\",\"bjmc\":\"{2}\",\"xnxqid\":\"{3}\",\"bjid\":\"{4}\"}}", item.xm, item.kcmc, item.xzbmc,
                    item.xnxqid, item.bjid, split);
                split = ",";
            }
            techjson = string.Format("[{0}]", techjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(techjson);
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