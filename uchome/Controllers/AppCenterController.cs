using ServiceStack.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ServiceStack.ServiceClient.Web;
using Student.Model;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.Controllers
{
    public class AppCenterController : Controller
    {
        //
        // GET: /AppCenter/
        readonly BLL.UCHome_AppBLL appbll = new BLL.UCHome_AppBLL();
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private static readonly string http = System.Web.Configuration.WebConfigurationManager.AppSettings["APIHttp"];
        private IServiceClient client = new JsonServiceClient(http + "/SNSApi/");
        private UserInfo user
        {
            get
            {
                return UCHomeBasePage.RequestUser;
            }
        }
        private Guid loginId
        {
            get
            {
                return user.userid;
            }
        }
        private string loginname
        {
            get
            {
                return user.username;
            }
        }
        private string userType
        {
            get
            {
                return user.usertype;
            }
        }
        private string xxmc
        {
            get
            {
                return user.xxmc;
            }
        }
        private string headpic
        {
            get
            {
                return user.headpic;
            }
        }
        private string subject
        {
            get
            {
                return user.subject;
            }
        }
        private string skincss
        {
            get
            {
                return user.skincss;
            }
        }
        private string skintheme
        {
            get
            {
                return user.skintheme;
            }
        }
        private Guid XXID
        {
            get
            {
                try
                {
                    return user.xxid;
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }
        }
        public ActionResult Index()
        {
            UCHomeEntities uc = new UCHomeEntities();
            if (userType.ToLower() == "p" && Request.Cookies["ChildInfo"] == null)
            {
                StudentEntities stu = new StudentEntities();
                List<Guid> rel =
                    stu.Stu_FamilyStuRel.Where(f => f.JZID == user.userid).Select(f => f.XSID).ToList();
                if (rel.Count == 1)
                {
                    //设置ChildGuid
                    Guid xsid = rel.First();
                    View_Simple_StuInfo childinfo = uc.View_Simple_StuInfo.SingleOrDefault(s => s.xsid == xsid);
                    UCHomeBasePage.SetChildCookies(childinfo);
                }
                else
                {
                    return RedirectToAction("HomePage", "Home");
                }

            }
            return View(user);
        }
        public PartialViewResult AppLeft()
        {
            if (userType.ToLower() == "t")
                ViewBag.Subject = subject;
            ViewBag.HeadPic = headpic;
            ViewBag.UserType = userType.ToLower() == "s" ? "学生" : "教师";
            ViewBag.UserID = loginId;
            ViewBag.XM = loginname;
            ViewBag.XXMC = xxmc;
            ViewBag.XXID = XXID;

            return PartialView("AppLeft");
        }
        public void Logout(string returnurl)
        {
            ucbase.LoginOut();
            if (UCHomeBasePage.loginmethod == "other")
            {
                string loginouturl = UCHomeBasePage.GetConfig("SSO.SignOut");
                Response.Redirect(loginouturl + "&ReturnUrl=" + returnurl);
            }
            else
            {
                string transferurl = string.Format("{0}Login/loginTransfer?uchomereturnurl={1}", UCHomeBasePage.iiswebsitedirectory, returnurl);
                Response.Redirect(Url.Action("Login", "Login") + "?ReturnUrl=" + Url.Encode(transferurl));
            }

        }
        public ActionResult FamSch(string id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_Skin skin = uc.UCHome_Skin.FirstOrDefault(s => s.PIndex > 0);
            return View("FamSch", skin);
        }
        public JsonResult corsgetdata(UCHome_Skin skin)
        {
            skin.PKID = Guid.NewGuid();
            string apiurl = APIHelper.GetApiUrl("addskin");
            var resultdata = HttpClientHelper.POSTMethod(apiurl, skin);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult MyAppMenu()
        {
            return PartialView("MyAppMenu", user);
        }
        public ActionResult HotApp()
        {
            ViewBag.userid = user.userid;
            ViewBag.usertype = user.usertype;
            List<UCHome_App_System> applist = GetApp(user.usertype);
            switch (user.usertype)
            {
                case "t":
                    return PartialView("HotAppByTeacher", applist);
                case "s":
                    return PartialView("HotAppByStudent", applist);
                case "p":
                    return PartialView("HotAppByParents", applist);
                case "m":
                    return PartialView("HotAppByTeacher", applist);
                default:
                    return View("Index");
            }
        }
        public ActionResult NewApp()
        {
            ViewBag.userid = user.userid;
            ViewBag.usertype = user.usertype;
            List<UCHome_App_System> applist = GetApp(user.usertype);
            switch (user.usertype)
            {
                case "t":
                    return PartialView("NewAppByTeacher", applist);
                case "s":
                    return PartialView("NewAppByStudent", applist);
                case "p":
                    return PartialView("NewAppByParents", applist);
                case "m":
                    return PartialView("NewAppByTeacher", applist);
                default:
                    return View("Index");
            }
        }
        public ActionResult AllApplication()
        {
            ViewBag.userid = user.userid;
            ViewBag.usertype = user.usertype;
            List<UCHome_App_System> applist = GetApp(user.usertype);
            switch (user.usertype)
            {
                case "t":
                    return PartialView("AllApplicationByTeacher", applist);
                case "s":
                    return PartialView("AllApplicationByStudent", applist);
                case "p":
                    return PartialView("AllApplicationByParents", applist);
                case "m":
                    return PartialView("AllApplicationByAdmin", applist);
                default:
                    return View("Index");
            }
        }

        public List<UCHome_App_System> GetApp(string usertype)
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<UCHome_App_System> applist = (from a in uc.UCHome_App_System
                                               join t in uc.UCHome_AppType_Relation on a.PKID equals t.AppID
                                               join b in uc.UCHome_AppRole_Relation on a.PKID equals b.Appid
                                               join c in uc.UCHome_UserRole_Relation.Where(x => x.UserId == loginId) on b.RoleId equals c.RoleId
                                               where ((a.STATUS == "1")) orderby a.AppOrder
                                               select a).Distinct().ToList();
            return applist;
        }
        public PartialViewResult AppInfoById(string id)
        {

            return PartialView("AppInfoById", id);

        }
        public ActionResult AppMenuById(string id)
        {

            return PartialView("AppMenuById", id);

        }
        public ActionResult MyAppRedirectPage(Guid id, Guid? MenuID)
        {
            if (user == null)
            {
                return Redirect(Url.Action("Logout"));
            }
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_App_My app = uc.UCHome_App_My.SingleOrDefault(u => u.PKID == id);
            if (app != null && app.AppTarget == "_blank")
                return Redirect(app.AppUrl);
            MyAppCollection myapp = new MyAppCollection();
            myapp.myapp = app;
            ViewBag.SelMenuID = MenuID;
            return View("MyAppRedirectPage", myapp);
        }
        public ActionResult SystemAppRedirectPage(Guid id,string typename, Guid? MenuID)
        {
            if (user == null)
            {
                return Redirect(Url.Action("Logout"));
            }
            UCHomeEntities uc = new UCHomeEntities();
            if (userType.ToLower() == "p" && Request.Cookies["ChildInfo"] == null)
            {
                StudentEntities stu = new StudentEntities();
                List<Guid> rel =
                    stu.Stu_FamilyStuRel.Where(f => f.JZID == user.userid).Select(f => f.XSID).ToList();
                if (rel.Count == 1)
                {
                    //设置ChildGuid
                    Guid xsid = rel.First();
                    View_Simple_StuInfo childinfo = uc.View_Simple_StuInfo.SingleOrDefault(s => s.xsid == xsid);
                    UCHomeBasePage.SetChildCookies(childinfo);
                }
                else
                {
                    return RedirectToAction("HomePage", "Home");
                }
            }
            UCHome_App_System app = uc.UCHome_App_System.SingleOrDefault(u => u.PKID == id);
            if (app != null && app.AppTarget == "_blank")
                return Redirect(app.AppUrl);
            AppCollection sysapp = new AppCollection();
            sysapp.app = app;
            ViewBag.SelMenuID = MenuID;
			ViewBag.TypeName = typename;
			return PartialView("AppRedirectPage", sysapp);
        }
        public JsonResult GetAppMenu(Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            IEnumerable<UCHome_App_Menu> app = uc.UCHome_App_Menu.Where(u => u.ParentAppID == id).OrderBy(u => u.Orders);
            return Json(app, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult Addapp()
        {
            UCHome_App_My myapp = new UCHome_App_My();
            myapp.PKID = Guid.NewGuid();
            return PartialView(myapp);
        }
        [HttpPost]
        public JsonResult Addapp(UCHome_App_My myapp)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                myapp.Downloads = 0;
                myapp.Hits = 0;
                myapp.AppOrder = 9;
                myapp.STATUS = "1";
                myapp.AppColor = "3";
                myapp.AppFrom = Guid.Empty;
                myapp.CreateTime = DateTime.Now;
                myapp.UserID = loginId;
                try
                {
                    appbll.Addmyapp(myapp);
                }
                catch (Exception ex)
                {
                    statuscode = "500";
                    msg = ex.ToString();
                }
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Addmyappfromsys(Guid appid)
        {
            UCHome_App_System sysapp = appbll.getsysappbyid(appid);

            string statuscode = "200";
            string msg = "";
            UCHome_App_My myapp = appbll.sysappTomyapp(sysapp);
            myapp.UserID = loginId;
            try
            {
                appbll.Addmyapp(myapp);
                appbll.addappdownloads(sysapp);
            }
            catch (Exception ex)
            {
                statuscode = "500";
                msg = ex.ToString();
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult Editmyapp(Guid appid)
        {
            UCHome_App_My myapp = appbll.getappbyid(appid);
            return PartialView(myapp);
        }
        [HttpPost]
        public JsonResult Editmyapp(UCHome_App_My myapp)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                myapp.Downloads = 0;
                myapp.Hits = 0;
                myapp.AppOrder = 9;
                myapp.STATUS = "1";
                myapp.AppColor = "3";
                myapp.AppFrom = Guid.Empty;
                myapp.CreateTime = DateTime.Now;
                myapp.UserID = loginId;
                try
                {
                    appbll.Addmyapp(myapp);
                }
                catch (Exception ex)
                {
                    statuscode = "500";
                    msg = ex.ToString();
                }
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult Deletemyapp()
        {
            UCHome_App_My myapp = new UCHome_App_My();
            myapp.PKID = Guid.NewGuid();
            return PartialView(myapp);
        }
        [HttpPost]
        public JsonResult Deletemyapp(UCHome_App_My myapp)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                myapp.Downloads = 0;
                myapp.Hits = 0;
                myapp.AppOrder = 9;
                myapp.STATUS = "1";
                myapp.AppColor = "3";
                myapp.AppFrom = Guid.Empty;
                myapp.CreateTime = DateTime.Now;
                myapp.UserID = loginId;
                try
                {
                    appbll.Addmyapp(myapp);
                }
                catch (Exception ex)
                {
                    statuscode = "500";
                    msg = ex.ToString();
                }
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Movemyapp(Guid appid)
        {
            string statuscode = "200";
            string msg = "";
            try
            {
                appbll.Movemyapp(appid);
            }
            catch (Exception)
            {
                statuscode = "500";
                msg = "操作失败，请重试！";
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Delmyapp(Guid appid)
        {
            string statuscode = "200";
            string msg = "";
            if (appbll.IsExistChildappmenu(appid))
            {
                statuscode = "300";
                msg = "抱歉，该项存在子功能，将不允许删除";
            }
            else
            {
                try
                {
                    appbll.Movemyapp(appid);
                }
                catch (Exception)
                {
                    statuscode = "500";
                    msg = "操作失败，请重试！";
                    throw;
                }
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult Addappmenu(Guid parentAppId)
        {
            ViewBag.ParentAppID = parentAppId;
            return PartialView();
        }
        [HttpPost]
        public JsonResult Addappmenu(UCHome_App_Menu appmenu)
        {
            appmenu.PKID = Guid.NewGuid();
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                appmenu.IsMyOrSystem = "1";
                try
                {
                    appbll.Addappmenu(appmenu);
                }
                catch (Exception ex)
                {
                    statuscode = "500";
                    msg = ex.ToString();
                }
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult Appmanage()
        {
            IEnumerable<UCHome_App_My> myapplist = appbll.getmyapplist(loginId);
            return PartialView(myapplist);
        }
        public string GetMyAppList()
        {
            IEnumerable<UCHome_App_My> myapplist = appbll.getmyapplist(loginId);
            StringBuilder jsonstr = new StringBuilder();
            string split = string.Empty;
            jsonstr.AppendFormat("{{\"data\":[");
            foreach (UCHome_App_My item in myapplist)
            {
                jsonstr.AppendFormat("{4}{{\"appname\":\"{0}\",\"appurl\":\"{1}\",\"apporder\":\"{2}\",\"pkid\":\"{3}\",\"appfrom\":\"{5}\"}}",
                    item.AppName, item.AppUrl, item.AppOrder, item.PKID, split, item.AppFrom);
                split = ",";
            }
            jsonstr.AppendFormat("]}}");
            return jsonstr.ToString();
        }
        public string GetMenuByParentAppId(Guid parentAppId)
        {
            IEnumerable<UCHome_App_Menu> menulist = appbll.getappmenulist(parentAppId);
            string menus = entitytojson(menulist);
            return menus;
        }

        public string entitytojson(IEnumerable<UCHome_App_Menu> menulist)
        {
            StringBuilder jsonstr = new StringBuilder();
            string split = string.Empty;
            jsonstr.AppendFormat("{{\"data\":[");
            foreach (UCHome_App_Menu item in menulist)
            {
                jsonstr.AppendFormat("{4}{{\"menuname\":\"{0}\",\"menuhref\":\"{1}\",\"orders\":\"{2}\",\"pkid\":\"{3}\",\"datapath\":\"{5}\",\"controllname\":\"{6}\"}}",
                    item.MenuName, item.Menuhref, item.Orders, item.PKID, split, item.DataPath, item.controllername);
                split = ",";
            }
            jsonstr.AppendFormat("]}}");
            return jsonstr.ToString();
        }

        [HttpPost]
        public JsonResult deleteappmenu(Guid appmenuid)
        {
            string statuscode = "200";
            string msg = "";
            try
            {
                appbll.DeleteAppmenu(appmenuid);
            }
            catch (Exception)
            {
                statuscode = "500";
                msg = "操作失败，请重试！";
                throw;
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            appbll.Dispose();
            base.Dispose(disposing);
        }
        public PartialViewResult AppMenu(Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            IEnumerable<UCHome_App_Menu> menulist = uc.UCHome_App_Menu.Where(m => m.ParentAppID == id);
            return PartialView(menulist);
        }
    }
}
