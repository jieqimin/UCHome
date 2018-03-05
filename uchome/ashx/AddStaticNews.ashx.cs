using ServiceModel;
using ServiceStack.Service;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// AddStaticNews 的摘要说明
    /// </summary>
    public class AddStaticNews : IHttpHandler
    {
         UCHomeBasePage page = new UCHomeBasePage();
        #region 属性
         private static UserInfo user
         {
             get { return UCHomeBasePage.RequestUser; }
         }
        private Guid userid
        {
            get { return user.userid; }
        }
        private string userName
        {
            get { return user.loginname; }
        }
        private Guid XXID
        {
            get { return user.xxid; }
        }
        private string XXMC
        {
            get { return user.xxmc; }
        }
        private string DisPlayName
        {
            get { return user.username; }
        }
        #endregion

        public void ProcessRequest(HttpContext context)
        { 
             string http = System.Web.Configuration.WebConfigurationManager.AppSettings["APIHttp"];

            IServiceClient client = new JsonServiceClient(http + "/SNSApi/");
           // IServiceClient client = new JsonServiceClient("http://education.istudy.sh.cn/SNSApi/");
            Guid id = Guid.Empty;
            string content = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["ID"]))
                id = new Guid(context.Request.QueryString["ID"]);
            if (!string.IsNullOrEmpty(context.Request.QueryString["Content"]))
            {
                content = HttpUtility.UrlDecode(context.Request.QueryString["Content"]);
            }
          
            
            UCHome_PersonNew pn=new UCHome_PersonNew();
            pn.AddUser = id;
            pn.DeployTime = DateTime.Now;
            pn.Content = content;
            pn.Hits = 0;
            pn.PKID = Guid.NewGuid();
            pn.Title = content;
            pn.UCType = "New";
            pn.IsShare = "9";
            pn.IsAudit = 0;
            pn.isTop = 0;
            pn.IsShow = 1;
            pn.flowers = 0;
            UCHomeEntities uc=new UCHomeEntities();
            string Newjson;
            try
            {
                uc.UCHome_PersonNew.AddObject(pn);
                uc.SaveChanges();
                UCHome_PersonNew pn2 = new UCHome_PersonNew();
                pn2.PKID = Guid.NewGuid();
                pn2.AddUser = pn.AddUser;
                pn2.UCType = "static";
                pn2.Title = "说说更新";
                pn2.Abstract = string.Format("发表了新说说【{0}】（{1}）", pn.Title, pn.DeployTime);
                pn2.Content = pn.Content;
                pn2.DeployTime = pn.DeployTime;
                pn2.IsShare = pn.IsShare;
                pn2.IsShow = 1;
                pn2.IsAudit = 0;
                pn2.WriteFrom = pn.UCType;
                pn2.flowers = 0;
                uc.UCHome_PersonNew.AddObject(pn2);
                uc.SaveChanges();
                Newjson = "true";
                AddSNSFeedEntry feed = new AddSNSFeedEntry
                {
                    ObjectType = "说说",
                    ObjectID = pn2.PKID.ToString(),
                    UID =  pn.AddUser.ToString(),
                    UName = DisPlayName,
                    School = XXMC,
                    Title = "说说更新",
                    Summary = pn.Content,
                    Date = DateTime.Now,
                    //URL = "http://www.baidu.com",
                    Like = 0,
                    Favorite = 0
                };
                client.Send<AddSNSFeedEntry>(feed);
            }
            catch (Exception ex)
            {
                Newjson = "false";
            }
           
          
            Newjson = string.Format("[{{ result:{0}}}]", Newjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newjson);
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