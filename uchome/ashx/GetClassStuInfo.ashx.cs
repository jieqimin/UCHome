using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// GetClassStuInfo 的摘要说明
    /// </summary>
    public class GetClassStuInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid idGuid = Guid.Empty;
            Guid xxid = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["bjid"]))
                idGuid = new Guid(context.Request.QueryString["bjid"]);
            if (!string.IsNullOrEmpty(context.Request.QueryString["xxid"]))
                xxid = new Guid(context.Request.QueryString["xxid"]);
            UCHomeEntities uc = new UCHomeEntities();
            List<View_Simple_StuInfo> techlist =
                uc.View_Simple_StuInfo.Where(t => t.BJID == idGuid && t.xxid == xxid).ToList();
            string techjson = "";
            string split = string.Empty;
            foreach (View_Simple_StuInfo item in techlist)
            {
                techjson += string.Format("{4}{{\"xsid\":\"{0}\",\"xm\":\"{1}\",\"xb\":\"{2}\",\"zp\":\"{3}\"}}", item.xsid, item.XM, item.XBM,
                    item.ZP, split);
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