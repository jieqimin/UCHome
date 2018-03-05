using System.Web;

namespace UCHome.ashx
{
    /// <summary>
    /// GetClassFamInfo 的摘要说明
    /// </summary>
    public class GetClassFamInfo : IHttpHandler
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