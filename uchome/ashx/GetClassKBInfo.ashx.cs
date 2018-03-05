using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// GetClassKBInfo 的摘要说明
    /// </summary>
    public class GetClassKBInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid idGuid = Guid.Empty;
            string uctype = "class";
            string xnxq = "";
            if (!string.IsNullOrEmpty(context.Request.QueryString["id"]))
                idGuid = new Guid(context.Request.QueryString["id"]);
            if (!string.IsNullOrEmpty(context.Request.QueryString["uctype"]))
                uctype = context.Request.QueryString["uctype"];
            if (!string.IsNullOrEmpty(context.Request.QueryString["xnxq"]))
                xnxq = context.Request.QueryString["xnxq"]; 
            List<View_KB_TeaInfo> kblist;
            if (uctype == "class")
            {
                kblist = GetKBInfoByClass(idGuid, xnxq);
            }
            else
            {
                kblist = GetKBInfoByTea(idGuid, xnxq);
            }
            string kbjson = "";
            string split = string.Empty;
            foreach (View_KB_TeaInfo item in kblist)
            {
                kbjson += string.Format("{5}{{\"teacher\":\"{0}\",\"kcmc\":\"{1}\",\"weekday\":\"{2}\",\"courseth\":\"{3}\",\"bjmc\":\"{4}\"}}", item.xm, item.kcmc, item.WeekDay,
                    item.ClassTh,item.xzbmc,split);
                split = ",";
            }
            kbjson = string.Format("[{0}]", kbjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(kbjson);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public List<View_KB_TeaInfo> GetKBInfoByClass(Guid bjid, string xnxq)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<View_KB_TeaInfo> kblist = uc.View_KB_TeaInfo.Where(u => u.bjid == bjid && u.xnxqid == xnxq).ToList();
            return kblist;
        }
        public List<View_KB_TeaInfo> GetKBInfoByTea(Guid jsid, string xnxq)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<View_KB_TeaInfo> kblist = uc.View_KB_TeaInfo.Where(u => u.jsid == jsid && u.xnxqid == xnxq).ToList();
            return kblist;
        }
    }
}