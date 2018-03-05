using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Basic.Model;
using ServiceModel;
using ServiceStack.ServiceClient.Web;
using UCHome.Model;
using Basic_ZZXX0101 = UCHome.Model.Basic_ZZXX0101;
using GBT2260 = UCHome.Model.GBT2260;

namespace UCHome.Controllers
{
    public class QueryController : Controller
    {
        //
        // GET: /TeacherCenter/
        private static readonly string http = WebConfigurationManager.AppSettings["APIHttp"];
        private readonly UCHomeEntities _uc;
        private readonly SchoolEntities _sch = new SchoolEntities();
        private Model.UserInfo user
        {
            get
            {
                return UCHomeBasePage.RequestUser;
            }
        }
        private Guid userid
        {
            get
            {
                return user.userid;
            }
        }
        public QueryController()
        {
            _uc = new UCHomeEntities();
        }

        public JsonResult DoFresh()
        {
            using (UCHomeEntities uc = new UCHomeEntities())
            {
                uc.ExecuteStoreCommand("exec Pro_UCHome_Statics");
                JsonResult jsonResult = new JsonResult
           {
               Data = "success"
           };
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult DataStatics()
        {
            var list = _uc.ExecuteStoreQuery<Count>("select '1' as [Key], COUNT(1) Value from UCHome_BaseInfo union all select '2' as [Key],COUNT(1) from UCHome_PersonNew where UCType='Article' AND ISSHOW=1").ToList();
            ViewBag.zoom = list.FirstOrDefault(m => m.Key == "1").Value;
            ViewBag.article = list.FirstOrDefault(m => m.Key == "2").Value;
            ViewBag.total = list.FirstOrDefault(m => m.Key == "2").Value * 2;
            return PartialView("DataStatics");
        }
        public ActionResult AllStatics()
        {
            return View();
        }
        public ActionResult AllStaticsByArea(string areacode)
        {
            UCHomeEntities ue = new UCHomeEntities();
            GBT2260 area = ue.GBT2260.SingleOrDefault(u => u.CODE_ID == areacode);
            return View(area);
        }
        public ActionResult AllStaticsBySch(Guid xxid)
        {
            return View(xxid);
        }
        public ActionResult NoActiveBySch(Guid xxid)
        {
            return View(xxid);
        }
        public ActionResult AllHomesList()
        {
            using (UCHomeEntities uc = new UCHomeEntities())
            {
                List<UCHome_Statics_Data> homeslist =
    uc.ExecuteStoreQuery<UCHome_Statics_Data>(
        "select sobjectid,sobjname,sum(sresults) sresults from UCHome_Statics_Results where stype='spacealive' and sgroup='area' and sobjectid<>'370900' group by stype,sgroup,sobjectid,sobjname order by sobjectid")
        .ToList();
                return PartialView(homeslist);
            }
        }

        public JsonResult AllHomesCount()
        {
            using (UCHomeEntities uc = new UCHomeEntities())
            {
                List<UCHome_Statics_Data> homeslist =
    uc.ExecuteStoreQuery<UCHome_Statics_Data>(
        "select sobjectid,sobjname,sum(sresults) sresults from UCHome_Statics_Results where stype='spacealive' and sgroup='area' and sobjectid<>'370900' group by stype,sgroup,sobjectid,sobjname order by sobjectid")
        .ToList();
                return Json(homeslist, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AllHomesPie()
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<UCHome_Statics_Data> homeslist =
                uc.ExecuteStoreQuery<UCHome_Statics_Data>(
                    "select sobjectid,sobjname,sum(sresults) sresults from UCHome_Statics_Results where stype='spacealive' and sgroup='area' and sobjectid<>'370900' group by stype,sgroup,sobjectid,sobjname order by sobjectid")
                    .ToList();
            return PartialView(homeslist);
        }
        public ActionResult AllHomesColumns()
        {
            using (UCHomeEntities uc = new UCHomeEntities())
            {
                List<UCHome_Statics_Results> homeslist =
    uc.ExecuteStoreQuery<UCHome_Statics_Results>(
        "select * from UCHome_Statics_Results where stype='spacealive' and sgroup='area' and sobjectid<>'370900' order by sobjectid")
        .ToList();
                return PartialView(homeslist);
            }
        }
        public ActionResult AllArticlesList()
        {
            using (UCHomeEntities uc = new UCHomeEntities())
            {
                List<UCHome_Statics_Data> articleslist =
    uc.ExecuteStoreQuery<UCHome_Statics_Data>(
        "select sobjectid,sobjname,sum(sresults) sresults from UCHome_Statics_Results where stype='articles' and sgroup='area' and sobjectid<>'370900' group by stype,sgroup,sobjectid,sobjname order by sobjectid")
        .ToList();
                return PartialView(articleslist);
            }
        }

        public JsonResult AllArticlesCount()
        {
            using (UCHomeEntities uc = new UCHomeEntities())
            {
                List<UCHome_Statics_Data> articleslist =
    uc.ExecuteStoreQuery<UCHome_Statics_Data>(
        "select sobjectid,sobjname,sum(sresults) sresults from UCHome_Statics_Results where stype='articles' and sgroup='area' and sobjectid<>'370900' group by stype,sgroup,sobjectid,sobjname order by sobjectid")
        .ToList();
                return Json(articleslist, JsonRequestBehavior.AllowGet);
            }
        }


       // [OutputCache(Duration = 3600, VaryByParam = "areacode")]
        public ActionResult AllArticlesListByArea(string areacode)
        {
            ViewBag.SchList = _sch.Basic_ZZXX0101.Where(s => s.XZQHM == areacode).OrderBy(s=>s.XXMC).ToList().Select(sch => new SelectListItem { Value = sch.XXID.ToString(), Text = sch.XXMC }).ToList();
            using (UCHomeEntities uc = new UCHomeEntities())
            {
                SqlParameter[] paras = new SqlParameter[1];
                paras[0] = new SqlParameter("@areacode", areacode.ToString());
                uc.CommandTimeout = 720;
                List<UCHome_Statics_Results> articleslist = uc.ExecuteStoreQuery<UCHome_Statics_Results>("select * from uchome_statics_results where SGroup='sch' and sparentobjectid=@areacode", paras).ToList();
                return PartialView(articleslist);
            }

        }
        [OutputCache(Duration = 3600, VaryByParam = "xxid")]
        public ActionResult AllArticlesListBySch(Guid xxid)
        {
            ViewBag.Cachetimebysch = DateTime.Now;
            UCHomeEntities uc = new UCHomeEntities();
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@schid", xxid.ToString());
            List<proc_uchome_getstaticbysch_Result> articleslist = uc.ExecuteStoreQuery<proc_uchome_getstaticbysch_Result>("exec proc_uchome_getstaticbysch @schid", paras).ToList();
            return PartialView(articleslist);
        }
        public ActionResult AllNoActiveBySch(Guid xxid)
        {
            ViewBag.Cachetimebysch = DateTime.Now;
            UCHomeEntities uc = new UCHomeEntities();
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@schid", xxid.ToString());
            List<proc_uchome_getnoactivebysch_Result> articleslist = uc.ExecuteStoreQuery<proc_uchome_getnoactivebysch_Result>("exec proc_uchome_getnoactivebysch @schid", paras).ToList();
            return PartialView(articleslist);
        }
        public ActionResult AllArticlesPie()
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<View_Statics_ArticleGroupByArea> articleslist = uc.View_Statics_ArticleGroupByArea.OrderBy(u => u.AreaCode).ToList();
            return PartialView(articleslist);
        }
        public ActionResult AllArticlesColumns()
        {
            using (UCHomeEntities uc = new UCHomeEntities())
            {
                List<UCHome_Statics_Results> articleslist =
    uc.ExecuteStoreQuery<UCHome_Statics_Results>(
        "select * from UCHome_Statics_Results where stype='articles' and sgroup='area' and sobjectid<>'370900' order by sobjectid")
        .ToList();
                return PartialView(articleslist);
            }
        }
        public ActionResult DayArticlesList()
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<View_Statics_DayArticleGroupByArea> articleslist = uc.View_Statics_DayArticleGroupByArea.OrderBy(u => u.AreaCode).ToList();
            return PartialView(articleslist);
        }

        public ActionResult DayArticlesPie()
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<View_Statics_DayArticleGroupByArea> articleslist = uc.View_Statics_DayArticleGroupByArea.OrderBy(u => u.AreaCode).ToList();
            return PartialView(articleslist);
        }
        public ActionResult DayArticlesColumns()
        {
            UCHomeEntities uc = new UCHomeEntities();
            List<View_Statics_DayArticleGroupByAreaAndUserType> articleslist = uc.View_Statics_DayArticleGroupByAreaAndUserType.OrderBy(u => u.AreaCode).ThenByDescending(u => u.usertype).ToList();
            return PartialView(articleslist);
        }
        public ActionResult ArticleInfo(Guid id)
        {
            View_PersonArticle article = _uc.View_PersonArticle.SingleOrDefault(c => c.PKID == id);
            if (article != null)
            {
                ArticleInfo result = new ArticleInfo
                {
                    PKID = article.PKID,
                    Title = article.Title,
                    DeployTime = article.DeployTime,
                    Author = article.username,
                    Hits = article.Hits.Value,
                    Content = article.Content,
                    AddUser = article.AddUser,
                    UserType = article.usertype
                };
                result.Hits = result.Hits + 1;
                ArticleUpdate(id);
                try
                {
                    var newClient = new JsonServiceClient(http + "/SNSApi/");
                    SNSFeedEntryDto snsarticle = newClient.Get(new GetSNSFeed
                    {
                        ObjectID = id.ToString()
                    });
                    //Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
                    ViewBag.Article = snsarticle;
                    //ViewBag.LoginId = userid;
                }
                catch (Exception)
                {
                    ViewBag.Article = new SNSFeedEntryDto();
                }
                return View("ArticleInfo", result);
            }
            return View("ArticleInfo", null);
        }
        private void ArticleUpdate(Guid pkid)
        {
            var ms_info = _uc.UCHome_PersonNew.SingleOrDefault(o => o.PKID == pkid);
            if (ms_info != null)
            {
                ms_info.Hits = ms_info.Hits + 1;
                _uc.SaveChanges();
            }
        }
        public ActionResult MoreArticle(string xzqhm, string schoolType, string xxid, string newtitle, int dataType)
        {
            ViewBag.xzqhm = xzqhm;
            ViewBag.schoolType = schoolType;
            ViewBag.xxid = xxid;
            ViewBag.NewTitle = newtitle;
            ViewBag.dataType = dataType;
            return View("MoreArticle");
        }
        public ActionResult MoreTeacher(string xzqhm, string schoolType, string xxid, string newtitle, int dataType)
        {
            ViewBag.xzqhm = xzqhm;
            ViewBag.schoolType = schoolType;
            ViewBag.xxid = xxid;
            ViewBag.NewTitle = newtitle;
            ViewBag.dataType = dataType;
            return View("MoreTeacher");
        }
        public ActionResult MoreClass(string xzqhm, string schoolType, string xxid, string newtitle, int dataType)
        {
            ViewBag.xzqhm = xzqhm;
            ViewBag.schoolType = schoolType;
            ViewBag.xxid = xxid;
            ViewBag.NewTitle = newtitle;
            ViewBag.dataType = dataType;
            return View("MoreClass");
        }
        public ActionResult MoreSchool(string xzqhm, string schoolType, string newtitle, int dataType)
        {
            ViewBag.xzqhm = xzqhm;
            ViewBag.schoolType = schoolType;
            ViewBag.dataType = dataType;
            ViewBag.NewTitle = newtitle;
            return View("MoreSchool");
        }
        public ActionResult MoreStudent(string xzqhm, string schoolType, string xxid, string newtitle, int dataType)
        {
            ViewBag.xzqhm = xzqhm;
            ViewBag.schoolType = schoolType;
            ViewBag.xxid = xxid;
            ViewBag.NewTitle = newtitle;
            ViewBag.dataType = dataType;
            return View("MoreStudent");
        }
        public ActionResult Part1(string xzqhm, string schoolType)
        {
            ViewBag.xzqhm = xzqhm;
            ViewBag.schoolType = schoolType;
            ViewBag.areaList = _uc.ExecuteStoreQuery<Data>("select CODE_ID as Id,CODE_NAME as Name from GBT2260 where CODE_ID>370900 and CODE_ID<371000").ToList();
            ViewBag.schoolTypeList = _uc.ExecuteStoreQuery<Data>("select CodeID as Id,CodeName as Name from Basic_SysDict where CodeType='学校类型' order by CodeID").ToList();
            return PartialView("Part1");
        }

        public ActionResult Part2(string key, string xzqhm, string schoolType, string xxid, int dataType)
        {
            int top = 12;
            if (dataType == 5)
            {
                top = 16;
            }
            key = HttpUtility.UrlDecode(key);
            var query = _uc.Basic_ZZXX0101.Where(c => c.XXMC.Contains(key));
            if (!string.IsNullOrWhiteSpace(xzqhm))
            {
                query = query.Where(c => c.XZQHM == xzqhm);
            }
            if (!string.IsNullOrWhiteSpace(schoolType))
            {
                query = query.Where(c => c.SchoolType == schoolType);
            }
            IQueryable<Basic_ZZXX0101> list;
            if (!string.IsNullOrWhiteSpace(xxid))
            {
                Guid schoolId = Guid.Parse(xxid);
                list = _uc.Basic_ZZXX0101.Where(c => c.XXID == schoolId)
                    .Concat(
                        query.Where(c => c.XXID != schoolId)
                        .Take(top)
                        );
            }
            else
            {
                list = query.Take(top);
            }
            ViewBag.schoolList = list.AsEnumerable().Select(c => new NewSchool(c.XXID, c.XXMC)).ToList();
            ViewBag.xxid = xxid;
            ViewBag.key = key;
            return PartialView("Part2");
        }
        public ActionResult PageTeacher(Guid xxid)
        {
            var query = _uc.UCHome_BaseInfo.Join(
               _uc.View_Simple_User,
               a => a.UserID,
               b => b.userid,
               (a, b) => new { a.UserType, a.CreateTime, a.IsStatus, b.username, b.xxid })
               .Where(u => u.xxid == xxid && u.UserType == "t" && u.IsStatus == "1");
            ViewBag.pageCount = query.Count();
            return PartialView("PageTeacher", xxid);
        }
        public ActionResult PageArticle(Guid xxid)
        {
            var query = _uc.UCHome_PersonNew.Join(
               _uc.View_Simple_User,
               a => a.AddUser,
               b => b.userid,
               (a, b) => new { a.PKID, a.Title, a.Abstract, a.DeployTime, a.AddUser, a.Hits, a.UCType, a.IsAudit, a.IsShare, a.IsShow, b.xxid })
               .Where(u => u.xxid == xxid && u.UCType == "Article" && u.IsShow == 1 && u.IsAudit > 1);
            ViewBag.pageCount = query.Count();
            return PartialView("PageArticle", xxid);
        }
        public ActionResult PageClass(Guid xxid)
        {
            var query =
                _uc.View_Simple_SchoolClass.Where(c => c.xxid == xxid);
            ViewBag.pageCount = query.Count();
            return PartialView("PageClass", xxid);
        }
        public ActionResult PageSchool(string key, string xzqhm, string schoolType)
        {
            key = HttpUtility.UrlDecode(key);
            var query =
                _uc.Basic_ZZXX0101.Where(c => c.XZQHM == xzqhm && c.SchoolType == schoolType && c.XXMC.Contains(key));
            ViewBag.pageCount = query.Count();
            return PartialView("PageSchool");
        }
        public ActionResult PageStudent(Guid bjid)
        {
            var query = _uc.UCHome_BaseInfo.Join(
               _uc.View_Simple_StuInfo,
               a => a.UserID,
               b => b.xsid,
               (a, b) => new { a.UserType, a.CreateTime, a.IsStatus, b.username, b.BJID })
               .Where(u => u.BJID == bjid && u.UserType == "s" && u.IsStatus == "1");
            ViewBag.pageCount = query.Count();
            return PartialView("PageStudent", bjid);
        }
        public ActionResult PartStudent(string xxid, string njid)
        {
            Guid schoolId;
            Guid gradeId;
            Guid.TryParse(xxid, out schoolId);
            Guid.TryParse(njid, out gradeId);
            var gradeList = _uc.ExecuteStoreQuery<Data>("select cast(njid as nvarchar(40)) as Id,njmc as Name from (select njid,njmc from  View_Simple_SchoolClass where xxid= '" + schoolId + "' group by njid,njmc ) a left join View_Basic_Grades b on a.njid=b.SerID where njid is not null  order by CodeID").ToList();
            ViewBag.gradeList = gradeList;
            string sql = "select cast(bjid as nvarchar(40)) as Id,xzbmc as Name from View_Simple_SchoolClass where xxid='" + schoolId + "'";
            if (!string.IsNullOrWhiteSpace(njid))
            {
                sql += " and njid='" + gradeId + "'";
            }
            else
            {
                if (gradeList.Count > 0)
                {
                    sql += " and njid='" + gradeList.First().Id + "'";
                }
            }
            sql += " order by xzbmc";
            ViewBag.classList = _uc.ExecuteStoreQuery<Data>(sql).ToList();
            ViewBag.xxid = xxid;
            ViewBag.njid = njid;
            return PartialView("PartStudent");
        }

    }
}
