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
    public class GetLogCount : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["ID"]))
                id = new Guid(context.Request["ID"]);
            string uctype = "new";
            if (!string.IsNullOrEmpty(context.Request["uctype"]))
                uctype = context.Request["uctype"];
            string isshare = "n";
            if (!string.IsNullOrEmpty(context.Request["isshare"]))
                isshare = context.Request["isshare"];
            int resultcount = GetNewInfoBySelf(id, uctype, isshare);
            string Newjson = "";
            Newjson = string.Format("[{{\"recordcount\":\"{0}\"}}]", resultcount);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newjson);
        }

        private int GetNewInfoBySelf(Guid uid, string uctype, string isshare)
        {
            UCHomeEntities uc = new UCHomeEntities();
            int pn;
            if (uctype != "draftboxlog")
            {
                pn =
                    uc.UCHome_PersonNew.Count(
                        u =>
                            u.AddUser == uid && u.UCType == uctype && u.IsShow == 1 &&
                            (isshare != "y" || u.IsShare != "0"));

            }
            else
            {
                pn =
    uc.UCHome_PersonNew.Count(u => u.AddUser == uid && (u.UCType == "log" || u.UCType == "article") && u.IsShow == 0 && (isshare != "y" || u.IsShare != "0"));
            }
            return pn;
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