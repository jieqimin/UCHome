using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using UCHome.Controllers;
using System.Threading.Tasks;
namespace UCHome.Hubs
{
    [HubName("chat")]
    public class ChatHub : Hub
    {
        static List<ChatUser> ConnectedUsers = new List<ChatUser>();
        [HubMethodName("send")]
        public void SendMessage(string name, string message, string roomname)
        {
            //string userName = Clients.Caller.userName;
            //Groups.Add(Context.ConnectionId, roomname);
            // Call the addNewMessageToPage method to update clients.
            Clients.Group(roomname).addNewMessageToPage(name, message);
        }

        public Task joinroom(string roomname)
        {
            string connectionid = Context.ConnectionId;
            string username = Clients.Caller.userName;
            Groups.Add(Context.ConnectionId, roomname);
            Clients.Group(roomname).connectionuser(connectionid, username);
            return Clients.Group(roomname).connectionuser(connectionid, username);
        }
        public Task leaveroom(string roomname)
        {
            string connectionid = Context.ConnectionId;
            string username = Clients.Caller.userName;
            Groups.Remove(Context.ConnectionId, roomname);
            return Clients.Group(roomname).disconnectuser(connectionid, username);
        }
        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopcalled)
        {
            return base.OnDisconnected(stopcalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
    }

    public class ChatUser
    {
        public Guid xxid { get; set; }
        public string xxmc { get; set; }
        public Guid userid { get; set; }
        public string username { get; set; }
        public string ConnectionId { get; set; }
        public string usertype { get; set; }
        public string status { get; set; }
    }
}