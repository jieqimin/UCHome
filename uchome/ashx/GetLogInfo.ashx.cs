using System;
using System.Linq;
using System.Web;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.ashx
{
    /// <summary>
    /// GetLogInfo 的摘要说明
    /// </summary>
    public class GetLogInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["pid"]))
                id = new Guid(context.Request.QueryString["pid"]);

            LogInfo item = GetLogInfoBySelf(id);
            string Logjson = string.Format("{0}{{\"user\":\"{1}\",\"time\":\"{2}\",\"title\":\"{3}\",\"abstract\":\"{4}\",\"hits\":\"{5}\",\"isshare\":\"{6}\",\"isaudit\":\"{7}\",\"isshow\":\"{8}\",\"pkid\":\"{9}\"}}", "",
                    item.loguser, item.logdate.ToShortDateString(), item.logtitle, UCHome_BaseOper.EncodeBase64(item.logabstract), item.loghits, item.logisshare, item.logisaudit, item.logisshow, item.pkid);
            Logjson = string.Format("[{0}]", Logjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Logjson);
        }

        private LogInfo GetLogInfoBySelf(Guid pid)
        {
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_PersonNew item = uc.UCHome_PersonNew.SingleOrDefault(u => u.PKID == pid);
            LogInfo log = new LogInfo();
            if (item != null)
            {
                log.pkid = item.PKID;
                log.logtitle = item.Title;
                log.logabstract = item.Abstract;
                log.logdate = item.DeployTime;
                log.loghits = item.Hits;
                log.logisaudit = EnumConvert.ConvertAudit(item.IsAudit);
                log.logisshare = EnumConvert.ConvertShare(item.IsShare);
                log.logisshow = EnumConvert.ConvertShow(item.IsShow);
                log.loguser = item.AddUser;
            }
            return log;
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