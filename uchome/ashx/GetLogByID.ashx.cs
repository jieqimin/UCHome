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
    public class GetLogByID : IHttpHandler
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
            string isshare = "n";
            if (!string.IsNullOrEmpty(context.Request["isshare"]))
                isshare = context.Request["isshare"];
            IEnumerable<LogInfo> loglist = GetNewInfoBySelf(id, top, uctype, pageindex, isshare);
            string Newjson = "";
            string split = string.Empty;
            foreach (LogInfo item in loglist)
            {
                Newjson += string.Format("{0}{{\"user\":\"{1}\",\"time\":\"{2}\",\"title\":\"{3}\",\"abstract\":\"{4}\",\"hits\":\"{5}\",\"isshare\":\"{6}\",\"isaudit\":\"{7}\",\"isshow\":\"{8}\",\"pkid\":\"{9}\",\"flowers\":\"{10}\"}}", split,
                    item.loguser, item.logdate.ToShortDateString(), UCHome_BaseOper.EncodeBase64(item.logtitle),UCHome_BaseOper.EncodeBase64(item.logabstract) , item.loghits, item.logisshare, item.logisaudit, item.logisshow, item.pkid, item.flowers);
                split = ",";
            }
            Newjson = string.Format("[{0}]", Newjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newjson);
        }

        private IEnumerable<LogInfo> GetNewInfoBySelf(Guid uid, int topcount, string uctype, int pageindex, string isshare)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<LogInfo> loglist = new List<LogInfo>();
            List<UCHome_PersonNew> pn;
            if (uctype != "draftboxlog")
            {
                pn =
                    uc.UCHome_PersonNew.Where(u => u.AddUser == uid && u.UCType == uctype && u.IsShow == 1 && (isshare != "y" || u.IsShare != "0"))
                        .OrderByDescending(u => u.DeployTime)
                        .Skip(topcount * (pageindex - 1))
                        .Take(topcount)
                        .ToList();
            }
            else
            {
                pn =
    uc.UCHome_PersonNew.Where(u => u.AddUser == uid && (u.UCType == "log" || u.UCType == "article") && u.IsShow == 0 && (isshare != "y" || u.IsShare != "0"))
        .OrderByDescending(u => u.DeployTime)
        .Skip(topcount * (pageindex - 1))
        .Take(topcount)
        .ToList();
            }
            foreach (UCHome_PersonNew item in pn)
            {
                LogInfo log = new LogInfo();
                log.pkid = item.PKID;
                log.logtitle = item.Title;
                log.logabstract = item.Abstract;
                log.logdate = item.DeployTime;
                log.loghits = item.Hits;
                log.flowers = item.flowers;
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
            public int? flowers { get; set; }
            public string logisshare { get; set; }
            public string logisshow { get; set; }
            public string logisaudit { get; set; }
        }
    }
}