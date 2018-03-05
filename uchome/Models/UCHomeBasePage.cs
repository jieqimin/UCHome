using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Basic.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UCHome.Models;
using Wef.Adapter.Authorization;

namespace UCHome.Model {
	public class UCHomeBasePage {
		//通过接口获取当前用户"
		public static string loginmethod = GetConfig("LoginSet.LoginMethod").ToLower();
		public static string iiswebsitedirectory = GetConfig("IISWebSiteDirectory");
		public static UserInfo RequestUser {
			get {
				UserInfo ui = new UserInfo();
				GetRequestUser(out ui);
				return ui;
			}
		}
		private static void GetRequestUser(out UserInfo userinfo) {
			UCHomeEntities uche = new UCHomeEntities();
			//Guid uid;
			//string rid = GetRequestValue();
			//if (string.IsNullOrEmpty(rid) || rid == Guid.Empty.ToString())
			//{
			//判断是否已登录
			if (HttpContext.Current.Request.Cookies["PersonInfo"] != null) {
				HttpCookie cookies = HttpContext.Current.Request.Cookies["PersonInfo"];

				UserInfo ui = new UserInfo {
					areacode = EncryptUtils.Base64Decrypt(cookies["areacode"]),
					xxid = new Guid(EncryptUtils.Base64Decrypt(cookies["xxid"])),
					xxmc = EncryptUtils.Base64Decrypt(cookies["xxmc"]),
					orgid = new Guid(EncryptUtils.Base64Decrypt(cookies["orgid"])),
					orgname = EncryptUtils.Base64Decrypt(cookies["orgname"]),
					usertype = EncryptUtils.Base64Decrypt(cookies["usertype"]),
					userid = new Guid(EncryptUtils.Base64Decrypt(cookies["userid"])),
					username = EncryptUtils.Base64Decrypt(cookies["username"]),
					loginname = EncryptUtils.Base64Decrypt(cookies["loginname"])
				};
				if (HttpContext.Current.Request.Cookies["SpaceInfo"] != null) {
					HttpCookie cookies2 = HttpContext.Current.Request.Cookies["SpaceInfo"];
					ui.headpic = EncryptUtils.Base64Decrypt(cookies2["HeadPic"]);
					ui.subject = EncryptUtils.Base64Decrypt(cookies2["Subject"]);
					ui.skintheme = EncryptUtils.Base64Decrypt(cookies2["Theme"]);
					ui.skincss = EncryptUtils.Base64Decrypt(cookies2["Skin"]);
				}
				else {
					UCHome_BaseInfo space = uche.UCHome_BaseInfo.SingleOrDefault(s => s.UserID == ui.userid);
					if (space != null) {
						SetSpaceCookies(space);
						ui.headpic = space.headphoto;
						ui.subject = space.Subject;
						ui.skincss = space.SkinCss;
						ui.skintheme = space.SkinTheme;
					}
				}
				if (HttpContext.Current.Request.Cookies["ChildInfo"] != null) {
					HttpCookie cookies3 = HttpContext.Current.Request.Cookies["ChildInfo"];
					ui.childinfo = new ChildInfo {
						childareacode = EncryptUtils.Base64Decrypt(cookies3["childareacode"]),
						childxxid = new Guid(EncryptUtils.Base64Decrypt(cookies3["childxxid"])),
						childxxmc = EncryptUtils.Base64Decrypt(cookies3["childxxmc"]),
						childbjid = new Guid(EncryptUtils.Base64Decrypt(cookies3["childbjid"])),
						childbjmc = EncryptUtils.Base64Decrypt(cookies3["childbjmc"]),
						childuserid = new Guid(EncryptUtils.Base64Decrypt(cookies3["childuserid"])),
						childusername = EncryptUtils.Base64Decrypt(cookies3["childusername"])
					};
					ui.areacode = EncryptUtils.Base64Decrypt(cookies3["childareacode"]);
					ui.xxid = new Guid(EncryptUtils.Base64Decrypt(cookies3["childxxid"]));
					ui.xxmc = EncryptUtils.Base64Decrypt(cookies3["childxxmc"]);
					ui.orgid = new Guid(EncryptUtils.Base64Decrypt(cookies3["childbjid"]));
					ui.orgname = EncryptUtils.Base64Decrypt(cookies3["childbjmc"]);
				}
				userinfo = ui;
			}
			else {
				userinfo = null;
				HttpContext.Current.Response.Redirect(iiswebsitedirectory + "Login/SSOtransfer?returnurl=" + HttpContext.Current.Request.Url);
			}
		}
		public static void SetSSOUser() {
			if (!IsLogin) {
				UCHomeEntities uche = new UCHomeEntities();
				//登录跳转 
				Guid uid;
				if (loginmethod == "other") {
					uid = SSO.Adapter.AdapterHelper.CurrentNewUser;
				}
				else {
					//本系统登录uid
					uid = Guid.Parse(HttpContext.Current.Request.QueryString["UserID"]);
				}
				if (uid != null && uid != Guid.Empty) {
					View_Simple_User u = uche.View_Simple_User.FirstOrDefault(tea => tea.userid == uid);

					if (u != null) {
						SetUserCookies(u);

						UserInfo ui = new UserInfo {
							areacode = u.xzqhm,
							xxid = new Guid(u.xxid.ToString()),
							xxmc = u.xxmc,
							orgid = u.orgid,
							orgname = u.orgname ?? "",
							usertype = u.usertype,
							userid = u.userid,
							username = u.username,
							loginname = u.loginname
						};
						UCHome_BaseInfo space = uche.UCHome_BaseInfo.SingleOrDefault(s => s.UserID == u.userid);
						if (space != null) {
							SetSpaceCookies(space);
							ui.headpic = space.headphoto;
							ui.subject = space.Subject;
							ui.skincss = space.SkinCss;
							ui.skintheme = space.SkinTheme;
						}
						if (HttpContext.Current.Request.Cookies["ChildInfo"] != null) {
							HttpCookie cookies3 = HttpContext.Current.Request.Cookies["ChildInfo"];
							ui.childinfo = new ChildInfo {
								childareacode = EncryptUtils.Base64Decrypt(cookies3["childareacode"]),
								childxxid = new Guid(EncryptUtils.Base64Decrypt(cookies3["childxxid"])),
								childxxmc = EncryptUtils.Base64Decrypt(cookies3["childxxmc"]),
								childbjid = new Guid(EncryptUtils.Base64Decrypt(cookies3["childbjid"])),
								childbjmc = EncryptUtils.Base64Decrypt(cookies3["childbjmc"]),
								childuserid = new Guid(EncryptUtils.Base64Decrypt(cookies3["childuserid"])),
								childusername = EncryptUtils.Base64Decrypt(cookies3["childusername"])
							};
						}
					}
				}
			}
		}
		public static void SetUserCookies(View_Simple_User u) {
			HttpCookie cookie = new HttpCookie("PersonInfo");
			cookie.Values.Add("areacode", EncryptUtils.Base64Encrypt(u.xzqhm ?? ""));
			cookie.Values.Add("xxid", EncryptUtils.Base64Encrypt(u.xxid.ToString()));
			cookie.Values.Add("xxmc", EncryptUtils.Base64Encrypt(u.xxmc ?? ""));
			cookie.Values.Add("orgid", EncryptUtils.Base64Encrypt(u.orgid.ToString()));
			cookie.Values.Add("orgname", EncryptUtils.Base64Encrypt(u.orgname ?? ""));
			cookie.Values.Add("usertype", EncryptUtils.Base64Encrypt(u.usertype));
			cookie.Values.Add("userid", EncryptUtils.Base64Encrypt(u.userid.ToString()));
			cookie.Values.Add("username", EncryptUtils.Base64Encrypt(u.username));
			cookie.Values.Add("loginname", EncryptUtils.Base64Encrypt(u.loginname));
			HttpContext.Current.Response.AppendCookie(cookie);
		}
		public static void SetSpaceCookies(UCHome_BaseInfo space) {
			HttpCookie cookie = new HttpCookie("SpaceInfo");
			cookie.Values.Add("HeadPic", EncryptUtils.Base64Encrypt(space.headphoto ?? ""));
			cookie.Values.Add("Subject", EncryptUtils.Base64Encrypt(space.Subject ?? ""));
			cookie.Values.Add("Theme", EncryptUtils.Base64Encrypt(space.SkinTheme ?? ""));
			cookie.Values.Add("Skin", EncryptUtils.Base64Encrypt(space.SkinCss ?? ""));
			HttpContext.Current.Response.AppendCookie(cookie);
		}
		public static void SetChildCookies(View_Simple_StuInfo child) {
			if (HttpContext.Current.Request.Cookies["ChildInfo"] != null) {
				HttpCookie cookie = HttpContext.Current.Request.Cookies["ChildInfo"];
				cookie.Values["childareacode"] = EncryptUtils.Base64Encrypt(child.XZQHM ?? "");
				cookie.Values["childxxid"] = EncryptUtils.Base64Encrypt(child.xxid.ToString());
				cookie.Values["childxxmc"] = EncryptUtils.Base64Encrypt(child.XXMC ?? "");
				cookie.Values["childbjid"] = EncryptUtils.Base64Encrypt(child.BJID.ToString());
				cookie.Values["childbjmc"] = EncryptUtils.Base64Encrypt(child.bjmc ?? "");
				cookie.Values["childuserid"] = EncryptUtils.Base64Encrypt(child.xsid.ToString());
				cookie.Values["childusername"] = EncryptUtils.Base64Encrypt(child.XM);
				HttpContext.Current.Response.AppendCookie(cookie);
			}
			else {
				HttpCookie cookie = new HttpCookie("ChildInfo");
				cookie.Values.Add("childareacode", EncryptUtils.Base64Encrypt(child.XZQHM ?? ""));
				cookie.Values.Add("childxxid", EncryptUtils.Base64Encrypt(child.xxid.ToString()));
				cookie.Values.Add("childxxmc", EncryptUtils.Base64Encrypt(child.XXMC ?? ""));
				cookie.Values.Add("childbjid", EncryptUtils.Base64Encrypt(child.BJID.ToString()));
				cookie.Values.Add("childbjmc", EncryptUtils.Base64Encrypt(child.bjmc ?? ""));
				cookie.Values.Add("childuserid", EncryptUtils.Base64Encrypt(child.xsid.ToString()));
				cookie.Values.Add("childusername", EncryptUtils.Base64Encrypt(child.XM));
				HttpContext.Current.Response.AppendCookie(cookie);
			}
		}
		public static void SetCookies<T>(T item, string cookeiname) {

			PropertyInfo[] propertys = item.GetType().GetProperties();

			HttpCookie cookie = new HttpCookie(cookeiname);
			for (int i = 0; i < propertys.Count(); i++) {
				object obj = propertys[i].GetValue(item, null);
				cookie.Values.Add(propertys[i].Name, EncryptUtils.Base64Encrypt(obj.ToString()));
			}
			HttpContext.Current.Response.AppendCookie(cookie);
		}

		//获取当前学校
		public Guid CurrentSch {
			get {
				return RequestUser.xxid;
			}
		}
		//获取当前学年;
		private string CurrentTerm {
			get {
				string yt;
				HttpCookie cookies = HttpContext.Current.Request.Cookies["PersonInfo"];
				if (cookies["yearterm"] == null) {
					var term = DateTime.Now.Month < 9 && DateTime.Now.Month > 2 ? "2" : "1";
					var year = DateTime.Now.Month < 9 && DateTime.Now.Month > 2
						? (DateTime.Now.Year - 1).ToString()
						: DateTime.Now.Year.ToString();
					SchoolEntities ett = new SchoolEntities();
					Basic_ZZJX0103 yearterm = ett.Basic_ZZJX0103.FirstOrDefault(s => s.XXID == CurrentSch);
					//
					//获取当前学校所在学年
					SchInfo.BLL.SICalendarBLL sicalbll = new SchInfo.BLL.SICalendarBLL();
					SchInfo.Model.siCalendar calendar = sicalbll.GetCurrentSICalendar(CurrentSch);
					if (calendar != null) {
						string cur = calendar.CalCode;
						yt = cur;
					}
					else {
						yt = string.Format("{0}{1}", year, term);
					}
					cookies.Values.Add("yearterm", yt);
					HttpContext.Current.Response.AppendCookie(cookies);
				}
				else {
					yt = cookies["yearterm"].ToString();
				}
				return yt;
			}
		}

		private string CurrentYYMM {
			get {

				var term = DateTime.Now.Month < 9 && DateTime.Now.Month > 2 ? "2" : "1";
				var year = DateTime.Now.Month < 9 && DateTime.Now.Month > 2 ? (DateTime.Now.Year - 1).ToString() : DateTime.Now.Year.ToString();
				var yt = string.Format("{0}{1}", year, term);
				return yt;
			}
		}

		public string CurrentSchTerm {
			get {
				if (HttpContext.Current.Request.Cookies["PersonInfo"] == null) {
					return CurrentYYMM;
				}
				return CurrentTerm;
			}
		}
		public View_Simple_StuInfo CurrentStu {
			get {
				UCHomeEntities uc = new UCHomeEntities();
				Guid xsGuid = RequestUser.userid;
				View_Simple_StuInfo stu = uc.View_Simple_StuInfo.SingleOrDefault(s => s.xsid == xsGuid);
				if (stu != null)
					return stu;
				return null;
			}
		}
		public static bool IsLogin {
			get {
				try {
					if (HttpContext.Current.Request.Cookies["PersonInfo"] != null) {
						return true;
					}
					return false;
				}
				catch (Exception) {
					return false;
				}
			}
		}

		public static string GetConfig(string key) {
			string keyvalue = System.Configuration.ConfigurationManager.AppSettings[key];
			if (!string.IsNullOrEmpty(keyvalue))
				return keyvalue;
			Exception e = new Exception("未找到该配置项，请确认已为[键值：" + key + "]设置配置项");
			throw e;
		}
		public void LoginOut() {
			//清空cookies
			HttpCookie aCookie;
			string cookieName;
			int limit = HttpContext.Current.Request.Cookies.Count;
			for (int i = 0; i < limit; i++) {
				cookieName = HttpContext.Current.Request.Cookies[i].Name;
				aCookie = new HttpCookie(cookieName);
				aCookie.Expires = DateTime.Now.AddDays(-1);
				HttpContext.Current.Response.Cookies.Add(aCookie);
			}
		}

		public UserInfo GetUserInfoByRequestId(Guid id) {
			UCHomeEntities uche = new UCHomeEntities();
			View_Simple_User u = uche.View_Simple_User.SingleOrDefault(tea => tea.userid == id);
			if (u != null) {
				UserInfo ui = new UserInfo {
					areacode = u.xzqhm,
					xxid = new Guid(u.xxid.ToString()),
					xxmc = u.xxmc,
					orgid = u.orgid,
					orgname = u.orgname ?? "",
					usertype = u.usertype,
					userid = u.userid,
					username = u.username,
					loginname = u.loginname
				};
				return ui;
			}
			return null;
		}
	}


}