using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCHome.BLL;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.Controllers
{
    public class DistrictCenterController : Controller
    {
        //
        // GET: /TeacherCenter/
        readonly UCHomeEntities _uc;
        private readonly UCResourceEntities _rec;



        public DistrictCenterController()
        {
            _uc = new UCHomeEntities();
            _rec = new UCResourceEntities();
        }
        private void Init()
        {
            string xzqhm = "";
            if (Request.RequestContext.RouteData.Values["id"] != null)
            {
                xzqhm = Request.RequestContext.RouteData.Values["id"].ToString();
            }
            var result = _uc.GBT2260.SingleOrDefault(x => x.CODE_ID == xzqhm);
            if (result != null)
            {
                ViewBag.districtName = result.CODE_NAME;
            }
        }
        public ActionResult Index()
        {
            var areaid = Request.RequestContext.RouteData.Values["id"];

            ViewBag.areaid = areaid;

            Init();
            return View();
        }

        public ActionResult NewsList()
        {
            return PartialView("NewsList");
        }

        public ActionResult NewArticle(string xzqhm, int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            var list = topbll.GetArticleNewTop(xzqhm, 5);
            return PartialView("NewArticle", list);
        }

        public ActionResult HotArticle(string xzqhm, int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            var list = topbll.GetArticleHitsTop(xzqhm, 5);
            return PartialView("HotArticle", list);
        }

        public ActionResult RoomsList(string xzqhm, string userType, int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            List<UCHome_Top_Data> list;
            if (userType.ToLower() == "t")
            {
                list = topbll.GetTeaArticlesTop(xzqhm, 7);
                ViewBag.UserType = "t";
            }
            else
            {
                list = topbll.GetStuArticlesTop(xzqhm, 7);
                ViewBag.UserType = "s";
            }
            return PartialView("RoomsList", list);
        }

        public ActionResult ResourceList(string xzqhm, string learnType, int pageSize)
        {
            var list =
                _rec.V_ResourceByLearn
                .Where(c => c.learntype == learnType && c.XZQHM == xzqhm)
                .OrderByDescending(c => c.CreateTime)
                .Take(pageSize)
                .AsEnumerable()
                .Select(c => new MyResource(c.id, c.Name.Trim(), c.CreateTime, c.Hits, c.Grade, c.Subject, c.XXMC, c.CreatorName, c.LinkUrl))
                .ToList();
            return PartialView("ResourceList", list);
        }

        public ActionResult CourseList(string xzqhm, string learnType, int pageSize)
        {
            string xx = xzqhm;
            var list =
                _rec.V_ResourceByLearn.Where(c => c.learntype == learnType && c.XZQHM == xzqhm)
                    .OrderByDescending(c => c.CreateTime)
                    .Take(pageSize).AsEnumerable()
                    .Select(c => new MyResource(c.id, c.Name.Trim(), c.CreateTime, c.Hits, c.Grade, c.Subject, c.XXMC, c.CreatorName, c.LinkUrl))
                    .ToList();
            return PartialView("CourseList", list);
        }

        [HttpGet]
        public JsonResult GetSchools(string xzqhm, int pageSize, int? schoolType, string key)
        {
            string schoolTypes;
            schoolTypes = schoolType == 1 ? ",1,4,7,"
                : schoolType == 2 ? ",2,3,4,5," : ",4,6,";
            var list =
                _uc.Basic_ZZXX0101.Where(
                    c => c.XZQHM == xzqhm && c.XXMC.Contains(key) && schoolTypes.Contains("," + c.SchoolType + ","))
                    .AsEnumerable()
                    .Select(c => new NewSchool8(c.XXID, c.XXMC)).Take(pageSize);
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SchoolRank(string xzqhm, int pageSize)
        {
            var query = _uc.View_PersonArticle.Where(c => c.UCType == "Article" && c.XZQHM == xzqhm);
            var list = (from t in query
                        group t by new { t.xxid, t.xxmc }
                            into g
                            orderby g.Count() descending
                            select new SchoolRank() { XXID = g.Key.xxid, XXMC = g.Key.xxmc, Hits = g.Count(), Title = g.Key.xxmc.Length > 12 ? g.Key.xxmc.Substring(0, 12) + "..." : g.Key.xxmc }).Take(pageSize).ToList();
            return PartialView("SchoolRank", list);
        }

        public JsonResult AreaResource(string AreaCode, Guid ResourceType)
        {
            string apiurl = APIHelper.GetApiUrl("AreaResource");
            var sum = apiurl + "/" + AreaCode + "/" + ResourceType;
            var resultdata = HttpClientHelper.GETMethod(sum);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
    }
}
