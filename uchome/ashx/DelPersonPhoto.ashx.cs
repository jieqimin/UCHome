using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// DelSay 的摘要说明
    /// </summary>
    public class DelPersonPhoto : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid pid = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["pid"]))
                pid = new Guid(context.Request["pid"]);
            string isshow = string.Empty;
            if (!string.IsNullOrEmpty(context.Request["isshow"]))
                isshow = context.Request["isshow"];
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_Photo photo = uc.UCHome_Photo.SingleOrDefault(n => n.PKID == pid);
            if (photo != null)
            {
                if (!string.IsNullOrEmpty(isshow))
                    photo.IsShow = int.Parse(isshow);
                uc.SaveChanges();
                List<UCHome_Rel_AblumPhoto> relAblumPhotos = uc.UCHome_Rel_AblumPhoto.Where(r => r.PhotoID == pid).ToList();
                List<Guid> pkids = relAblumPhotos.Select(a => a.AlbumID).Distinct().ToList();
                List<UCHome_Album> albums = uc.UCHome_Album.Where(a => pkids.Contains(a.PKID)).ToList();
                foreach (UCHome_Rel_AblumPhoto ucHomeRelAblumPhoto in relAblumPhotos)
                {
                    if (ucHomeRelAblumPhoto.IsCover == 1)
                    {
                        UCHome_Album album = albums.SingleOrDefault(a => a.PKID == ucHomeRelAblumPhoto.AlbumID);
                        if (album != null)
                        {
                            album.CoverImg = "";
                            uc.SaveChanges();
                        }
                    }
                    uc.DeleteObject(ucHomeRelAblumPhoto);
                    uc.SaveChanges();
                }
                context.Response.ContentType = "text/plain";
                context.Response.Write("200");
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