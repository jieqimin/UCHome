using EcoEdu.Framework.SingleSignOn;
using Newtonsoft.Json;
using ServiceStack.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ServiceModel;
using ServiceStack.ServiceClient.Web;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class FollowUserCenterController : Controller
    {
        //
        // GET: /FollowUserCenter/
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private static string http = System.Web.Configuration.WebConfigurationManager.AppSettings["APIHttp"];
        private static string http2 = System.Web.Configuration.WebConfigurationManager.AppSettings["APIHttp2"];
        private static string images_head = System.Web.Configuration.WebConfigurationManager.AppSettings["images_head"];
        private IServiceClient client = new JsonServiceClient(http + "/SNSApi/");
        private readonly UCHomeEntities _uc = new UCHomeEntities();
        private static UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        private Guid loginId
        {
            get
            {
                try
                {
                    return user.userid;
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }
        }

        #region 好友

        public ActionResult HailFellow()
        {
            ViewBag.Useirid = loginId;
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_BaseInfo userInfo = uc.UCHome_BaseInfo.FirstOrDefault(u => u.UserID == loginId);
            return View(userInfo);
        }

        public ActionResult FollowUser()
        {
            ViewBag.APIHTTP2 = http2;
            ViewBag.ImageHead = images_head;
            return View();
        }

        public ActionResult SchoolFollowUser()
        {
            ViewBag.APIHTTP2 = http2;
            ViewBag.ShoolId = user.xxid;
            ViewBag.ImageHead = images_head;

            if (user.usertype == "S")
            {
                ViewBag.TeaTitle = "我的老师";
                ViewBag.StuTitle = "我的同学";
            }
            else
            {
                ViewBag.TeaTitle = "我的同事";
                ViewBag.StuTitle = "我的学生";
            }

            return View();
        }

        public ActionResult SelectFollowUser()
        {
            ViewBag.APIHTTP2 = http2;
            ViewBag.ImageHead = images_head;
            return View();
        }

        public JsonResult GetMyBookList(string curPage,string name)
        {
            var newClient = new JsonServiceClient(http + "/SNSApi/");

            var userId = loginId.ToString().ToLower();

            AddrBookPageResult book =
                newClient.Get(new GetGroupAddrBooks
                {
                    GroupID = userId,
                    UName = name,
                    CurPage = Convert.ToInt32(curPage) - 1,
                    PageSize = 15
                });

            return Json(book, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMyBookUserIds(string schoolId)
        {
            var newClient = new JsonServiceClient(http + "/SNSApi/");

            var userId = loginId.ToString().ToLower();

            AddrBookPageResult book =
                newClient.Get(new GetGroupAddrBooks
                {
                    GroupID = userId,
                    CurPage = 0,
                    PageSize = 15
                });

            var userIds = new List<string>();

            if (book != null && book.rows != null && book.rows.Count > 0)
            {
                userIds = book.rows.Select(s => s.UID).ToList();
            }

            return Json(userIds, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBeMyBookList(string curPage)
        {
            var newClient = new JsonServiceClient(http + "/SNSApi/");

            var userId = loginId.ToString().ToLower();

            StringPageResult book =
                newClient.Get(new GetBeConcerned
                {
                    UID = userId,
                    CurPage = Convert.ToInt32(curPage) - 1,
                    PageSize = 15
                });

            return Json(book, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveMyBook(string removeUserId)
        {
            var reUserId = new Guid(removeUserId);

            UCHomeBasePage ucbBasePage = new UCHomeBasePage();
            Guid adduser = user.userid;
            UCHomeEntities uc = new UCHomeEntities();
            View_Simple_User vsu = uc.View_Simple_User.SingleOrDefault(u => u.userid == reUserId);
            if (vsu != null)
            {
                UCHome_Attention attent =
                    uc.UCHome_Attention.FirstOrDefault(u => u.AddUser == adduser && u.AttenUser == vsu.userid);
                if (attent != null)
                {
                    uc.UCHome_Attention.DeleteObject(attent);
                    uc.SaveChanges();

                    var client1 = new JsonServiceClient(http + "/SNSApi/");
                    DeleteAddrBook book = new DeleteAddrBook
                    {
                        GroupID = user.userid.ToString(),
                        UID = vsu.userid.ToString()
                    };

                    client1.Delete(book);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddMyBook(string addUserId)
        {
            var aUserId = new Guid(addUserId);

            UCHomeBasePage ucbBasePage = new UCHomeBasePage();
            Guid adduser = user.userid;
            UCHomeEntities uc = new UCHomeEntities();
            View_Simple_User vsu = uc.View_Simple_User.SingleOrDefault(u => u.userid == aUserId);
            if (vsu != null)
            {
                UCHome_Attention attent = new UCHome_Attention();
                attent.PKID = Guid.NewGuid();
                attent.AddUser = adduser;
                attent.AttenUser = vsu.userid;
                attent.AttenName = vsu.username; attent.AttenTime = DateTime.Now;
                attent.AttenIdentity = vsu.usertype.ToUpper();
                uc.UCHome_Attention.AddObject(attent);
                uc.SaveChanges();

                AddAddrBookEntry book = new AddAddrBookEntry
                {
                    GroupID = user.userid.ToString(),
                    GroupName = "我的关注",
                    UID = vsu.userid.ToString(),
                    UName = vsu.username
                };

                client.Send<AddAddrBookEntry>(book);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAreaList()
        {
            var areaList = _uc.ExecuteStoreQuery<Data>("select CODE_ID as Id,CODE_NAME as Name from GBT2260 where CODE_ID>370900 and CODE_ID<371000").ToList();;

            return Json(areaList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSchoolTypeList()
        {
            var typeList = _uc.ExecuteStoreQuery<Data>("select CodeID as Id,CodeName as Name from Basic_SysDict where CodeType='学校类型' order by CodeID").ToList();

            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchoolList(string areaId,string typeId)
        {
            var query =_uc.Basic_ZZXX0101.Where(c => 1==1);

            if (!string.IsNullOrWhiteSpace(areaId))
            {
                query = query.Where(c => c.XZQHM == areaId);
            }
            if (!string.IsNullOrWhiteSpace(typeId))
            {
                query = query.Where(c => c.SchoolType == typeId);
            }

            return Json(query.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
