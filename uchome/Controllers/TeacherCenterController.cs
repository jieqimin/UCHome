using System.Globalization;
using System.Security.Permissions;
using ServiceStack.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ServiceModel;
using ServiceStack.ServiceClient.Web;
using ServiceStack.Text;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class TeacherCenterController : Controller
    {
        private static string http = System.Web.Configuration.WebConfigurationManager.AppSettings["APIHttp"];
        private IServiceClient client = new JsonServiceClient(http + "/SNSApi/");
        static readonly UCHomeBasePage ucbase = new UCHomeBasePage();

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
        private string userName
        {
            get
            {
                try
                {
                    return user.username;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }
        public ActionResult FriendsCircle()
        {
            ViewBag.LoginId = loginId;

            return PartialView("FriendsCircle");
        }

        public ActionResult Circle()
        {
            ViewBag.LoginId = loginId;

            var curPage = Request["curPage"];

            var newClient = new JsonServiceClient(http + "/SNSApi/");

            var userId = loginId.ToString().ToLower();

            List<SNSFeedEntryDto> resList =
                newClient.Get(new GetSNSFeeds
                {
                    UID = userId,
                    CurPage = Convert.ToInt32(curPage),
                    PageSize = 10
                });

            Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
            foreach (var res in resList)
            {
                res.Summary = regex.Replace(res.Summary ?? "", "");
                res.ObjectID = loginId.ToString();
                res.School = (Convert.ToInt32(curPage) * 10).ToString(CultureInfo.InvariantCulture);
            }

            return PartialView("Circle", resList);
        }

        /// <summary>
        /// 提交一级评论
        /// </summary>
        /// <param name="fid">资源ID</param>
        /// <param name="content">评论内容</param>
        /// <returns></returns>
        public JsonResult PostParentComment(string fid, string content)
        {
            AddCommentEntry comment = new AddCommentEntry
            {
                FId = fid,
                CId = Guid.NewGuid().ToString(),
                Content = content,
                UID = loginId.ToString(),
                UName = userName,
                Date = DateTime.Now
            };

            client.Send<AddCommentEntry>(comment);

            comment.FId = comment.Date.Month + "月" + comment.Date.Day + "日 " + comment.Date.Hour + ":" +
                          comment.Date.Minute;

            return Json(comment, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 提交二级评论
        /// </summary>
        /// <param name="fid">资源ID</param>
        /// <param name="content">评论内容</param>
        /// <param name="parentId">父级ID</param>
        /// <returns></returns>
        public JsonResult PostComment(string fid, string content, string parentId)
        {
            AddCommentEntry comment = new AddCommentEntry
            {
                FId = fid,
                CId = Guid.NewGuid().ToString(),
                Content = content,
                UID = loginId.ToString(),
                UName = userName,
                PId = new Guid(parentId),
                Date = DateTime.Now
            };

            client.Send<AddCommentEntry>(comment);

            comment.FId = comment.Date.Month + "月" + comment.Date.Day + "日 " + comment.Date.Hour + ":" +
                          comment.Date.Minute;

            return Json(comment, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 赞
        /// </summary>
        /// <param name="fid">资源ID</param>
        /// <returns></returns>
        public JsonResult PostLike(string fid)
        {
            AddLikeEntry like = new AddLikeEntry
            {
                FId = new Guid(fid),
                UID = loginId.ToString(),
                UName = userName
            };

            client.Send<AddLikeEntry>(like);

            return Json(like, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除动态
        /// </summary>
        /// <param name="fid">资源ID</param>
        /// <returns></returns>
        public JsonResult DeleteCirle(string fid)
        {
            var client1 = new JsonServiceClient(http + "/SNSApi/");
            DeleteSNSFeeds like = new DeleteSNSFeeds
            {
                FId = new Guid(fid)
            };

            client1.Delete(like);



            return Json(like, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="cid">评论ID</param>
        /// <returns></returns>
        public JsonResult DeleteComments(string fid, string cid)
        {
            bool result = false;

            try
            {
                var client1 = new JsonServiceClient(http + "/SNSApi/");

                DeleteComment comment = new DeleteComment
                {
                    FId = new Guid(fid),
                    CID = cid
                };

                client1.Delete(comment);

                result = true;
            }
            catch (Exception)
            {

                throw;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View(loginId);
        }

        public ActionResult TeacherTask()
        {
            return View(loginId);
        }
        public ActionResult Query(Guid id)
        {
            UserInfo u = ucbase.GetUserInfoByRequestId(id);
            return View(u);
        }
        public ActionResult PersonHome(Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_BaseInfo ubi = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == id);
            ViewData["usertype"] = ubi != null ? ubi.UserType.ToUpper() : "";
            return PartialView("PersonHome", loginId);
        }
        public ActionResult PersonHome2(Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_BaseInfo ubi = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == id);
            ViewData["usertype"] = ubi != null ? ubi.UserType.ToUpper() : "";
            return PartialView("PersonHome2", id);
        }
        public ActionResult PersonKB(Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            Guid jsid = id;
            View_Simple_TeaInfo tea = uc.View_Simple_TeaInfo.SingleOrDefault(u => u.jsid == jsid);
            return PartialView("PersonKB", tea);
        }
        public ActionResult PersonInfo(Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            Guid jsid = id;//LoginSet.Consts.CurrentUser.ID;
            TeacherAllInfo tai = new TeacherAllInfo();
            View_Simple_TeaInfo tea = uc.View_Simple_TeaInfo.SingleOrDefault(u => u.jsid == jsid);
            if (tea != null) tai.VST = tea; else tai.VST = new View_Simple_TeaInfo();
            UCHome_BaseInfo ubi = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == jsid);
            if (ubi != null) tai.UBI = ubi; else tai.UBI = new UCHome_BaseInfo();
            return PartialView("PersonInfo", tai);
        }
        public ActionResult PersonInfo2(Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            Guid jsid = id;//LoginSet.Consts.CurrentUser.ID;
            TeacherAllInfo tai = new TeacherAllInfo();
            View_Simple_TeaInfo tea = uc.View_Simple_TeaInfo.SingleOrDefault(u => u.jsid == jsid);
            if (tea != null) tai.VST = tea; else tai.VST = new View_Simple_TeaInfo();
            UCHome_BaseInfo ubi = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == jsid);
            if (ubi != null) tai.UBI = ubi; else tai.UBI = new UCHome_BaseInfo();
            return PartialView("PersonInfo2", tai);
        }
        public ActionResult Studentlist(string bjid, Guid? id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            MyClass myclass = new MyClass();

            if (string.IsNullOrEmpty(bjid))
            {
                if (id != null)
                {
                    Guid jsid = id.Value;
                    var techlist = uc.View_Class_TeaInfo.Where(u => u.jsid == jsid && u.xnxqid == ucbase.CurrentSchTerm).AsEnumerable().Select(u => new MyClass { xxid = new Guid(u.xxid.ToString()), bjid = u.bjid, bjmc = u.xzbmc }).Distinct().ToList();
                    myclass = techlist.FirstOrDefault();
                }
            }
            else
            {
                View_Simple_SchoolClass schclass = uc.View_Simple_SchoolClass.SingleOrDefault(b => b.bjid == new Guid(bjid));
                myclass.bjid = new Guid(bjid);
                if (schclass != null)
                {
                    if (schclass.xxid != null) myclass.xxid = schclass.xxid.Value;
                    myclass.bjmc = schclass.xzbmc;
                }
            }
            return PartialView("Studentlist", myclass);
        }
        public ActionResult Parentlist(string bjid, Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            MyClass myclass = new MyClass();

            if (string.IsNullOrEmpty(bjid))
            {
                Guid jsid = id;
                var techlist = uc.View_Class_TeaInfo.Where(u => u.jsid == jsid && u.xnxqid == ucbase.CurrentSchTerm).AsEnumerable().Select(u => new MyClass { xxid = new Guid(u.xxid.ToString()), bjid = u.bjid, bjmc = u.xzbmc }).Distinct().ToList();
                myclass = techlist.FirstOrDefault();
            }
            else
            {
                View_Simple_SchoolClass schclass = uc.View_Simple_SchoolClass.SingleOrDefault(b => b.bjid == new Guid(bjid));
                myclass.bjid = new Guid(bjid);
                if (schclass != null)
                {
                    if (schclass.xxid != null) myclass.xxid = schclass.xxid.Value;
                    myclass.bjmc = schclass.xzbmc;
                }
            }
            return PartialView("Parentlist", myclass);
        }

        public ActionResult staticnews(Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            Guid jsid = id;//new Guid("efcf1840-e5d6-42cd-a1a3-5f457bc680bf"); //LoginSet.Consts.CurrentUser.ID;
            View_Simple_User tea = uc.View_Simple_User.SingleOrDefault(u => u.userid == jsid);
            return PartialView("staticnews", tea);
        }

        public ActionResult ShowResource(string learntype)
        {
            string ltype = HttpUtility.UrlDecode(learntype);
            ViewBag.LearnType = ltype;
            return PartialView("ShowResource");
        }
        public ActionResult ResourcesList(string userType, string elementName, string elementValue, string userid, int top)
        {
            ViewBag.userType = userType;
            ViewBag.elementName = elementName;
            ViewBag.elementValue = elementValue;
            ViewBag.userid = userid;
            ViewBag.top = top;
            return PartialView(null);
        }
        public class TeacherAllInfo
        {
            public View_Simple_TeaInfo VST { get; set; }
            public UCHome_BaseInfo UBI { get; set; }
        }
    }
}
