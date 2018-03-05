using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.ashx
{
    /// <summary>
    /// 获取动态信息
    /// </summary>
    public class GetMessageById : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {

            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(context.Request["ID"]))
                id = new Guid(context.Request["ID"]);
            int top = 5;
            if (!string.IsNullOrEmpty(context.Request["pagesize"]))
                top = int.Parse(context.Request["pagesize"]);
            int pageindex = 1;
            if (!string.IsNullOrEmpty(context.Request["curpage"]))
                pageindex = int.Parse(context.Request["curpage"]);
            IEnumerable<View_UCHome_Leave> msglist = GetNewInfoBySelf(id, top, pageindex);
            string Newjson = "";
            string split = string.Empty;
            foreach (View_UCHome_Leave item in msglist)
            {
                Newjson += string.Format("{0}{{\"messageuser\":\"{1}\",\"time\":\"{2}\",\"contents\":\"{3}\",\"orders\":\"{4}\",\"msgtype\":\"{5}\",\"parentid\":\"{6}\",\"isshow\":\"{7}\",\"pkid\":\"{8}\",\"flowers\":\"{9}\",\"msgname\":\"{10}\",\"headimg\":\"{11}\"}}", split,
                    item.MessageUserID, item.EditDate.ToShortDateString(),UCHome_BaseOper.EncodeBase64(item.Contents), item.orders, item.msgtype, item.parentid, item.isshow, item.PKID, item.flowers, item.NickName, item.headphoto);
                split = ",";
            }
            Newjson = string.Format("[{0}]", Newjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(Newjson);
        }
        private IEnumerable<View_UCHome_Leave> GetNewInfoBySelf(Guid uid, int topcount, int pageindex)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<View_UCHome_Leave> pn =
                    uc.View_UCHome_Leave.Where(u => u.AcceptUserID == uid && u.msgtype != "master" && u.isshow == 1)
                        .OrderByDescending(u => u.EditDate)
                        .Skip(topcount * (pageindex - 1))
                        .Take(topcount)
                        .ToList();

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