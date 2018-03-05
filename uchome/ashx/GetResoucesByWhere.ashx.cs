using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Web;
using UCHome.Model;
namespace UCHome.ashx
{
    /// <summary>
    /// GetResoucesByType 的摘要说明
    /// </summary>
    public class GetResoucesByWhere : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            UCResourceEntities ucr = new UCResourceEntities();
            string useridGuid = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["userid"]))
                useridGuid = context.Request.QueryString["userid"];
            string w_sec = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["w_sec"]))
                w_sec = HttpUtility.UrlDecode(context.Request.QueryString["w_sec"]);
            string w_sub = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["w_sub"]))
                w_sub = HttpUtility.UrlDecode(context.Request.QueryString["w_sub"]);
            string w_grade = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["w_grade"]))
                w_grade = HttpUtility.UrlDecode(context.Request.QueryString["w_grade"]);
            string w_res = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["w_res"]))
                w_res = HttpUtility.UrlDecode(context.Request.QueryString["w_res"]);
            string w_doc = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString["w_doc"]))
                w_doc = HttpUtility.UrlDecode(context.Request.QueryString["w_doc"]);
            string s_order = "hits";
            if (!string.IsNullOrEmpty(context.Request.QueryString["s_order"]))
                s_order = context.Request.QueryString["s_order"];
            int top = 5;
            if (!string.IsNullOrEmpty(context.Request["pagesize"]))
                top = int.Parse(context.Request["pagesize"]);
            int pageindex = 1;
            if (!string.IsNullOrEmpty(context.Request["curpage"]))
                pageindex = int.Parse(context.Request["curpage"]);
            StringBuilder strwhere = new StringBuilder();
            if (string.IsNullOrEmpty(w_sec) || w_sec == "全部")            
            {
                w_sec = "";
                //strwhere.AppendFormat("it.Hits>=0");
            }
            if (string.IsNullOrEmpty(w_sub) || w_sub == "全部")
            {
                w_sub = ""; //strwhere.AppendFormat(" and it.Subject='{0}'", w_sub);
            }
            if (string.IsNullOrEmpty(w_grade) || w_grade == "全部")
            {
                w_grade = ""; //strwhere.AppendFormat(" and it.Grade='{0}'", w_grade);
            }
            if (string.IsNullOrEmpty(w_res) || w_res == "全部")
            {
                w_res = ""; //strwhere.AppendFormat(" and it.ResourceType='{0}'", w_res);
            }
            if (string.IsNullOrEmpty(w_doc) || w_doc == "全部")
            {
                w_doc = ""; //strwhere.AppendFormat(" and it.ResourceFormat like '%{0}%'", w_doc);
            }
         
            if (!string.IsNullOrEmpty(useridGuid))
            {
                strwhere.AppendFormat("it.CreatorId='{0}'", useridGuid);
            }
            else
            {
                strwhere.AppendFormat("1=1");
            }
            SqlParameter[] paras = new SqlParameter[11];
            paras[0] = new SqlParameter("@ordervalue", s_order);
            paras[1] = new SqlParameter("@pageindex", pageindex - 1);
            paras[2] = new SqlParameter("@pagesize", top);
            paras[3] = new SqlParameter("@strwhere", strwhere.ToString());
            paras[5] = new SqlParameter("@ResourceFormat", w_doc);
            paras[6] = new SqlParameter("@ResourceType", w_res);
            paras[7] = new SqlParameter("@schoolstage", w_sec);
            paras[8] = new SqlParameter("@subject", w_sub);
            paras[9] = new SqlParameter("@Grade", w_grade);
            paras[10] = new SqlParameter("@CreatorId", useridGuid);
            List<V_ResourceByLearn> resouces = ucr.ExecuteStoreQuery<V_ResourceByLearn>("exec proc_ResourceByPage @ordervalue,@pageindex,@pagesize,@strwhere,@learntype,@ResourceFormat,@ResourceType,@schoolstage,@subject,@Grade,@CreatorId",
                paras).ToList();
            
            //if (s_order == "hits")
            //    resouces = ucr.V_ResourceByLearn.Where(strwhere.ToString()).OrderByDescending(r => r.Hits).Skip(top * (pageindex - 1)).Take(top).ToList();
            //else
            //    resouces = ucr.V_ResourceByLearn.Where(strwhere.ToString()).OrderByDescending(r => r.CreateTime).Skip(top * (pageindex - 1)).Take(top).ToList();
            string Newjson = "";
            string split = string.Empty;

            SqlParameter[] parasclone = new SqlParameter[11];
            for (int i = 0, j = 11; i < j; i++)
            {
                parasclone[i] = (SqlParameter)((ICloneable)paras[i]).Clone();
            }

            int rcount = ucr.ExecuteStoreQuery<int>("exec proc_ResourceCountByPage @ordervalue,@pageindex,@pagesize,@strwhere,@learntype,@ResourceFormat,@ResourceType,@schoolstage,@subject,@Grade,@CreatorId",
               parasclone).ToList()[0];
            //int rcount = ucr.V_ResourceByLearn.Where(strwhere.ToString()).OrderByDescending(r => r.Hits).Count();
            foreach (V_ResourceByLearn item in resouces)
            {
                Newjson += string.Format("{11}{{\"pkid\":\"{0}\",\"name\":\"{1}\",\"createtime\":\"{2}\",\"creatorname\":\"{3}\",\"hits\":\"{4}\",\"learntype\":\"{5}\",\"schoolstage\":\"{6}\",\"grade\":\"{7}\",\"subject\":\"{8}\",\"ResourceFormat\":\"{9}\",\"ResourceType\":\"{10}\"}}",
                    item.id, item.Name, item.CreateTime, item.CreatorName, item.Hits, item.learntype, item.schoolstage, item.Grade, item.Subject, item.ResourceFormat, item.ResourceType, split);
                split = ",";
            }
            Newjson = string.Format("[{{\"returncount\":\"{0}\",\"resourceinfo\":[{1}]}}]", rcount, Newjson);
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