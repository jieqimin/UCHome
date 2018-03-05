using System;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// DelSay 的摘要说明
    /// </summary>
    public class DelPersonAlbum : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid pid = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["pid"]))
                pid = new Guid(context.Request["pid"]);
            string isshow = string.Empty;
            if (!string.IsNullOrEmpty(context.Request["isshow"]))
                isshow = context.Request["isshow"];
            //isshare
            string isshare = string.Empty;
            if (!string.IsNullOrEmpty(context.Request["isshare"]))
                isshare = context.Request["isshare"];
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_Album album = uc.UCHome_Album.SingleOrDefault(n => n.PKID == pid);
            if (album != null)
            {
                if (!string.IsNullOrEmpty(isshow) && uc.UCHome_Rel_AblumPhoto.Any(u => u.AlbumID == pid))
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("201");
                    context.Response.End();
                }
                else
                {
                    if (!string.IsNullOrEmpty(isshow))
                        album.IsShow = int.Parse(isshow);
                    if (!string.IsNullOrEmpty(isshare))
                        album.IsShare = isshare;
                    uc.SaveChanges();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("200");
                }
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("202");
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