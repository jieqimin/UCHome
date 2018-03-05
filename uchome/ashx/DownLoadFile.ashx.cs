
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using OA.Model;



namespace UCHome.ashx
{
    /// <summary>
    /// DownLoadFile 的摘要说明
    /// </summary>
    public class DownLoadFile : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Request.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            context.Response.Charset = "utf-8";
            context.Response.CacheControl = "no-cache";

     
            string filePath =System.Web.HttpContext.Current.Server.MapPath(context.Request.QueryString["FileUrl"]);//路径



            if (filePath != null)
            {
               

                //以字符流的形式下载文件
                FileStream fs = null;
                fs = new FileStream(filePath, FileMode.Open);

                byte[] bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                context.Response.ContentType = "application/octet-stream";
                //通知浏览器下载文件而不是打开
                context.Response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(filePath, System.Text.Encoding.UTF8));
                context.Response.BinaryWrite(bytes);
                context.Response.Charset = "UTF8";
                context.Response.Flush();
                context.Response.End();

                context.Response.Write("1");
            }
            else
            {
                context.Response.Write("<script type=\"text/javascript\">alert('文件不存在，下载失败！');</script>");
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