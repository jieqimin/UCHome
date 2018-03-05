using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Student.Information.Service;
using Student.Model;
using UCHome.Model;
using UCHome.Models;
using UCHome.BLL;

namespace UCHome.Controllers {
	public class TaskCenterController : Controller {
		//
		// GET: /TaskCenter/
		readonly UCHomeBasePage ucbase = new UCHomeBasePage();
		private readonly string http = System.Web.Configuration.WebConfigurationManager.AppSettings["APIHttp"];
		private static UserInfo user {
			get { return UCHomeBasePage.RequestUser; }
		}
		private Guid loginId {
			get {
				try {
					return user.userid;
				}
				catch (Exception) {

					return Guid.Empty;
				}

			}
		}

		public ActionResult Index() {
			ViewBag.Http = http;
			var u = new UCHome.Models.User();
			u.UserId = loginId;
			u.UserName = user.username;
			u.SchoolId = user.xxid;
			return View(u);
		}

		public ActionResult IndexScore() {
			ViewBag.Http = http;
			var u = new UCHome.Models.User();
			u.UserId = loginId;
			u.UserName = user.username;
			u.SchoolId = user.xxid;
			return View(u);
		}

		public ActionResult TaskIndex() {
			ViewBag.Http = http;
			var u = new UCHome.Models.User();
			u.UserId = loginId;
			u.UserName = user.username;
			u.SchoolId = user.xxid;
			return View(u);
		}
		public ActionResult TaskMan() {
			return View();
		}
		public PartialViewResult TaskManage() {

			UCHome_TaskBLL taskbll = new UCHome_TaskBLL();
			ViewBag.Records = taskbll.GetInfoRecords();
			return PartialView();
		}
		public PartialViewResult TaskCondition(Guid TaskID) {
			return PartialView(TaskID);
		}
		public PartialViewResult TaskTrigger(Guid TaskID) {
			return PartialView(TaskID);
		}
		public PartialViewResult TaskTarget(Guid TaskID) {
			return PartialView(TaskID);
		}
		public PartialViewResult TaskReward(Guid TaskID) {
			return PartialView(TaskID);
		}
		public PartialViewResult AddTask() {
			return PartialView();
		}
		[HttpGet]
		public JsonResult GetAllTask() {
			UCHome_TaskBLL taskbll = new UCHome_TaskBLL();
			var tasklist = new {
				data = taskbll.GetAllTask(),
				recordcount = taskbll.GetInfoRecords()
			};
			return Json(tasklist, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult GetTaskTrigger(Guid TaskID) {
			UCHome_TaskTriggerBLL UTTBLL = new UCHome_TaskTriggerBLL();
			var tasklist = new {
				data = UTTBLL.GetTaskTriggers(TaskID),
				recordcount = UTTBLL.GetInfoRecords(TaskID)
			};
			return Json(tasklist, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult GetTaskTarget(Guid TaskID) {
			UCHome_TaskTargetBLL UTTBLL = new UCHome_TaskTargetBLL();
			var tasklist = new {
				data = UTTBLL.GetTaskTargets(TaskID),
				recordcount = UTTBLL.GetInfoRecords(TaskID)
			};
			return Json(tasklist, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult GetTaskReward(Guid TaskID) {
			UCHome_TaskRewardBLL UTRBLL = new UCHome_TaskRewardBLL();
			var tasklist = new {
				data = UTRBLL.GetTaskRewards(TaskID),
				recordcount = UTRBLL.GetInfoRecords(TaskID)
			};
			return Json(tasklist, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult GetTaskConditioin(Guid TaskID) {
			UCHome_TaskConditionBLL UTCBLL = new UCHome_TaskConditionBLL();
			var tasklist = new {
				data = UTCBLL.GetTaskCondition(TaskID),
				recordcount = 1
			};
			return Json(tasklist, JsonRequestBehavior.AllowGet);
		}
	}
}
