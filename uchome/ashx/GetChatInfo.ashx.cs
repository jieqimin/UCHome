using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;

namespace UCHome.ashx
{
    /// <summary>
    /// GetChatInfo 的摘要说明
    /// </summary>
    public class GetChatInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Guid id;
            if (!string.IsNullOrEmpty(context.Request.QueryString["ID"]))
                id = new Guid(context.Request.QueryString["ID"]);
            else
            {
                id = Guid.Empty;
            }
            string chattype = "self";
            if (!string.IsNullOrEmpty(context.Request.QueryString["chatType"]))
                chattype = context.Request.QueryString["chatType"];
            List<ChatInfo> chatlist = new List<ChatInfo>();
            switch (chattype)
            {
                case "self":
                    chatlist = GetChatInfoBySelf(id);
                    break;
                case "classtea":
                    chatlist = GetChatInfoByClassTea(id);
                    break;
                case "classstu":
                    chatlist = GetChatInfoByClassStu(id);
                    break;
            }

            string chatjson = "";
            string split = string.Empty;
            foreach (ChatInfo item in chatlist)
            {
                chatjson += string.Format("{2}{{\"chatuser\":\"{0}\",\"chatcontent\":\"{1}\"}}", item.ChatUser,
                    item.Chatcontent, split);
                split = ",";
            }
            chatjson = string.Format("[{0}]", chatjson);
            context.Response.ContentType = "text/plain";
            context.Response.Write(chatjson);
        }

        public List<ChatInfo> GetChatInfoBySelf(Guid uid)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<ChatInfo> chatlist = uc.UCHome_PersonNew.Where(u => u.AddUser == uid && u.UCType == "chat" && u.IsShow == 1).Select(u => new ChatInfo { ChatUser = uid, Chatcontent = u.Content }).ToList();
            return chatlist;
        }
        public List<ChatInfo> GetChatInfoByClassTea(Guid bjid)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<ChatInfo> chatlist = uc.View_Class_teaNews.Where(u => u.bjid == bjid && u.UCType == "chat" && u.IsShow == 1 && u.IsShare == "1" && u.IsAudit == 1).Select(u => new ChatInfo { ChatUser = u.AddUser, Chatcontent = u.Content }).ToList();
            return chatlist;
        }
        public List<ChatInfo> GetChatInfoByClassStu(Guid bjid)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<ChatInfo> chatlist = uc.View_Class_StuNews.Where(u => u.bjid == bjid && u.UCType == "chat" && u.IsShow == 1 && u.IsShare == "1" && u.IsAudit == 1).Select(u => new ChatInfo { ChatUser = u.AddUser, Chatcontent = u.Content }).ToList();
            return chatlist;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public class ChatInfo
        {
            public Guid? ChatUser { get; set; }
            public string Chatcontent { get; set; }
        }
    }
}