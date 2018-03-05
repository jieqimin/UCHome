using System;
using System.Web.Mvc;
using UCHome.BLL;
using UCHome.Model;
namespace UCHome.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /TeacherCenter/
        readonly UCHomeEntities _uc = new UCHomeEntities();
        private readonly UCResourceEntities _rec = new UCResourceEntities();
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        public ActionResult login(string returnurl)
        {
            string loginmethod = UCHomeBasePage.loginmethod;
            if (UCHomeBasePage.IsLogin)
            {
                return RedirectToAction("Index", "Home");
            }
            if (loginmethod == "self")
            {
                ViewBag.ReturnUrl = returnurl == null ? Url.Action("loginTransfer") + "?uchomereturnurl=" + Url.Action("Index") : returnurl;
                return View();
            }
            else
            {
                returnurl = returnurl == null ? Url.Action("Index") : returnurl;
                return Redirect(Url.Action("SSOtransfer") + "?ReturnUrl=" + returnurl);
            }
        }
        public JsonResult CheckLogin(string UserName, string UserPwd)
        {
            UCHome_UserBLL userbll = new UCHome_UserBLL();
            UserPwd = EncryptUtils.MD5Encrypt(UserPwd);
            if (userbll.CheckUser(UserName, UserPwd, out Guid useid))
            {
                JsonResult result = new JsonResult
                {
                    Data = new { StatusCode = 200, ErrorMessage = "", UserID = useid }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                JsonResult result = new JsonResult
                {
                    Data = new { StatusCode = 400, ErrorMessage = "帐号密码错误", UserID = "" }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AdminLogin()
        {
            if (UCHomeBasePage.IsLogin)
            {
                return RedirectToAction("Index", "Home");
            }
            string UserName = "jimmy", UserPwd = "jimmy";
            UCHome_UserBLL userbll = new UCHome_UserBLL();
            UserPwd = EncryptUtils.MD5Encrypt(UserPwd);
            Guid userid = Guid.Empty;
            userbll.CheckUser(UserName, UserPwd, out userid);
            string transferurl = string.Format("{0}Login/loginTransfer?uchomereturnurl={1}&UserID={2}", UCHomeBasePage.iiswebsitedirectory, Url.Action("HomePage","Home"), userid);
            return Redirect(transferurl);
        }
        public ActionResult TeacherLogin()
        {
            if (UCHomeBasePage.IsLogin)
            {
                return RedirectToAction("Index", "Home");
            }
            string UserName = "wangdzi", UserPwd = "123456";
            UCHome_UserBLL userbll = new UCHome_UserBLL();
            UserPwd = EncryptUtils.MD5Encrypt(UserPwd);
            Guid userid = Guid.Empty;
            userbll.CheckUser(UserName, UserPwd, out userid);
            string transferurl = string.Format("{0}Login/loginTransfer?uchomereturnurl={1}&UserID={2}", UCHomeBasePage.iiswebsitedirectory, Url.Action("HomePage", "Home"), userid);
            return Redirect(transferurl);
        }
        public ActionResult StudentLogin()
        {
            if (UCHomeBasePage.IsLogin)
            {
                return RedirectToAction("Index", "Home");
            }
            string UserName = "2016370901000631005", UserPwd = "111111";
            UCHome_UserBLL userbll = new UCHome_UserBLL();
            UserPwd = EncryptUtils.MD5Encrypt(UserPwd);
            Guid userid = Guid.Empty;
            userbll.CheckUser(UserName, UserPwd, out userid);
            string transferurl = string.Format("{0}Login/loginTransfer?uchomereturnurl={1}&UserID={2}", UCHomeBasePage.iiswebsitedirectory, Url.Action("HomePage", "Home"), userid);
            return Redirect(transferurl);
        }
        public ActionResult ParentLogin()
        {
            if (UCHomeBasePage.IsLogin)
            {
                return RedirectToAction("Index", "Home");
            }
            string UserName = "370983199102123753", UserPwd = "111111";
            UCHome_UserBLL userbll = new UCHome_UserBLL();
            UserPwd = EncryptUtils.MD5Encrypt(UserPwd);
            Guid userid = Guid.Empty;
            userbll.CheckUser(UserName, UserPwd, out userid);
            string transferurl = string.Format("{0}Login/loginTransfer?uchomereturnurl={1}&UserID={2}", UCHomeBasePage.iiswebsitedirectory, Url.Action("HomePage", "Home"), userid);
            return Redirect(transferurl);
        }
        public ActionResult index()
        {
            return View();
        }
        public ActionResult SSOtransfer(string returnurl)
        {
            string transferurl = string.Format("{0}Login/loginTransfer?uchomereturnurl={1}", UCHomeBasePage.iiswebsitedirectory, returnurl);
            if (UCHomeBasePage.loginmethod == "other")
            {
                return Redirect(UCHomeBasePage.GetConfig("SSO.SignIn") + "&returnurl=" + Url.Encode(transferurl));
            }
            else
            {
                return Redirect(Url.Action("Login") + "?returnurl=" + Url.Encode(transferurl));
            }
        }
        /// <summary>
        /// 指定返回路径
        /// </summary>
        /// <param name="returnurl"></param>
        /// <returns></returns>
        public ActionResult SSOlogin(string returnurl)
        {
            UCHomeBasePage.SetSSOUser();
            return Redirect(returnurl);
        }
        //不指定返回路径
        public ActionResult loginTransfer(string uchomereturnurl)
        {
            UCHomeBasePage.SetSSOUser();
            return Redirect(uchomereturnurl);
        }

    }
}
