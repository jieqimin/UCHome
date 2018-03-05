using System.Web;

namespace UCHome.ashx
{
    /// <summary>
    /// GetClassArtInfo 的摘要说明
    /// </summary>
    public class GetClassArtInfo : IHttpHandler
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