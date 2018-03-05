using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// 获取动态信息
    /// </summary>
    public class GetClassNewInfo : IHttpHandler
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
            List<ClassNewInfo> Newlist;
            switch (Newtype)
            {
                case "classtea":
                    Newlist = GetNewInfoByClassTea(id, top);
                    break;
                case "classstu":
                    Newlist = GetNewInfoByClassStu(id, top);
                    break;
                default:
                    Newlist = GetNewInfoByClassTea(id, top);
                    Newlist.AddRange(GetNewInfoByClassStu(id, top));
                    break;
            }

            string Newjson = "";
            string split = string.Empty;
            foreach (ClassNewInfo item in Newlist)
            {
                Newjson += string.Format("{3}{{\"newuser\":\"{0}\",\"newcontent\":\"{1}\",\"newdate\":\"{2}\"}}", item.NewUser,
                    item.Newcontent, item.NewDate, split);
                split = ",";
            }
            Newjson = string.Format("[{0}]", Newjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newjson);
        }



        public List<ClassNewInfo> GetNewInfoByClassTea(Guid bjid, int topcount)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<ClassNewInfo> Newlist =
                uc.View_Class_teaNews.Where(
                    u => u.bjid == bjid && u.UCType == "New" && u.IsShow == 1 && u.IsShare == "1" && u.IsAudit == 1)
                    .Select(u => new ClassNewInfo { NewUser = u.xm+"老师", Newcontent = u.Content, NewDate = u.DeployTime }).Take(topcount)
                    .ToList();
            return Newlist;
        }

        public List<ClassNewInfo> GetNewInfoByClassStu(Guid bjid, int topcount)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<ClassNewInfo> Newlist =
                uc.View_Class_StuNews.Where(
                    u => u.bjid == bjid && u.UCType == "New" && u.IsShow == 1 && u.IsShare == "1" && u.IsAudit == 1)
                    .Select(u => new ClassNewInfo { NewUser = u.xm+"学生", Newcontent = u.Content, NewDate = u.DeployTime }).Take(topcount)
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

        public class ClassNewInfo
        {
            public string NewUser { get; set; }
            public string Newcontent { get; set; }
            public DateTime NewDate { get; set; }
        }
    }
}