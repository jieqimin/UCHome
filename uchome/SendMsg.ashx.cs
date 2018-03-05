using System.Web;

namespace UCHome
{
    /// <summary>
    /// SendMsg 的摘要说明
    /// </summary>
    public class SendMsg : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string body=context.Request.Form["msg"];
       
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
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