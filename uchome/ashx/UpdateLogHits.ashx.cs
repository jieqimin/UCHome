using System;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// UpdateLogHits 的摘要说明
    /// </summary>
    public class UpdateLogHits : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["ID"]))
                id = new Guid(context.Request.QueryString["ID"]);

            UCHomeEntities uc = new UCHomeEntities();
            UCHome_PersonNew pn = uc.UCHome_PersonNew.SingleOrDefault(u => u.PKID == id);
            if (pn != null) pn.Hits = pn.Hits + 1;
            try
            {
                uc.SaveChanges();
                context.Response.ContentType = "text/plain";
                context.Response.Write(true);
            }
            catch (Exception)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write(false);
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