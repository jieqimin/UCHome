using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// GetSubjectBySection 的摘要说明
    /// </summary>
    public class GetSubjectBySection : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string section = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["section"]))
            {
                section = HttpUtility.UrlDecode(context.Request.QueryString["section"]);
            }
            if (!string.IsNullOrEmpty(section))
            {
                MetadataCenterEntities meta = new MetadataCenterEntities();
                string likevalue = string.Format("/{0}/", section);
                List<VIEW_METADATA_Property> vmp = meta.VIEW_METADATA_Property.Where(m => m.NamePath.StartsWith(likevalue) && m.tagname == "学科").OrderBy(m => m.SortNo).ToList();
                string results = string.Empty;
                string split = string.Empty;
                foreach (VIEW_METADATA_Property item in vmp)
                {
                    results += string.Format("{0}{{\"section\":\"{1}\",\"subname\":\"{2}\"}}", split,
                        section, item.DisplayName);
                    split = ",";
                }
                string jsonresult = string.Format("[{{\"statuscode\":\"200\",\"results\":[{0}]}}]", results);
                context.Response.ContentType = "text/plain";
                context.Response.Write(jsonresult);
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("[{\"statuscode\":\"500\",\"results\":\"网络数据异常\"}]");
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