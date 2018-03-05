using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.ashx
{
    /// <summary>
    /// 获取动态信息
    /// </summary>
    public class GetStaticnews : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["ID"]))
                id = new Guid(context.Request.QueryString["ID"]);
            int top = 5;
            if (!string.IsNullOrEmpty(context.Request.QueryString["topcount"]))
                top = int.Parse(context.Request.QueryString["topcount"]);
            string Newtype = "self";
            if (!string.IsNullOrEmpty(context.Request.QueryString["NewType"]))
                Newtype = context.Request.QueryString["NewType"];
            List<NewInfo> Newlist;
            switch (Newtype)
            {
                case "self":
                    Newlist = GetNewInfoBySelf(id, top);
                    break;
                case "classtea":
                    Newlist = GetNewInfoByClassTea(id, top);
                    break;
                case "classstu":
                    Newlist = GetNewInfoByClassStu(id, top);
                    break;
                case "classes":
                    Newlist = GetArtileInfoByClassTea(id, top);

                    break;
                case "hotclasses":
                    Newlist = GetHotInfoByClassTea(id, top);
                    break;
                default:
                    Newlist = GetNewInfoByClassTea(id, top);
                    Newlist.AddRange(GetNewInfoByClassStu(id, top));
                    break;
            }

            string Newjson = "";
            string split = string.Empty;
            foreach (NewInfo item in Newlist)
            {
                Newjson += string.Format("{0}{{\"newsuser\":\"{1}\",\"newscontent\":\"{2}\",\"newsdate\":\"{3}\",\"writefrom\":\"{4}\",\"pid\":\"{5}\",\"title\":\"{6}\"}}", split, item.NewUser,
                   UCHome_BaseOper.EncodeBase64(item.Newcontent), item.NewDate.ToShortDateString(), item.writefrom, item.pid, item.title);
                split = ",";
            }
            Newjson = string.Format("[{0}]", Newjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newjson);
        }

        public List<NewInfo> GetNewInfoBySelf(Guid uid, int topcount)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<NewInfo> Newlist =
                uc.UCHome_PersonNew.Where(u => u.AddUser == uid && u.UCType == "static" && u.IsShow == 1 && u.WriteFrom != "log" && u.IsShare != "0")
                    .Select(u => new NewInfo { NewUser = uid, Newcontent = u.Abstract, NewDate = u.DeployTime, writefrom = u.WriteFrom, pid = u.Content }).OrderByDescending(n => n.NewDate).Take(topcount)
                    .ToList();
            return Newlist;
        }

        public List<NewInfo> GetNewInfoByClassTea(Guid bjid, int topcount)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<NewInfo> Newlist =
                uc.View_Class_teaNews.Where(
                    u => u.bjid == bjid && u.UCType == "static" && u.IsShow == 1 && u.IsShare == "1" && u.IsAudit == 1)
                    .OrderByDescending(n => n.DeployTime).Take(topcount).ToList()
                    .Select(u => new NewInfo { NewUser = u.AddUser, Newcontent = u.Content, NewDate = u.DeployTime, writefrom = u.WriteFrom, pid = u.PKID.ToString() })
                    .ToList();
            return Newlist;
        }

        public List<NewInfo> GetNewInfoByClassStu(Guid bjid, int topcount)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<NewInfo> Newlist =
                uc.View_Class_StuNews.Where(
                    u => u.bjid == bjid && u.UCType == "static" && u.IsShow == 1 && u.IsShare == "1" && u.IsAudit == 1).OrderByDescending(n => n.DeployTime).Take(topcount).ToList()
                    .Select(u => new NewInfo { NewUser = u.AddUser, Newcontent = u.Content, NewDate = u.DeployTime, writefrom = u.WriteFrom, pid = u.PKID.ToString() })
                    .ToList();
            return Newlist;
        }

        public List<NewInfo> GetArtileInfoByClassTea(Guid bjid, int topcount)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<NewInfo> Newlist =
                uc.View_Class_teaandstuNews.Where(
                    u => u.bjid == bjid && u.IsShow == 1 && u.UCType == "article").OrderByDescending(n => n.DeployTime).Take(topcount).ToList()
                    .Select(u => new NewInfo { NewUser = u.AddUser, Newcontent = u.Content, NewDate = u.DeployTime, writefrom = u.WriteFrom, pid = u.PKID.ToString(), title = u.Title })
                    .ToList();
            return Newlist;
        }

        public List<NewInfo> GetHotInfoByClassTea(Guid bjid, int topcount)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<NewInfo> Newlist =
                uc.View_Class_teaandstuNews.Where(
                    u => u.bjid == bjid && u.IsShow == 1 && u.UCType == "article").ToList()
                    .Select(u => new NewInfo { NewUser = u.AddUser, Newcontent = u.Content, NewDate = u.DeployTime, writefrom = u.WriteFrom, pid = u.PKID.ToString(), hits = u.Hits == null ? 0 : u.Hits, title = u.Title }).OrderBy(n => n.hits).Take(topcount)
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

        public class NewInfo
        {
            public Guid? NewUser { get; set; }
            public string Newcontent { get; set; }
            public string title { get; set; }
            public DateTime NewDate { get; set; }
            public string writefrom { get; set; }
            public string pid { get; set; }
            public int? hits { get; set; }
        }
    }
}