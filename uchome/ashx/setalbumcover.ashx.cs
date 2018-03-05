using System;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// DelSay 的摘要说明
    /// </summary>
    public class setalbumcover : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            Guid pid = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["pid"]))
                pid = new Guid(context.Request["pid"]);
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_Photo photo = uc.UCHome_Photo.SingleOrDefault(n => n.PKID == pid);
            if (photo != null)
            {
                string purl = photo.PhotoUrl;
                photo.IsAblumCover = "1";
                uc.SaveChanges();
                UCHome_Rel_AblumPhoto relAblumPhoto = uc.UCHome_Rel_AblumPhoto.FirstOrDefault(r => r.PhotoID == pid);
                if (relAblumPhoto != null)
                {
                    Guid albumid = relAblumPhoto.AlbumID;
                    relAblumPhoto.IsCover = 1;
                    UCHome_Album album = uc.UCHome_Album.SingleOrDefault(p => p.PKID == albumid);
                    if (album != null)
                    {
                        album.CoverImg = purl;
                        uc.SaveChanges();
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("200");
                    }
                    else
                    {
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("201");
                    }

                }
                else
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("201");
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