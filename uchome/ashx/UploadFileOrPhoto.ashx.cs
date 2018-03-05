using System.Web;

namespace UCHome.ashx
{
    /// <summary>
    /// UploadFileOrPhoto 的摘要说明
    /// </summary>
    public class UploadFileOrPhoto : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            HttpPostedFile imgfiles = context.Request.Files["file"];
           
            if (imgfiles != null)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("test");
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("{statusText:\"error\"}");
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