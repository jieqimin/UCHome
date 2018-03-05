using Newtonsoft.Json;
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
    public class GetTeacherByXXID : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["ID"]))
                id = new Guid(context.Request["ID"]);
            int top = 5;
            if (!string.IsNullOrEmpty(context.Request["pagesize"]))
                top = int.Parse(context.Request["pagesize"]);
            int pageindex = 1;
            if (!string.IsNullOrEmpty(context.Request["curpage"]))
                pageindex = int.Parse(context.Request["curpage"]);
            string usertype = "t";
            if (!string.IsNullOrEmpty(context.Request["usertype"]))
                usertype = context.Request["usertype"];
            IEnumerable<UserInfo> list = GetUserInfoByXXID(id, top, usertype, pageindex);
            string newjson = JsonConvert.SerializeObject(list);
            context.Response.ContentType = "text/plain";
            context.Response.Write(newjson);
        }

        private IEnumerable<UserInfo> GetUserInfoByXXID(Guid xxid, int topcount, string usertype, int pageindex)
        {
            UCHomeEntities uc = new UCHomeEntities();
            var pn = uc.UCHome_BaseInfo.Join(
                uc.View_Simple_User,
                a => a.UserID,
                b => b.userid,
                (a, b) => new { a.UserID, a.UserType, a.CreateTime, a.headphoto,a.IsStatus, b.username, b.xxid })
                .Where(u => u.xxid == xxid && u.UserType == usertype && u.IsStatus == "1")
                .OrderByDescending(u => u.CreateTime)
                .Skip(topcount * (pageindex - 1))
                .Take(topcount)
                .AsEnumerable()
                .Select(c => new UserInfo(c.UserID, c.username, c.headphoto))
                .ToList();
            return pn;
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private class UserInfo
        {
            public Guid userId { get; set; }
            public string userName { get; set; }
            public string headphoto { get; set; }

            public UserInfo(Guid userid, string username, string headphoto)
            {
                userId = userid;
                userName = username;
                if (string.IsNullOrWhiteSpace(headphoto))
                {
                    this.headphoto = HttpContext.Current.Request.ApplicationPath + "/Content/NewFile/image/city/default.jpg";
                }
                else
                {
                    this.headphoto = HttpContext.Current.Request.ApplicationPath + headphoto;
                }
            }
        }
    }
}