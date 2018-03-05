using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.ashx
{
    /// <summary>
    /// GetAttention 的摘要说明
    /// </summary>
    public class GetDicussInfos : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["ID"]))
                id = new Guid(context.Request["ID"]);
            UCHomeEntities uc = new UCHomeEntities();
            string strid = id.ToString();
            List<View_UCHome_Dicuss> dicussInfos = uc.View_UCHome_Dicuss.Where(u => u.AddUser == id || u.groupuser.Contains(strid)).OrderByDescending(u => u.CreateDate).ToList();
            string Newjson = "";
            string split = string.Empty;
            foreach (View_UCHome_Dicuss item in dicussInfos)
            {
                Newjson += string.Format("{7}{{\"dicusstopic\":\"{0}\",\"abstracts\":\"{1}\",\"createdate\":\"{2}\",\"id\":\"{3}\",\"nickname\":\"{4}\",\"headphoto\":\"{5}\",\"usertype\":\"{6}\"}}", item.DicussTopic,
                   UCHome_BaseOper.EncodeBase64(item.Abstracts), item.CreateDate.ToShortDateString(), item.PKID, item.NickName, item.headphoto, item.UserType, split);
                split = ",";
            }
            Newjson = string.Format("[{0}]", Newjson);
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