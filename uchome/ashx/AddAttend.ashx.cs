using System;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// AddAttend 的摘要说明
    /// </summary>
    public class AddAttend : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid attenduser = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["attenduser"]))
                attenduser = new Guid(context.Request["attenduser"]);
            Guid adduser = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["adduser"]))
                adduser = new Guid(context.Request["adduser"]);
            UCHomeEntities uc = new UCHomeEntities();
            View_Simple_User vsu = uc.View_Simple_User.SingleOrDefault(u => u.userid == attenduser);
            if (vsu != null)
            {
                UCHome_Attention attent = new UCHome_Attention();
                attent.PKID = Guid.NewGuid();
                attent.AddUser = adduser;
                attent.AttenUser = vsu.userid;
                attent.AttenName = vsu.username; attent.AttenTime = DateTime.Now;
                uc.UCHome_Attention.AddObject(attent);
                uc.SaveChanges();
                context.Response.ContentType = "text/plain";
                context.Response.Write(true);
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write(false);
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
}