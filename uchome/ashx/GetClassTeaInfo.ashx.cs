using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// GetClassTeaInfo 的摘要说明
    /// </summary>
    public class GetClassTeaInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid idGuid = Guid.Empty;
            Guid xxid = Guid.Empty;
            string xnxqid = string.Empty;
            int topn = 6;
            if (!string.IsNullOrEmpty(context.Request.QueryString["bjid"]))
                idGuid = new Guid(context.Request.QueryString["bjid"]);
            if (!string.IsNullOrEmpty(context.Request.QueryString["xxid"]))
                xxid = new Guid(context.Request.QueryString["xxid"]);
            if (!string.IsNullOrEmpty(context.Request.QueryString["xnxqid"]))
                xnxqid = context.Request.QueryString["xnxqid"];
            UCHomeEntities uc = new UCHomeEntities();
            var tealist =
                uc.View_Class_TeaInfo.Where(t => t.bjid == idGuid && t.xxid == xxid && t.xnxqid == xnxqid).Select(tea => tea.jsid).Distinct().Take(topn).ToList();
            List<View_Simple_TeaInfo> techlist =
                uc.View_Simple_TeaInfo.Where(tea => tealist.Contains(tea.jsid)).ToList();
            string techjson = "";
            string split = string.Empty;
            foreach (View_Simple_TeaInfo item in techlist)
            {
                techjson += string.Format("{4}{{\"jsid\":\"{0}\",\"xm\":\"{1}\",\"xb\":\"{2}\",\"zp\":\"{3}\"}}", item.jsid, item.XM, item.xbm,
                    item.zp, split);
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