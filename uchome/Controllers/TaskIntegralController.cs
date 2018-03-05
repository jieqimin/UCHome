using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ServiceStack.Text;
//using TaskIntegral.Bll;
//using TaskIntegral.Model;
using UCHome.Model;
using UCHome.Models;
/// <summary>
/// 任务管理
/// </summary>
namespace UCHome.Controllers {
	public class TaskIntegralController : Controller {
		readonly UCHomeBasePage ucbase = new UCHomeBasePage();
		readonly UCHome.BLL.UCHome_TaskBLL taskbll = new BLL.UCHome_TaskBLL();
		public ActionResult Index() {
			return View();
		}
		private UserInfo user {
			get { return UCHomeBasePage.RequestUser; }
		}
		private Guid userid {
			get { return user.userid; }
		}
		private Guid xxid {
			get { return user.xxid; }
		}
		public JsonResult AddTask(UCHome.Model.UCHome_Task postData) {
			try {
				if (postData.PKID == Guid.Empty) {
					postData.PKID = Guid.NewGuid();
				}
				bool result = taskbll.AddTask(postData);
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e) {
				var result = new {
					flag = false,
					message = e.InnerException.ToString()
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		public class userResOrder {
			public Guid Id { get; set; }
			public string Name { get; set; }
			public string UserName { get; set; }
			public long ResCount { get; set; }
			public long SchoolOrder { get; set; }
			public long AreaOrder { get; set; }
			public long CityOrder { get; set; }
		}

		public class userWeekResOrder {
			public Guid UserId { get; set; }
			public string Count { get; set; }

		}

		public class userSysResOrder {
			public Guid CreatorId { get; set; }
			public string Count { get; set; }
			public string Order { get; set; }
		}

	}

}