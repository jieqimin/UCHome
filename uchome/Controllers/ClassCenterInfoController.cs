using ClassCenterBLL;
using ClassCenterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ServiceModel;
using ServiceStack.Service;
using ServiceStack.ServiceClient.Web;
using TeacherInfo.Model;
using UCHome.Model;
using System.IO;

namespace UCHome.Controllers
{
    public class ClassCenterInfoController : Controller
    {
        private static string http = System.Web.Configuration.WebConfigurationManager.AppSettings["APIHttp"];
        private static string images_head = System.Web.Configuration.WebConfigurationManager.AppSettings["images_head"];
        private IServiceClient client = new JsonServiceClient(http + "/SNSApi/");
        UCHomeEntities ue = new UCHomeEntities();
        ClassCenterInfo msl = new ClassCenterInfo();
        private readonly UCHomeBasePage page = new UCHomeBasePage();
        ClassEntities db = new ClassEntities();
        ZZ_MIFVSEntities entity = new ZZ_MIFVSEntities();
        #region 属性
        private static UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        private Guid userid
        {
            get { return user.userid; }
        }
        private string userName
        {
            get { return user.loginname; }
        }
        private Guid XXID
        {
            get { return user.xxid; }
        }
        private string DisPlayName
        {
            get { return user.username; }
        }
        private string XXMC
        {
            get { return user.xxmc; }
        }
        private string XNXQ
        {
            get { return page.CurrentSchTerm; }
        }
        #endregion

        public ActionResult ClassIndex(string bjid,string bjmc,string urlType="")
        {
            ViewBag.UrlType = urlType;
            ViewBag.Title = "班级空间";          
            ViewBag.BJMC = bjmc;
            ViewBag.XNXQ = XNXQ;
            Guid BJID=Guid.Parse(bjid);
            var xxid = db.Basic_ZZJX0202.FirstOrDefault(x => x.BJID == BJID);
            Guid shid = xxid.XXID.Value;
            ViewBag.XXID = xxid.XXID;
            Guid schoolid = shid;
            var viewBasicSchools = db.View_Basic_Schools.FirstOrDefault(x => x.XXID == schoolid);
            if (viewBasicSchools != null)
                ViewBag.XXMC = viewBasicSchools.XXMC;
            //var bjid = "29F9BBE5-5F14-4B5C-9CFB-17D1A7E5B14F";
            ViewBag.BJID = bjid;

            return View();
        }
        public ActionResult Loading()
        {
            return View();
        }
        public ActionResult Index()
        {
            ViewBag.ImageHead = images_head;
            var bjid = Request["BJID"];
            ViewBag.BJID = bjid;
            var list = msl.GetClassActiveByClassList(new Guid(bjid));
            return View(list);
        }
        public ActionResult Details()
        {
            var bjid = Request["BJID"];
            var pkid = Request["pkid"];
            ViewBag.PKID = pkid;
            Guid BJID = Guid.Parse(bjid);
            var info = ue.View_Class_teaandstuPhotos.Where(x => x.bjid == BJID).ToList();
            return View(info);
        }
        public ActionResult Mien()
        {
            var bjid = Request["BJID"];
            return View();
        }
        public ActionResult Member()
        {
            var bjid = Request["BJID"];
            Guid BJID = new Guid(bjid);
       
            return View();
        }
        public ActionResult TeacherPart()
        {
            var bjid = Request["BJID"];
            Guid BJID = new Guid(bjid);
            var info = ue.View_Class_Teacher.Where(x => x.bjid == BJID).ToList();
            if (info.Any())
            {
                foreach (var tea in info)
                {
                    var imageurl = "/upload/headimages/" + tea.jsid + ".jpg";

                    if (!System.IO.File.Exists("imageurl")) //判断头像图片是否存在
                    {
                        if (tea.XBM == "2")
                        {
                            imageurl = images_head + "/content/images/male.png";
                        }
                        else
                        {
                            imageurl = images_head + "/content/images/man.png";
                        }
                    }

                    tea.ZP = imageurl;
                }
            }
            return View(info);
        }
        public ActionResult StudentPart()
        {
            var bjid = Request["BJID"];
            Guid BJID = new Guid(bjid);
            var info = ue.View_Simple_StuInfo.Where(x => x.BJID == BJID).ToList();
            if (info.Any())
            {
                foreach (var stu in info)
                {
                    var imageurl = "/upload/headimages/" + stu.xsid + ".jpg";

                    if (!System.IO.File.Exists("imageurl")) //判断头像图片是否存在
                    {
                        if (stu.XBM == "2")
                        {
                            imageurl = images_head + "/content/images/male.png";
                        }
                        else
                        {
                            imageurl = images_head + "/content/images/man.png";
                        }
                    }

                    stu.ZP = imageurl;
                }
            }
            return View(info);
        }
        public ActionResult Dynamic()
        {
            return View();
        }
        public ActionResult Honor()
        {
            ViewBag.Year = DateTime.Now.Year;
            return View();
        }
        public ActionResult Culture(Guid bjid)
        {
            var  stu = ue.View_Simple_StuInfo.FirstOrDefault(u => u.xsid == userid && u.BJID==bjid);
            var checkClass = false;
            if (stu != null)
            {
                checkClass = true;
            }
            return View(checkClass);
        }
        public ActionResult Active()
        {
            var bjid = Request["BJID"];
            return View();
        }

        public JsonResult GetActive(Guid bjid, int page)
        {
            var active = msl.GetClassActiveByClassList(bjid, page - 1, 8);

            return Json(active, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHonor(Guid bjid, string year)
        {
            var honorList = msl.GetClassHonorByBJIDList(bjid, year);

            return Json(honorList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCicrle(Guid bjid,int page)
        {
            var month = DateTime.Now.Month.ToString();

            if (month.Length == 1)
            {
                month = "0" + month;
            }

            var yearMonth = DateTime.Now.Year + "/" + month;

            var cicrleList = msl.GetClassCicrleByClassList(bjid, yearMonth, page - 1, 9, images_head);

            return Json(cicrleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModthBirStu(Guid bjid)
        {
            var month = DateTime.Now.Month.ToString();

            if (month.Length == 1)
            {
                month = "0" + month;
            }

            var yearMonth = DateTime.Now.Year + "/" + month;

            var stuList =
                ue.View_Simple_StuInfo.Where(
                    u => u.BJID == bjid && u.SFZJH.Length > 15 && u.SFZJH.Substring(6, 6) == yearMonth);

            if (stuList.Any())
            {
                foreach (var stu in stuList)
                {
                    var imageurl = "/upload/headimages/" + stu.xsid + ".jpg";

                    if (!System.IO.File.Exists("imageurl")) //判断头像图片是否存在
                    {
                        if (stu.XBM == "2")
                        {
                            imageurl = images_head + "/content/images/male.png";
                        }
                        else
                        {
                            imageurl = images_head + "/content/images/man.png";
                        }
                    }

                    stu.ZP = imageurl;
                }
            }

            return Json(stuList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostCicrleCreate(Guid bjid, string content)
        {
            var cicrle = new ClassCicrle
            {
                ID = Guid.NewGuid(),
                XXID = XXID,
                BJID = bjid,
                UserID = userid,
                UserName = DisPlayName,
                CurrentDate = DateTime.Now.ToString("yyyy/MM/dd"),
                Content = HttpUtility.UrlDecode(content),
                CreateTime = DateTime.Now.ToString(),
                Creator = userName
            };

            var result = msl.PostCirleCreate(cicrle);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActiveDetails(Guid id)
        {
            var active = msl.GetClassActiveInfo(id);
            return View(active);
        }

        public ActionResult Circle()
        {
            ViewBag.LoginId = userid;

            var curPage = Request["curPage"];
            var bjid = Request["BJID"];

            var newClient = new JsonServiceClient(http + "/SNSApi/");

            var bId = bjid.ToLower();

            List<SNSFeedEntryDto> resList =
                newClient.Get(new GetGroupSNSFeeds
                {
                    GroupID = bId,
                    //UID=userid.ToString(),
                    CurPage = Convert.ToInt32(curPage),
                    PageSize = 5
                });

            Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
            foreach (var res in resList)
            {
                res.Summary = regex.Replace(res.Summary ?? "", "");
                res.ObjectID = userid.ToString();
                res.School = (Convert.ToInt32(curPage) * 10).ToString();
            }

            return PartialView("Circle", resList);
        }

        public JsonResult PostUserAction(string bjid)
        {
            AddUserActionEntry comment = new AddUserActionEntry
            {
                Id = Guid.NewGuid(),
                ObjectType = "班级空间",
                ObjectID = bjid,
                Action = "View",
                UID = userid.ToString(),
                UName = DisPlayName,
                School = XXMC,
                Date = DateTime.Now,
                UserHostAddress = Request.UserHostAddress
            };

            client.Send<AddUserActionEntry>(comment);

            return Json(comment, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserAction(string bjid, int curPage, int pageSize)
        {
            var newClient = new JsonServiceClient(http + "/SNSApi/");

            List<UserActiveEntryDto> activeList =
                newClient.Get(new GetUserAction
                {
                    ObjectType = "班级空间",
                    ObjectID = bjid,
                    Action = "View",
                    CurPage = Convert.ToInt32(curPage),
                    PageSize = pageSize
                });

            if (activeList.Any())
            {
                foreach (var act in activeList)
                {
                    act.UserHostAddress = act.Date.ToString("yyyy.MM.dd hh:mm");
                }
            }

            return Json(activeList, JsonRequestBehavior.AllowGet);
        }
    }
}
