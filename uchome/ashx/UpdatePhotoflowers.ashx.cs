using System;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// UpdateLogHits 的摘要说明
    /// </summary>
    public class UpdatePhotoflowers : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["pid"]))
                id = new Guid(context.Request.QueryString["pid"]);

            UCHomeEntities uc = new UCHomeEntities();
            UCHome_Photo pn = uc.UCHome_Photo.SingleOrDefault(u => u.PKID == id);
            if (pn != null) pn.flowers = pn.flowers + 1;
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