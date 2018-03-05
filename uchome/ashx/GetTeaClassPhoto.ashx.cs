using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// GetTeaClassPhoto 的摘要说明
    /// </summary>
    public class GetTeaClassPhoto : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["ID"]))
                id = new Guid(context.Request.QueryString["ID"]);
            string Newtype = "photo";
            if (!string.IsNullOrEmpty(context.Request.QueryString["NewType"]))
                Newtype = context.Request.QueryString["NewType"];
            List<PhotoTeaInfo> Photolist = new List<PhotoTeaInfo>();
            switch (Newtype)
            {

                case "photo":
                    Photolist = GetTeaPhotoInfoByClass(id);
                    break;
                default:

                    break;
            }

            string Newjson = "";
            string split = string.Empty;
            foreach (PhotoTeaInfo item in Photolist)
            {
                Newjson += string.Format("{0}{{\"newsuser\":\"{1}\",\"photourl\":\"{2}\",\"createdate\":\"{3}\",\"bjid\":\"{4}\",\"pkid\":\"{5}\",\"isshow\":\"{6}\"}}", split, item.NewUser,
                   item.PhotoUrl, item.CreateTime.ToShortDateString(), item.bjid, item.pkid, item.IsShow);
                split = ",";
            }
            Newjson = string.Format("[{0}]", Newjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newjson);
        }
        public List<PhotoTeaInfo> GetTeaPhotoInfoByClass(Guid bjid)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<PhotoTeaInfo> Newlist =
                uc.View_Class_teaPhotos.Where(
                    u => u.bjid == bjid)
                    .Select(u => new PhotoTeaInfo { NewUser = u.AddUser, bjid = u.bjid, PhotoUrl = u.PhotoUrl, CreateTime = u.CreateTime, pkid = u.PKID, IsShow = u.IsShow }).OrderBy(n => n.CreateTime).Take(16)
                    .ToList();
            return Newlist;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
    public class PhotoTeaInfo
    {
        public Guid? NewUser { get; set; }
        public Guid? bjid { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid pkid { get; set; }
        public int IsShow { get; set; }
    }
}