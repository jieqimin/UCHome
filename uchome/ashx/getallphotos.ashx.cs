using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.ashx
{
    /// <summary>
    /// getallphotos 的摘要说明
    /// </summary>
    public class getallphotos : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strjson = string.Empty;
            Guid UserID = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["ID"]))
                UserID = new Guid(context.Request["ID"]);
            Guid albumid = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["albumid"]))
                albumid = new Guid(context.Request["albumid"]);
            string isshare = "n";
            if (!string.IsNullOrEmpty(context.Request["isshare"]))
                isshare = context.Request["isshare"];
            UCHomeEntities uc = new UCHomeEntities();


            List<UCHome_Photo> photos = uc.UCHome_Photo.Where(p => p.AddUser == UserID && p.IsShow == 1).OrderByDescending(p=>p.CreateTime).ToList();
            if (albumid != Guid.Empty)
            {
                List<Guid> relAblumPhotoIds =
                    uc.UCHome_Rel_AblumPhoto.Where(r => r.AlbumID == albumid).Select(p => p.PhotoID).ToList();
                photos = photos.Where(p => relAblumPhotoIds.Contains(p.PKID)).ToList();
            }
            //过滤不公开的相册
            if (isshare == "y")
            {
                List<Guid> albumids =
                    uc.UCHome_Album.Where(u => u.AddUser == UserID && u.IsShare != "0" && u.IsShow == 1)
                        .Select(u => u.PKID).ToList();
                List<Guid> sharephotoids =
                    uc.UCHome_Rel_AblumPhoto.Where(r => albumids.Contains(r.AlbumID)).Select(p => p.PhotoID).ToList();
                photos = photos.Where(p => sharephotoids.Contains(p.PKID)).ToList();
            }

            List<string> photodates = photos.Select(p => p.CreateTime.ToShortDateString()).Distinct().ToList();
            string split = string.Empty;
            for (int i = 0; i < photodates.Count; i++)
            {
                List<UCHome_Photo> childphotos =
                    photos.Where(p => p.CreateTime.ToShortDateString() == photodates[i]).ToList();
                string childphotojson = string.Empty;
                string childsplit = string.Empty;
                foreach (UCHome_Photo photoitem in childphotos)
                {
                    childphotojson += string.Format("{2}{{\"photoid\":\"{0}\",\"photourl1\":\"{1}\",\"photourl2\":\"{5}\",\"photoname\":\"{3}\",\"flowers\":\"{4}\"}}",
                        photoitem.PKID, UCHome_BaseOper.ConvertThumbnail(photoitem.PhotoUrl, "max"), childsplit, photoitem.PhotoName, photoitem.flowers, UCHome_BaseOper.ConvertThumbnail(photoitem.PhotoUrl, "thumbnail"));
                    childsplit = ",";
                }
                childphotojson = string.Format("[{0}]", childphotojson);
                strjson += string.Format("{2}{{\"photodate\":\"{0}\",\"photos\":{1}}}", DateTime.Parse(photodates[i]).ToString("yyyy年MM月dd日"), childphotojson, split);
                split = ",";
            }
            strjson = string.Format("[{0}]", strjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(strjson);
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