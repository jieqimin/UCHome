using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Web;
using UCHome.Model;
namespace UCHome.ashx {
	/// <summary>
	/// GetResoucesByType 的摘要说明
	/// </summary>
	public class GetApplication : IHttpHandler {
		readonly UCHomeBasePage ucbase = new UCHomeBasePage();
		private UserInfo user {
			get {
				return UCHomeBasePage.RequestUser;
			}
		}
		private Guid loginId {
			get {
				return user.userid;
			}
		}
		public void ProcessRequest(HttpContext context) {
			Guid id = Guid.Empty;
			if (!string.IsNullOrEmpty(context.Request["ID"]))
				id = new Guid(context.Request["ID"]);
			Guid typeid = Guid.Empty;
			//if (!string.IsNullOrEmpty(context.Request["typeid"]))
			//	typeid = new Guid(context.Request["typeid"]);
			int top = 0;
			if (!string.IsNullOrEmpty(context.Request["pagesize"]))
				top = int.Parse(context.Request["pagesize"]);
			int pageindex = 0;
			if (!string.IsNullOrEmpty(context.Request["curpage"]))
				pageindex = int.Parse(context.Request["curpage"]);
			string funcrole = "TP";
			if (!string.IsNullOrEmpty(context.Request["funcrole"]))
				funcrole = context.Request["funcrole"];

			UCHomeEntities uc = new UCHomeEntities();
			//List<ApplicationInfo> applist = (from s in uc.UCHome_App_System
			//                                 join t in uc.UCHome_AppType_Relation on s.PKID equals t.AppID into app
			//                                 from appi in app.DefaultIfEmpty()
			//                                 where s.STATUS == "1" && s.AppFuncRole == funcrole && (typeid == Guid.Empty || (appi.TypeID == typeid))
			//                                 select new ApplicationInfo
			//                                 {
			//                                     sapp = s,
			//                                     ismy = true
			//                                 }).ToList();
			List<ApplicationInfo> applist = (from a in uc.UCHome_App_System.OrderBy(s => s.AppOrder)
											 join t in uc.UCHome_AppType_Relation on a.PKID equals t.AppID
											 join p in uc.UCHome_App_Type on t.TypeID equals p.PKID
											 join b in uc.UCHome_AppRole_Relation on a.PKID equals b.Appid
											 join c in uc.UCHome_UserRole_Relation.Where(x => x.UserId == loginId) on b.RoleId equals c.RoleId
											 join d in uc.UCHome_App_My.Where(m => m.UserID == loginId) on a.PKID equals d.AppFrom into app
											 from appi in app.DefaultIfEmpty<UCHome_App_My>()
											 where (a.STATUS == "1")
											 select new ApplicationInfo {
												 sapp = a,
												 typeid = t.TypeID,
												 typename = p.TypeName,
												 ismy = appi.UserID != null && appi.UserID == loginId
											 }).Distinct().OrderBy(p => p.sapp.AppOrder).ToList();



			int pcount = applist.Count();
			List<ApplicationInfo> apps = applist.ToList();
			if (top != 0 && pageindex != 0) {
				apps = applist.Skip(top * (pageindex - 1)).Take(top).ToList();
			}
			string Newjson = "";
			string split = string.Empty;
			foreach (ApplicationInfo item in apps) {
				Newjson += string.Format("{12}{{\"pkid\":\"{0}\",\"appname\":\"{1}\",\"appurl\":\"{2}\",\"apptarget\":\"{3}\",\"appicon\":\"{4}\",\"appcolor\":\"{5}\",\"appmemo\":\"{6}\",\"apphelpchmurl\":\"{7}\",\"ismy\":\"{8}\",\"downloads\":\"{9}\",\"typeid\":\"{10}\",\"typename\":\"{11}\"}}",
					item.sapp.PKID, item.sapp.AppName, item.sapp.AppUrl, item.sapp.AppTarget, item.sapp.AppIcon, item.sapp.AppColor, item.sapp.memo, item.sapp.helpchmurl, item.ismy, item.sapp.Downloads, item.typeid, item.typename, split);
				split = ",";
			}
			Newjson = string.Format("[{{\"returncount\":\"{0}\",\"appinfos\":[{1}]}}]", pcount, Newjson);
			context.Response.ContentType = "text/plain";
			context.Response.Write(Newjson);
		}

		public bool IsReusable {
			get {
				return false;
			}
		}
	}

	public class ApplicationInfo {
		public UCHome_App_System sapp;
		public bool ismy;
		public Guid typeid;
		public string typename;
	}
}

