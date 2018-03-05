using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// GetSchoolClass 的摘要说明
    /// </summary>
    public class GetSchoolClass : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var schoolid = Guid.Empty;// LoginSet.Consts.CurrentUser.CurrentDepartmentID;
            if (!string.IsNullOrEmpty(context.Request.QueryString["id"]))
            {
                schoolid = new Guid(context.Request.QueryString["id"]);
            }
            UCHomeEntities uc = new UCHomeEntities();
            List<View_Simple_SchoolClass> classlist = uc.View_Simple_SchoolClass.Where(u => u.xxid == schoolid).Take(10).ToList();
            string schclassjson = "";
            string split = string.Empty;
            foreach (View_Simple_SchoolClass item in classlist)
            {
                schclassjson += string.Format("{3}{{\"njmc\":\"{0}\",\"bjid\":\"{1}\",\"bjmc\":\"{2}\"}}", item.njmc, item.bjid, item.xzbmc,
                     split);
                split = ",";
            }
            schclassjson = string.Format("[{0}]", schclassjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(schclassjson);
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