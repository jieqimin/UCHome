using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.Controllers
{
    public class MyResourceController : Controller
    {
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
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
        public ActionResult Index()
        {
            UCHomeEntities ue = new UCHomeEntities();
            UCHome_BaseInfo ubi = ue.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == loginId);
            return View(ubi);
        }
        public ActionResult SendRes(string id)
        {
            UCHomeEntities ue = new UCHomeEntities();
            UCHome_BaseInfo ubi = ue.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == loginId);
            return View(ubi);
        }
        public ActionResult MyRes(string id)
        {
            UCHomeEntities ue = new UCHomeEntities();
            UCHome_BaseInfo ubi = ue.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == loginId);
            return View(ubi);
        }
        public ActionResult QueryResById(Guid id)
        {
            UCHomeEntities ue = new UCHomeEntities();
            UCHome_BaseInfo ubi = ue.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == id);
            return PartialView(ubi);
        }
        public ActionResult QueryResById2(Guid id)
        {
            UCHomeEntities ue = new UCHomeEntities();
            UCHome_BaseInfo ubi = ue.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == id);
            return View(ubi);
        }
        public PartialViewResult GetResourceList(string schoolstages, string subjectss)
        {
            schoolstages = HttpUtility.UrlDecode(schoolstages);
            subjectss = HttpUtility.UrlDecode(subjectss);
            if (schoolstages != null)
            {
                string[] schoolstagearray = schoolstages.Split(',');
                if (subjectss != null)
                {
                    string[] subjectarray = subjectss.Split(',');
                    UCResourceEntities uc = new UCResourceEntities();
                    List<V_ResourceByLearn> resourceByLearns = uc.V_ResourceByLearn.Where(r => schoolstagearray.Contains(r.schoolstage) && subjectarray.Contains(r.Subject)).ToList();
                    return PartialView("ResourceListByLearn", resourceByLearns);
                }
            }
            return PartialView("ResourceListByLearn", null);
        }
        private string apiconfig
        {
            get
            {
                return Server.MapPath("~\\API.config");
            }
        }
        public JsonResult GetResourceByWhere(string conditionType,string condition,string format,string type,int page)
        {
            string apiurl = APIHelper.GetApiUrl("getresourcebywhere", apiconfig);
            apiurl = string.Format("{0}?conditionType={1}&condition={2}&format={3}&type={4}&page={5}", apiurl, conditionType,condition,format,type,page);
            var resultdata = HttpClientHelper.GETMethod(apiurl);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMyResourceByWhere(string conditionType, string condition, string format, string type, int page,Guid userid)
        {
            string apiurl = APIHelper.GetApiUrl("getpersonresbywhere", apiconfig);
            apiurl = string.Format("{0}?conditionType={1}&condition={2}&format={3}&type={4}&page={5}&userID={6}", apiurl, conditionType, condition, format, type, page, userid);
            var resultdata = HttpClientHelper.GETMethod(apiurl);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
    }
}
