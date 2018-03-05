using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcoEdu.Framework.SingleSignOn;
using OA.Model;
using Student.Model;
using UCHome.BLL;
using UCHome.Model;

namespace UCHome.Controllers {
	public class SysManageController : Controller {
		//
		// GET: /SysManage/
		UCHome_AppBLL appbll = new UCHome_AppBLL();

		public ActionResult Index() {
			return View();
		}
		public ActionResult UserMan() {
			return View();
		}
		public ActionResult AppMan() {
			return View();
		}
		public ActionResult RoleMan() {
			return View();
		}
		public ActionResult FuncMan() {
			return View();
		}
		public ActionResult ParamsMan() {
			return View();
		}
		public ActionResult InterfaceMan() {
			return View();
		}
		public ActionResult ModalMan() {
			return View();
		}
		public ActionResult IconMan() {
			return View();
		}
		public ActionResult DataStat() {
			return View();
		}

		public ActionResult CharacterMan() {
			return View();
		}

		public PartialViewResult SetPermissions(string roletype) {
			return PartialView("SetPermissions", roletype);
		}

		public PartialViewResult sysmenu() {
			return PartialView();
		}
		public PartialViewResult Header() {
			return PartialView();
		}
		public PartialViewResult Appmanage() {
			UCHome_SkinBLL skinbll = new UCHome_SkinBLL();
			IEnumerable<UCHome_Skin> applist = skinbll.getsysskinlist();
			return PartialView(applist);
		}
		public PartialViewResult Addapp() {
			return PartialView();
		}
		public JsonResult GetUserList(string XM, string UserName) {
			UCHome_UserBLL userbll = new UCHome_UserBLL();
			var applist = new {
				data = userbll.GetUserList(XM, UserName),
				recordcount = userbll.GetInfoRecords(XM, UserName)
			};
			return Json(applist, JsonRequestBehavior.AllowGet);
		}
		public JsonResult DeleteUser(Guid PKID) {
			UCHome_UserBLL userbll = new UCHome_UserBLL();
			int result = userbll.DeleteUser(PKID, 0);
			if (result == 1) {
				var jr = new {
					statuscode = 200
				};
				return Json(jr, JsonRequestBehavior.AllowGet);
			}
			else {
				var jr = new {
					statuscode = 500
				};
				return Json(jr, JsonRequestBehavior.AllowGet);
			}
		}
		public JsonResult recoveryUser(Guid PKID) {
			UCHome_UserBLL userbll = new UCHome_UserBLL();
			int result = userbll.DeleteUser(PKID, 1);
			if (result == 1) {
				var jr = new {
					statuscode = 200
				};
				return Json(jr, JsonRequestBehavior.AllowGet);
			}
			else {
				var jr = new {
					statuscode = 500
				};
				return Json(jr, JsonRequestBehavior.AllowGet);
			}
		}
		public JsonResult ResetPwd(Guid PKID) {
			UCHome_UserBLL userbll = new UCHome_UserBLL();
			int result = userbll.ChangePassword(PKID, EncryptUtils.MD5Encrypt("123456"));
			if (result == 1) {
				var jr = new {
					statuscode = 200
				};
				return Json(jr, JsonRequestBehavior.AllowGet);
			}
			else {
				var jr = new {
					statuscode = 500
				};
				return Json(jr, JsonRequestBehavior.AllowGet);
			}
		}
		public PartialViewResult usermanage() {
			UCHome_UserBLL userbll = new UCHome_UserBLL();
			ViewBag.Records = userbll.GetInfoRecords("", "");
			return PartialView();
		}
		public PartialViewResult useradd() {
			UCHome_User user = new UCHome_User();
			user.PKID = Guid.NewGuid();
			user.State = 1;
			return PartialView(user);
		}
		public JsonResult AddUser(UCHome_User user) {
			user.UserPwd = EncryptUtils.MD5Encrypt(user.UserPwd);
			UCHome_UserBLL userbll = new UCHome_UserBLL();
			int result = userbll.AddUser(user);
			string errormsg = "提交的数据异常或者网络异常";
			if (result == 1) {
				int result2 = userbll.AddRelationOrgAndUser(user.PKID, new Guid(UCHomeBasePage.GetConfig("MainSchoolID")));
				if (result2 == 1) {
					var jrs = new {
						StatusCode = 200,
						data = user
					};
					return Json(jrs, JsonRequestBehavior.AllowGet);
				}
				else {
					userbll.RemoveUser(user);
					errormsg = "添加组织关系失败";
				}
			}
			else if (result == 2) {
				errormsg = "账号已经存在";
			}
			else {
				errormsg = "提交的数据异常或者网络异常";
			}
			var jr = new {
				StatusCode = 500,
				data = new UCHome_User {
					PKID = Guid.NewGuid()
				},
				msg = errormsg
			};
			return Json(jr, JsonRequestBehavior.AllowGet);
		}
		public PartialViewResult Rolemanage() {
			return PartialView();
		}
		public PartialViewResult Roleapp() {
			return PartialView();
		}

		public JsonResult GetRoleList(string roleType) {
			UCHome_AppBLL appbll = new UCHome_AppBLL();
			var result = appbll.GetRoleList(roleType);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult GetSysApp() {
			UCHome_AppBLL appbll = new UCHome_AppBLL();
			IEnumerable<UCHome_App_System> applist = appbll.getsysapplist();
			return Json(applist, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult GetAppmenus(Guid parentAppid) {
			UCHome_AppBLL appbll = new UCHome_AppBLL();
			IEnumerable<UCHome_App_Menu> appmenulist = appbll.getappmenulist(parentAppid);
			return Json(appmenulist, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult DeleteAppmenu(Guid pkid) {
			UCHome_AppBLL appbll = new UCHome_AppBLL();
			string statuscode = "200";
			string msg = "";
			try {
				appbll.DeleteAppmenu(pkid);
			}
			catch (Exception) {
				statuscode = "500";
				msg = "操作失败，请重试！";
				throw;
			}
			JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
			return Json(jr, JsonRequestBehavior.AllowGet);
		}
		[HttpPost]
		public JsonResult AddAppmenu(UCHome_App_Menu newappmenu) {
			UCHome_AppBLL appbll = new UCHome_AppBLL();
			string statuscode = "200";
			string msg = "";
			try {
				newappmenu.IsMyOrSystem = "1";
				appbll.Addappmenu(newappmenu);
			}
			catch (Exception) {
				statuscode = "500";
				msg = "操作失败，请重试！";
				throw;
			}
			JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
			return Json(jr, JsonRequestBehavior.AllowGet);
		}
		[HttpPost]
		public JsonResult AddApp(UCHome_AppType_Relation relation, UCHome_App_System sysapp) {
			UCHome_AppBLL appbll = new UCHome_AppBLL();
			string statuscode = "200";
			string msg = "";
			try {
				sysapp.Downloads = 0;
				sysapp.Hits = 0;
				sysapp.STATUS = "1";
				sysapp.AppColor = "1";
				sysapp.CreateTime = DateTime.Now;
				appbll.Addsystemapp(sysapp);
				relation.AppID = sysapp.PKID;
				relation.PKID = Guid.NewGuid();
				appbll.AddRelation_App_Type(relation);
			}
			catch (Exception) {
				statuscode = "500";
				msg = "操作失败，请重试！";
				throw;
			}
			JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
			return Json(jr, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult DeleteApp(Guid pkid) {
			UCHome_AppBLL appbll = new UCHome_AppBLL();
			string statuscode = "200";
			string msg = "";
			if (appbll.IsExistChildappmenu(pkid)) {
				statuscode = "300";
				msg = "抱歉，该项存在子功能，将不允许删除";
			}
			else {
				try {
					appbll.Deletesystemapp(pkid);
				}
				catch (Exception) {
					statuscode = "500";
					msg = "操作失败，请重试！";
					throw;
				}
			}

			JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
			return Json(jr, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult MoveApp(Guid pkid, string appstatus) {
			UCHome_AppBLL appbll = new UCHome_AppBLL();
			string statuscode = "200";
			string msg = "";
			UCHome_App_System newapp;
			try {
				newapp = appbll.Removesystemapp(pkid, appstatus);
			}
			catch (Exception) {
				statuscode = "500";
				msg = "操作失败，请重试！";
				throw;
			}
			JsonResult jr = new JsonResult { Data = new { statuscode, newapp } };
			return Json(jr, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult GetSysSkin() {
			UCHome_SkinBLL skinbll = new UCHome_SkinBLL();
			IEnumerable<UCHome_Skin> applist = skinbll.getsysskinlist();
			return Json(applist, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult MoveSkin(Guid pkid, string skinstatus) {
			UCHome_SkinBLL skinbll = new UCHome_SkinBLL();
			string statuscode = "200";
			string msg = "";
			UCHome_Skin newskin;
			try {
				newskin = skinbll.Removesystemskin(pkid, skinstatus);
			}
			catch (Exception) {
				statuscode = "500";
				msg = "操作失败，请重试！";
				throw;
			}
			JsonResult jr = new JsonResult { Data = new { statuscode, newskin } };
			return Json(jr, JsonRequestBehavior.AllowGet);
		}
		public JsonResult AddSkin(string SkinTheme, string SkinColor, string SkinName, string SkinBackground, string memo) {
			UCHome_SkinBLL skinbll = new UCHome_SkinBLL();
			UCHome_Skin sysskin = new UCHome_Skin {
				PKID = Guid.NewGuid(),
				SkinTheme = SkinTheme,
				Status = "1",
				SkinColor = SkinColor,
				SkinName = SkinName,
				SkinBackground = SkinBackground,
				Memo = memo,
			};
			string statuscode = "200";
			string msg = "";
			try {
				skinbll.Addsystemskin(sysskin);
			}
			catch (Exception) {
				statuscode = "500";
				msg = "操作失败，请重试！";
				throw;
			}
			JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
			return Json(jr, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetAppAndChild(Guid roleId) {
			var result = appbll.getAppAndChild(roleId);
			var parentMenuList = result.Select(p => p.parentMenuID).Distinct().ToList();
			var returnList = new List<object>();
			foreach (var pId in parentMenuList) {
				var childList = result.Where(p => p.parentMenuID == pId).ToList();
				var item = new {
					pId = pId,
					pName = childList.FirstOrDefault().parentMenuName,
					childList = childList.Select(p => new { appId = p.childMenuID, appName = p.childMenuName, selected = p.selected > 0 }).ToList()
				};
				returnList.Add(item);
			}
			return Json(returnList, JsonRequestBehavior.AllowGet);
		}


		public void ChooseApp(Guid appId, Guid roleId) {
			appbll.chooseApp(appId, roleId);
		}

		public JsonResult AddRole(string roleName, string roleType) {
			int addapp = appbll.AddRole(roleName, roleType);
			return Json(addapp, JsonRequestBehavior.AllowGet);
		}
		//删除角色
		public string DelappRole(Guid roleId) {
			var result = appbll.DeleteRole(roleId);
			return result;
		}

		public void AddUseToRole(string oldIds, string Ids, Guid roleId, string names) {
			UCHome_AppBLL appbll = new UCHome_AppBLL();
			appbll.addUseToRole(oldIds, Ids, roleId, names);
		}

		public JsonResult GetRoleListbyRId(Guid roleId) {
			var result = appbll.getroleListByRId(roleId).Select(s => new SelectListItem {
				Text = s.UserName.Trim(),
				Value = s.UserId.ToString()
			});
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		
	}
}
