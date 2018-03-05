using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using UCHome.BLL;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.Controllers
{
    public class CityCenterController : Controller
    {
        //
        // GET: /TeacherCenter/
        readonly UCHomeEntities _uc;
        private readonly UCResourceEntities _rec;
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        public CityCenterController()
        {
            _uc = new UCHomeEntities();
            _rec = new UCResourceEntities();
        }
        public ActionResult Index()
        {


            return View();
        }

        public ActionResult NewsList()
        {
            return PartialView("NewsList");
        }
        public ActionResult NewArticles(int pageSize)
        {
            var list =
                _uc.View_PersonArticle2
                .Where(c => c.UCType == "Article" && c.IsShow == 1 && c.IsAudit == 4)
                .OrderByDescending(c => c.DeployTime)
                .Take(pageSize)
                .AsEnumerable()
                .Select(c => new NewArticle(c.PKID, c.Title, c.DeployTime, c.username))
                .ToList();
            return PartialView("NewArticles", list);
        }
        public ActionResult HotArticles(int pageSize)
        {
            var list =
                _uc.View_PersonArticle2
                .Where(c => c.UCType == "Article" && c.IsShow == 1 && c.IsAudit == 4)
                    .OrderByDescending(c => c.Hits)
                    .ThenByDescending(c => c.DeployTime)
                    .Take(pageSize)
                    .AsEnumerable()
                    .Select(c => new HotArticles(c.PKID, c.Title, c.DeployTime, c.username))
                    .ToList();
            return PartialView("HotArticles", list);
        }
        public ActionResult Resources(int pageSize)
        {
            //var list =
            //    _rec.V_ResourceByLearn
            //        .OrderByDescending(c => c.CreateTime)
            //        .Take(pageSize).AsEnumerable()
            //        .Select(c => new Resources(c.id, c.Name, c.CreateTime, c.Hits, c.Grade, c.Subject, c.XXMC, c.CreatorName))
            //        .ToList();
            return PartialView("Resources");
        }

        public ActionResult NewArticle(int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            var list = topbll.GetArticleNewTop(5);
            return PartialView("NewArticle", list);
        }

        public ActionResult NewArticleList(int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            var list = topbll.GetArticleNewTop(10);
            return PartialView("NewArticle", list);
        }

        public ActionResult HotArticle(int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            var list = topbll.GetArticleHitsTop(5);
            return PartialView("HotArticle", list);
        }


        public ActionResult HotArticleList(int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            var list = topbll.GetArticleHitsTop(10);
            return PartialView("HotArticle", list);
        }

        public ActionResult RoomsList(string userType, int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            List<UCHome_Top_Data> list;
            if (userType.ToLower() == "t")
            {
                list = topbll.GetTeaArticlesTop(7);
                ViewBag.UserType = "t";
            }
            else
            {
                list = topbll.GetStuArticlesTop(7);
                ViewBag.UserType = "s";
            }
            return PartialView("RoomsList", list);
        }

        public ActionResult GoodStuAndTeaList(string userType, int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            List<UCHome_Top_Data> list;
            if (userType.ToLower() == "t")
            {
                list = topbll.GetTeaArticlesTop(9);
                ViewBag.UserType = "t";
            }
            else
            {
                list = topbll.GetStuArticlesTop(9);
                ViewBag.UserType = "s";
            }
            return PartialView("GoodStuAndTeaList", list);
        }

        //
        //教研活动 教师培训
        public ActionResult ResourceList(string learnType, int pageSize)
        {
            var list =
                _rec.V_ResourceByLearn.Where(c => c.learntype == learnType)
                    .OrderByDescending(c => c.CreateTime)
                    .Take(pageSize).AsEnumerable()
                    .Select(c => new MyResource(c.id, c.Name, c.CreateTime, c.Hits, c.Grade, c.Subject, c.XXMC, c.CreatorName, c.LinkUrl))
                    .ToList();
            return PartialView("ResourceList", list);
        }
        //精品课程
        public ActionResult CourseList(string learnType, int pageSize)
        {
            var list =
                _rec.V_ResourceByLearn.Where(c => c.learntype == learnType)
                    .OrderByDescending(c => c.CreateTime)
                    .Take(pageSize).AsEnumerable()
                    .Select(c => new MyResource(c.id, c.Name, c.CreateTime, c.Hits, c.Grade, c.Subject, c.XXMC, c.CreatorName, c.LinkUrl))
                    .ToList();
            return PartialView("CourseList", list);
        }


        //教学设计  //课堂实录  //教学课件
        public JsonResult GetTeachingDesign(Guid type)
        {
            //  http://192.168.99.137/RedisCache.Api/api/resource/cityRes/byCreateTimeDesc/DED65366-CCE1-4A42-AB1A-95F103FF97AD
            string apiurl = APIHelper.GetApiUrl("CityResource");
            var sum = apiurl + "/" + type;
            var resultdata = HttpClientHelper.GETMethod(sum);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }

        //市级的资源总数
        public JsonResult GetSumResource()
        {

            string apiurl = APIHelper.GetApiUrl("CityResourceCount");
            var sum = apiurl;
            var resultdata = HttpClientHelper.GETMethod(sum);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
        //获取课件
        public JsonResult GetSumLessonPlan(Guid subject, string pageSize, string pageIndex)
        {
            string apiurl = APIHelper.GetApiUrl("GetLessonPlan");
            var postData = new
           {
               subject,
               pageSize,
               pageIndex
           };
            var resultdata = HttpClientHelper.POSTMethod(apiurl, postData);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }

        //获取全市教案数
        public JsonResult GetSumLessonCount(string pageSize, string pageIndex)
        {
            string apiurl = APIHelper.GetApiUrl("GetLessonPlan");
            var postData = new
            {
                pageSize,
                pageIndex
            };
            var resultdata = HttpClientHelper.POSTMethod(apiurl, postData);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }

        //获取资源统计GetSumEbookStatic
        public JsonResult GetSumRescourseStatic()
        {
            UCResourceEntities ucr = new UCResourceEntities();
            var rescourseStatic = ucr.View_StatisticAreaShiCount.ToList();
            return Json(rescourseStatic, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSumEbookStatic()
        {
            using (UCHomeEntities uc = new UCHomeEntities())
            {
                var rescourseStatic = uc.UCHome_Statics_Results.Where(e => e.SType == "ebooks" && e.SGroup == "area" && e.SObjectID != "370900").OrderBy(s => s.SObjectID).ToList();
                return Json(rescourseStatic, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult GetResource(string conditionType, Guid condition, string format = "", string type = "", string page = "")
        {
            string apiurl = APIHelper.GetApiUrl("getresourcebywhere");
            apiurl = apiurl + "?conditionType=" + conditionType
               + "&condition=" + condition + "&format=" + format + "&type=" + type + "&page=" + page;
            var resultdata = HttpClientHelper.POSTMethod(apiurl, null);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserdegree()
        {
            using (UCHomeEntities uc = new UCHomeEntities())
            {
                List<UCHome_Statics_Data> homeslist =
    uc.ExecuteStoreQuery<UCHome_Statics_Data>(
        "select SObjectID,SobjName,((select 5*SResults from UCHome_Statics_Results where SType='ebooks' " +
        "and SObjectID=a.SObjectID)+(select SResults from UCHome_Statics_Results where SType='articles' " +
        "and usertype='t' and SObjectID=a.SObjectID)+(select 2*SResults from UCHome_Statics_Results " +
        "where SType='resources' and SObjectID=a.SObjectID))/(select SResults from UCHome_Statics_Results " +
        "where SType='users'and usertype='t' and SObjectID=a.SObjectID) Sresults from UCHome_Statics_Results a where" +
        " SGroup='area'  and usertype='t' and SObjectID<>'370900' group by SObjectID,SobjName")
        .ToList();
                return Json(homeslist, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
