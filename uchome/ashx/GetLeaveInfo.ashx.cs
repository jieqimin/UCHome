using System.Web;

namespace UCHome.ashx
{
    /// <summary>
    /// GetLeaveInfo 的摘要说明
    /// </summary>
    public class GetLeaveInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
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