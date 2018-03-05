using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Controllers;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// GetUserInfo 的摘要说明
    /// </summary>
    public class GetUserInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            UCHomeEntities uc = new UCHomeEntities();
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["ID"]))
                id = new Guid(context.Request["ID"]);
            TeacherAllInfo tai = new TeacherAllInfo();
            StudentAllInfo sai = new StudentAllInfo();
            View_Simple_TeaInfo tea = uc.View_Simple_TeaInfo.SingleOrDefault(u => u.jsid == id);
            View_Simple_StuInfo stu = uc.View_Simple_StuInfo.SingleOrDefault(u => u.xsid == id);
            if (tea != null)
            {
                tai.VST = tea;
                UCHome_BaseInfo ubi = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == id);
                if (ubi != null) tai.UBI = ubi; else tai.UBI = new UCHome_BaseInfo();
                string Newjson = "";
                string split = string.Empty;
                Newjson += string.Format("{0}{{\"nickname\":\"{1}\",\"username\":\"{2}\",\"xxmc\":\"{3}\",\"xbm\":\"{4}\",\"headphoto\":\"{5}\"}}", split,
                   ubi.NickName, tea.XM, tea.xxmc, tai.VST.xbm, ubi.headphoto);
                split = ",";
                Newjson = string.Format("[{0}]", Newjson);
                context.Response.ContentType = "text/plain";
                context.Response.Write(Newjson);
            }
            else if (stu != null)
            {
                if (stu != null) sai.VSS = stu; else sai.VSS = new View_Simple_StuInfo();
                UCHome_BaseInfo ubi = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == id);
                if (ubi != null) sai.UBI = ubi; else sai.UBI = new UCHome_BaseInfo();
                string Newjson = "";
                string split = string.Empty;
                Newjson += string.Format("{0}{{\"nickname\":\"{1}\",\"username\":\"{2}\",\"xxmc\":\"{3}\",\"xbm\":\"{4}\",\"headphoto\":\"{5}\"}}", split,
                   ubi.NickName, stu.XM, stu.XXMC, sai.VSS.XBM, ubi.headphoto);
                split = ",";
                Newjson = string.Format("[{0}]", Newjson);
                context.Response.ContentType = "text/plain";
                context.Response.Write(Newjson);
            }
            else
            {
                var Newjson = "当前登陆人信息没有传递！";
                context.Response.ContentType = "text/plain";
                context.Response.Write(Newjson);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
    public class TeacherAllInfo
    {
        public View_Simple_TeaInfo VST { get; set; }
        public UCHome_BaseInfo UBI { get; set; }
    }
}