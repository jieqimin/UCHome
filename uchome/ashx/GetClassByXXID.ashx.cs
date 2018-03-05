using System;
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
    public class GetClassByXXID : IHttpHandler
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
            IEnumerable<SClass> list = GetClassList(id, top, pageindex);
            string newjson = JsonConvert.SerializeObject(list);
            context.Response.ContentType = "text/plain";
            context.Response.Write(newjson);
        }

        private IEnumerable<SClass> GetClassList(Guid xxid, int topcount, int pageindex)
        {
            UCHomeEntities uc = new UCHomeEntities();
            string sql =
                "select bjid, xzbmc,year,njdm,xxid from view_school_classes where JGH='1' and xxid='" + xxid + "' order by njdm,xzbmc";
            var query = uc.ExecuteStoreQuery<TempClass>(sql);
            var list =
                query.OrderBy(c => c.njdm)
                    .ThenBy(c => c.xzbmc)
                    .Skip(topcount * (pageindex - 1))
                    .Take(topcount)
                    .AsEnumerable()
                    .Select(c => new SClass(c.bjid, c.xzbmc, c.year))
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