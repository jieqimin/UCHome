using System.Web;

namespace UCHome.ashx
{
    /// <summary>
    /// GetTeacherTechDoc 的摘要说明
    /// </summary>
    public class GetTeacherTechDoc : IHttpHandler
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