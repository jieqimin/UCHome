using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// GetAttention 的摘要说明
    /// </summary>
    public class GetAttention : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["ID"]))
                id = new Guid(context.Request["ID"]);
            UCHomeEntities uc = new UCHomeEntities();
            List<View_UCHome_Attention> attentionlist = uc.View_UCHome_Attention.Where(u => u.AddUser == id).ToList();
            string Newjson = "";
            string split = string.Empty;

            foreach (View_UCHome_Attention item in attentionlist)
            {
                Newjson += string.Format("{5}{{\"attenuser\":\"{0}\",\"attenname\":\"{1}\",\"attentime\":\"{2}\",\"identity\":\"{3}\",\"headphoto\":\"{4}\"}}", item.AttenUser,
                    item.AttenName, item.AttenTime.ToShortDateString(), item.AttenIdentity,item.headphoto, split);
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