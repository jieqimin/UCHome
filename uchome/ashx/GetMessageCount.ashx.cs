using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// 获取动态信息
    /// </summary>
    public class GetMessageCount : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {

            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["ID"]))
                id = new Guid(context.Request["ID"]);
            int recordcount = GetNewInfoBySelf(id);
            string Newjson = "";
            Newjson = string.Format("[{{\"recordcount\":\"{0}\"}}]", recordcount);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newjson);
        }
        public string EncodeBase64(string code_type, string code)
        {
            string encode;
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        private int GetNewInfoBySelf(Guid uid)
        {
            UCHomeEntities uc = new UCHomeEntities();
            int pn =
                uc.UCHome_Leave.Count(u => u.AcceptUserID == uid && u.msgtype != "master" && u.isshow == 1);                      
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