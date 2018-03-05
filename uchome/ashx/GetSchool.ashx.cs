using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// 获取动态信息
    /// </summary>
    public class GetSchool : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string key = "";
            if (!string.IsNullOrEmpty(context.Request["key"]))
                key = context.Request["key"];
            string xzqhm = "370901";
            if (!string.IsNullOrEmpty(context.Request["xzqhm"]))
                xzqhm = context.Request["xzqhm"];
            string schoolType = "1";
            if (!string.IsNullOrEmpty(context.Request["schoolType"]))
                schoolType = context.Request["schoolType"];
            int top = 5;
            if (!string.IsNullOrEmpty(context.Request["pagesize"]))
                top = int.Parse(context.Request["pagesize"]);
            int pageindex = 1;
            if (!string.IsNullOrEmpty(context.Request["curpage"]))
                pageindex = int.Parse(context.Request["curpage"]);
            IEnumerable<SchoolInfo> list = GetSchoolList(key, xzqhm, schoolType, top, pageindex);
            string newjson = JsonConvert.SerializeObject(list);
            context.Response.ContentType = "text/plain";
            context.Response.Write(newjson);
        }

        private IEnumerable<SchoolInfo> GetSchoolList(string key, string xzqhm, string schoolType, int topcount, int pageindex)
        {
            UCHomeEntities uc = new UCHomeEntities();
            var list =
                uc.Basic_ZZXX0101.Where(c => c.XZQHM == xzqhm && c.SchoolType == schoolType && c.XXMC.Contains(key))
                    .OrderBy(c => c.XXMC)
                    .Skip(topcount * (pageindex - 1))
                    .Take(topcount)
                    .AsEnumerable()
                    .Select(c => new SchoolInfo(c.XXID,c.XXMC,c.XXDZ,c.CLBJ))
                    .ToList();
            return list;
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