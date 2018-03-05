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
    public class GetArticleByXXID : IHttpHandler
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
            string uctype = "new";
            if (!string.IsNullOrEmpty(context.Request["uctype"]))
                uctype = context.Request["uctype"];
            IEnumerable<LogInfo> loglist = GetNewInfoBySelf(id, top, uctype, pageindex);
            string Newjson = "";
            string split = string.Empty;
            foreach (LogInfo item in loglist)
            {
                Newjson += string.Format("{0}{{\"user\":\"{1}\",\"time\":\"{2}\",\"title\":\"{3}\",\"abstract\":\"{4}\",\"hits\":\"{5}\",\"isshare\":\"{6}\",\"isaudit\":\"{7}\",\"isshow\":\"{8}\",\"pkid\":\"{9}\"}}", split,
                    item.loguser, item.logdate.ToShortDateString(), item.logtitle, item.logabstract, item.loghits, item.logisshare, item.logisaudit, item.logisshow, item.pkid);
                split = ",";
            }
            Newjson = string.Format("[{0}]", Newjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newjson);
        }

        private IEnumerable<LogInfo> GetNewInfoBySelf(Guid xxid, int topcount, string uctype, int pageindex)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<LogInfo> loglist = new List<LogInfo>();
            var pn = uc.UCHome_PersonNew.Join(
                uc.View_Simple_User,
                a=>a.AddUser,
                b=>b.userid,
                (a, b) => new { a.PKID, a.Title, a.Abstract, a.DeployTime,a.AddUser, a.Hits,a.UCType, a.IsAudit, a.IsShare, a.IsShow, b.xxid })
                .Where(u => u.xxid == xxid && u.UCType == uctype && u.IsShow==1 && u.IsAudit>1)
                .OrderByDescending(u => u.DeployTime).Skip(topcount * (pageindex - 1)).Take(topcount).ToList();
            foreach( var item in pn)
            {
                LogInfo log = new LogInfo();
                log.pkid = item.PKID;
                log.logtitle = item.Title;
                log.logabstract = item.Abstract;
                log.logdate = item.DeployTime;
                log.loghits = item.Hits;
                log.logisaudit = EnumConvert.ConvertAudit(item.IsAudit);
                log.logisshare = EnumConvert.ConvertShare(item.IsShare);
                log.logisshow = EnumConvert.ConvertShow(item.IsShow);
                log.loguser = item.AddUser;
                loglist.Add(log);
            }
            return loglist;
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private class LogInfo
        {
            public Guid pkid { get; set; }
            public string logtitle { get; set; }
            public Guid? loguser { get; set; }
            public string logabstract { get; set; }
            public DateTime logdate { get; set; }
            public int? loghits { get; set; }
            public string logisshare { get; set; }
            public string logisshow { get; set; }
            public string logisaudit { get; set; }
        }
    }
}