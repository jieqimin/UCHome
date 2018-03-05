using System.Data.SqlClient;
using MessageCenter.Model;
using OA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCHome.BLL;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.Controllers
{
    public class SchoolCenterController : Controller
    {
        //
        // GET: /TeacherCenter/
        readonly UCHomeEntities _uc;
        private readonly UCResourceEntities _rec;


        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        ZZ_MessageCenterEntities db = new ZZ_MessageCenterEntities();  //zy
        ZZ_MIFVSEntities zz = new ZZ_MIFVSEntities();
        public SchoolCenterController()
        {
            _uc = new UCHomeEntities();
            _rec = new UCResourceEntities();
        }

        private void Init()
        {
            Guid xxid = new Guid();
            if (Request.RequestContext.RouteData.Values["id"] != null)
            {
                Guid.TryParse(Request.RequestContext.RouteData.Values["id"].ToString(), out xxid);
            }
            var result = _uc.Basic_ZZXX0101.SingleOrDefault(x => x.XXID == xxid);
            if (result != null)
            {
                ViewBag.xzqhm = result.XZQHM;
                ViewBag.xxmc = result.XXMC;
            }
        }
        public ActionResult Index()
        {
            // Guid xxid = xxid;
            Guid xxid = new Guid();
            if (Request.RequestContext.RouteData.Values["id"] != null)
            {
                Guid.TryParse(Request.RequestContext.RouteData.Values["id"].ToString(), out xxid);
            }
            ViewBag.xxid = xxid;

            var school = _uc.Basic_ZZXX0101.FirstOrDefault(m => m.XXID == xxid);
            if (school != null)
            {
                ViewBag.xzqhm = school.XZQHM;
                ViewBag.schoolType = school.SchoolType;
            }
            Init();
            var schoolarea = _uc.GBT2260.FirstOrDefault(x => x.CODE_ID == school.XZQHM).CODE_NAME;
            ViewBag.schoolarea = schoolarea;

            //访问量
            var list = _uc.ExecuteStoreQuery<Count>("select '1' as [Key], COUNT(*) Value from UCHome_BaseInfo a left join View_Simple_User b on a.UserID=b.userid where b.xxid='" + xxid + "' union all select '2' as [Key],COUNT(*) from UCHome_PersonNew a left join View_Simple_User b on a.AddUser=b.userid where b.xxid='" + xxid + "' and UCType='Article' union all select '3' as [Key],isnull(sum(Hits),0) from UCHome_PersonNew a left join View_Simple_User b on a.AddUser=b.userid where b.xxid='" + xxid + "' and UCType='Article'").ToList();
            ViewBag.zoom = list.FirstOrDefault(m => m.Key == "1").Value;
            ViewBag.article = list.FirstOrDefault(m => m.Key == "2").Value;
            ViewBag.total = list.FirstOrDefault(m => m.Key == "3").Value;
            //ViewBag.schoolResources = zz.TalbeFile.Where(x => x.Xxid == xxid).Count();

            return View("Index", xxid);
        }

        public ActionResult NewsList()
        {
            return PartialView("NewsList");
        }

        //最新文章
        public ActionResult NewArticle(Guid xxid, int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            var list = topbll.GetArticleNewTop("", xxid.ToString(), 5);
            return PartialView("NewArticle", list);
        }

        //优秀教师空间
        public ActionResult RoomsList(string xxid, string userType, int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            List<UCHome_Top_Data> list;
            if (userType.ToLower() == "t")
            {
                list = topbll.GetTeaArticlesTop("", xxid, pageSize);
                ViewBag.UserType = "t";
            }
            else
            {
                list = topbll.GetStuArticlesTop("", xxid, pageSize);
                ViewBag.UserType = "s";
            }
            return PartialView("RoomsList", list);
        }


        //public ActionResult ResourceList(Guid xxid, string learnType, int pageSize)
        //{
        //    var list =
        //        _rec.V_ResourceByLearn.Where(c => c.learntype == learnType && c.XXID == xxid)
        //            .OrderByDescending(c => c.CreateTime)
        //            .Take(pageSize).AsEnumerable()
        //            .Select(c => new MyResource(c.id, c.Name.Trim(), c.CreateTime, c.Hits, c.Grade, c.Subject, c.XXMC, c.CreatorName,c.LinkUrl))
        //            .ToList();
        //    return PartialView("ResourceList", list);
        //}

        //public ActionResult CourseList(Guid xxid, string learnType, int pageSize)
        //{
        //    var list =
        //        _rec.V_ResourceByLearn.Where(c => c.learntype == learnType && c.XXID == xxid)
        //            .OrderByDescending(c => c.CreateTime)
        //            .Take(pageSize).AsEnumerable()
        //            .Select(c => new MyResource(c.id, c.Name.Trim(), c.CreateTime, c.Hits, c.Grade, c.Subject, c.XXMC, c.CreatorName,c.LinkUrl))
        //            .ToList();
        //    return PartialView("CourseList", list);
        //}


        //文章点击数排行
        public ActionResult ArticleHitsRank(string xxid, int pageSize)
        {
            UCHome_TopBLL topbll = new UCHome_TopBLL();
            var list = topbll.GetArticleHitsTop("", xxid, 5);
            return PartialView("HotArticle", list);
        }

        //班级列表
        public ActionResult ClassList(string xxid, int pageSize)
        {
            Guid schoolId;
            Guid.TryParse(xxid, out schoolId);
            string sql =
                "select bjid, xzbmc,year,njdm,xxid from view_school_classes where JGH='1' and xxid=@xxid order by njdm,xzbmc";
            object[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@xxid", xxid);
            var list =
                _uc.ExecuteStoreQuery<TempClass>(sql, paras)
                    .OrderBy(c => c.njdm)
                    .ThenBy(c => c.xzbmc)
                    .Take(pageSize)
                    .Select(c => new SClass(c.bjid, c.xzbmc, c.year))
                    .ToList();
            return PartialView("ClassList", list);
        }

        public JsonResult GetArticles(Guid xxid, int pageIndex, int pageSize)
        {
            var query = _uc.View_PersonArticle
                .Where(c => c.xxid == xxid);
            var list = query.OrderByDescending(c => c.DeployTime).Skip(pageIndex * pageSize).Take(pageSize).AsEnumerable()
                .Select(c => new Article(c.PKID, c.Title, c.DeployTime)).ToList();
            return Json(new { data = list, total = query.Count() }, JsonRequestBehavior.AllowGet);
        }

        #region  学校资源  学校通知

        // 学校通知_list zy
        public ActionResult SchoolMessage(string xxid, int pageSize)
        {
            Guid schoolId;
            Guid.TryParse(xxid, out schoolId);
            string sql =
               "select  * from [ZZ_MessageCenter].dbo.Msg_Notice where  XXID= '" + xxid + "' and  MsgTypeName='学校通知' and Status !='已删除' order by MsgDate desc ";
            var list = db.ExecuteStoreQuery<NewSchoolMessage>(sql).Take(pageSize).ToList();

            return PartialView("SchoolMessage", list);

        }
        //学校通知_具体查看
        public ActionResult SchoolMessageEdit(Guid id)
        {

            var result = db.Msg_Notice.Where(x => x.ID == id).AsEnumerable()
                .Select(x => new NewSchoolMessage(x.ID, x.MsgTitle, x.MsgSendUserName, DateTime.Parse(x.MsgDate.ToString()), x.MsgContent, x.FileID, x.FileUrl))
                .First();
            return PartialView("SchoolMessageEdit", result);

        }

        //所有的学校消息
        public ActionResult MessageMores(Guid xxid)
        {
            string sql =
                   "select  * from [ZZ_MessageCenter].dbo.Msg_Notice where  XXID= '" + xxid + "' and  MsgTypeName='学校通知' and Status !='已删除' order by MsgDate desc ";
            var list = db.ExecuteStoreQuery<NewSchoolMessage>(sql).ToList();

            var result = _uc.Basic_ZZXX0101.SingleOrDefault(x => x.XXID == xxid);
            ViewBag.xxmc = result.XXMC;

            return PartialView("MessageMores", list);


        }

        public JsonResult schoolResources(Guid type, Guid xxid)
        {
            string apiurl = APIHelper.GetApiUrl("schoolResources");
            var url = apiurl + xxid + "/xxid/" + type + "/type";

            var resultdata = HttpClientHelper.GETMethod(url);
            return Json(resultdata, JsonRequestBehavior.AllowGet);

        }

        //统计学校资源总数
        public JsonResult SumschoolResources(Guid xxid)
        {
            string apiurl = APIHelper.GetApiUrl("SumschoolResources");
            // http://192.168.99.137/RedisCache.Api/api/resource/statistic/schoolResCount/6B915392-B695-4026-9526-2F6DBCD7D7DE
            var sum = apiurl + xxid;
            var resultdata = HttpClientHelper.GETMethod(sum);
            return Json(resultdata, JsonRequestBehavior.AllowGet);

        }


        #endregion

        #region 学校资源  资源



        #endregion

        #region  学校封面
        public JsonResult GetDiviamges(Guid xxid)
        {
            //Guid xxid = new Guid();
            //if (Request.RequestContext.RouteData.Values["id"] != null)
            //{
            //    Guid.TryParse(Request.RequestContext.RouteData.Values["id"].ToString(), out xxid);
            //}
            //ViewBag.xxid = xxid;
            //   string filePath = context.Server.MapPath(model.FilePath);


            var hostname = System.Configuration.ConfigurationManager.AppSettings["Storages"];
            var url = zz.TalbeFile.Where(x => x.Xxid == xxid && x.banner == "1").ToList();

            var urlfile = "";
            List<string> list = new List<string>();
            foreach (var item in url)
            {
                urlfile = item.Path + hostname + item.Url;
                list.Add(urlfile);
            }
            // var host = urlfile.Substring(1, urlfile.Length - 1);


            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetMarquee_x(Guid xxid)
        {
            var hostname = System.Configuration.ConfigurationManager.AppSettings["Storages"];
            var model = zz.TalbeFile.Where(x => x.Xxid == xxid && x.banner == "0").ToList();

            List<string> list = new List<string>();
            List<string> tempname = new List<string>();
            var urlfile = "";
            var name = "";
            foreach (var item in model)
            {
                urlfile = item.Path + hostname + item.Url;
                name = item.UrlName;

                list.Add(urlfile);
                tempname.Add(name);
            }


            return Json(new { data = list, nameurl = tempname }, JsonRequestBehavior.AllowGet);

        }


        #endregion


    }
}
