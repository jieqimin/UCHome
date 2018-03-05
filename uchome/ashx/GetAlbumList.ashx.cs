using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.ashx
{
    /// <summary>
    /// 获取动态信息
    /// </summary>
    public class GetAlbumList : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["ID"]))
                id = new Guid(context.Request["ID"]);
            string isshare = "n";
            if (!string.IsNullOrEmpty(context.Request["isshare"]))
                isshare = context.Request["isshare"];
            IEnumerable<UCHome_Album> ablumlist = GetAlbumListBySelf(id, isshare);
            string Newjson = "";
            string split = string.Empty;
            foreach (UCHome_Album item in ablumlist)
            {
                Newjson += string.Format("{0}{{\"user\":\"{1}\",\"time\":\"{2}\",\"title\":\"{3}\",\"abstract\":\"{4}\",\"hits\":\"{5}\",\"isshare\":\"{6}\",\"pkid\":\"{7}\",\"themes\":\"{8}\",\"albumtype\":\"{9}\",\"coverimg\":\"{10}\"}}", split,
                    item.AddUser, item.CreateTime.ToShortDateString(), item.AlbumName, UCHome_BaseOper.EncodeBase64(item.AlbumAbstract), item.Hits, item.IsShare, item.PKID, item.AlbumThemes, item.AlbumType, item.CoverImg);
                split = ",";
            }
            Newjson = string.Format("[{0}]", Newjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newjson);
        }

        private IEnumerable<UCHome_Album> GetAlbumListBySelf(Guid uid, string isshare)
        {
            UCHomeEntities uc = new UCHomeEntities();
            IEnumerable<UCHome_Album> albums = uc.UCHome_Album.Where(u => u.AddUser == uid && u.IsShow == 1 && (isshare != "y" || u.IsShare != "0"));
            return albums;
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