using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ErrorCollection.Model;
using ErrorCollection.BLL;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.Controllers
{
    public class ErrorCollectionController : Controller
    {
        //
        // GET: /ErrorCollection/
        UCHomeBasePage uc = new UCHomeBasePage();
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
            return View();
        }
        public ActionResult EbookIndex()
        {
            
            return View();
        }
        private string apiconfig
        {
            get
            {
                return Server.MapPath("~\\API.config");
            }
        }
        public PartialViewResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult AddErrorQuestion(Question_ErrorCollection newerror, string[] qtype, string[] errorreason)
        {
            ErrorCollectionbll bll = new ErrorCollectionbll();
            string statuscode = "200";
            string msg = "";
            try
            {
                Guid pkid = Guid.NewGuid();
                newerror.PKID = pkid;
                newerror.UserID = user.userid;
                newerror.EditDate = DateTime.Now;
                newerror.ErrorTimes = 1;
                newerror.status = "0";
                bll.AddErrorQuestion(newerror);
                foreach (string item in qtype)
                {
                    Rel_Question_TypeAndError typerelation = new Rel_Question_TypeAndError();
                    typerelation.PKID = Guid.NewGuid();
                    typerelation.QTID = new Guid(item);
                    typerelation.QECID = pkid;
                    bll.AddRelationType(typerelation);
                }
                foreach (string item in errorreason)
                {
                    Rel_Question_ReasonAndError reasonrelation = new Rel_Question_ReasonAndError();
                    reasonrelation.PKID = Guid.NewGuid();
                    reasonrelation.ERID = new Guid(item);
                    reasonrelation.QECID = pkid;
                    bll.AddRelationReason(reasonrelation);
                }
            }
            catch (Exception)
            {
                statuscode = "500";
                msg = "操作失败，请重试！";
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChangeStatus(Guid pkid, string status)
        {
            status = status == "0" ? "1" : "0";
            ErrorCollectionbll bll = new ErrorCollectionbll();
            string statuscode = "200";
            string msg = "";
            try
            {
                bll.ChangeErrorStatus(pkid, status);
            }
            catch (Exception)
            {
                statuscode = "500";
                msg = "操作失败，请重试！";
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuestionType()
        {
            ErrorCollectionbll bll = new ErrorCollectionbll();
            IEnumerable<Question_Type> typelist = bll.GetQuestionTypes();
            return Json(typelist, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetErrorReason()
        {
            ErrorCollectionbll bll = new ErrorCollectionbll();
            IEnumerable<Question_ErrorReason> reasonlist = bll.GetErrorReasons();
            return Json(reasonlist, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetErrorCollections()
        {
            ErrorCollectionbll bll = new ErrorCollectionbll();
            IEnumerable<Question_ErrorCollection> errorCollections = bll.GetErrorCollections(loginId);
            return Json(errorCollections, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetErrorCollectionByPKID(Guid pkid)
        {
            ErrorCollectionbll bll = new ErrorCollectionbll();
            Question_ErrorCollection errorCollection = bll.GetErrorCollection(pkid);
            var ecext = new ErrorCollectionExt { ErrorCollection = errorCollection };
            return Json(ecext, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult DeleteErrorCollection(Guid pkid)
        {
            ErrorCollectionbll bll = new ErrorCollectionbll();
            string statuscode = "200";
            string msg = "";
            try
            {
                bll.DeleteErrorCollection(pkid);
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
        public JsonResult GetErrorStatus(Guid[] ids)
        {
            ErrorCollectionbll bll = new ErrorCollectionbll();
            var result = bll.GetErrorStatus(ids);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveErrorFromTaixue(Question_ErrorCollection model, Guid[] types, Guid[] reasons)
        {
            try
            {
                model.UserID = loginId;
                ErrorCollectionbll errorCollectionbll = new ErrorCollectionbll();
                errorCollectionbll.AddErrorQuestion(model);
                foreach (var type in types)
                {
                    Rel_Question_TypeAndError typeAndError = new Rel_Question_TypeAndError();
                    typeAndError.PKID = Guid.NewGuid();
                    typeAndError.QECID = model.PKID;
                    typeAndError.QTID = type;
                    errorCollectionbll.AddRelationType(typeAndError);
                }
                foreach (var reason in reasons)
                {
                    Rel_Question_ReasonAndError resAndError = new Rel_Question_ReasonAndError();
                    resAndError.PKID = Guid.NewGuid();
                    resAndError.ERID = reason;
                    resAndError.QECID = model.PKID;
                    errorCollectionbll.AddRelationReason(resAndError);
                }
                var result = new
                {
                    flag = true
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var result = new
                {
                    flag = false,
                    error=e.Message+"\n"+e.InnerException.ToString()+"\n"+e.StackTrace
                };
                return Json(result, JsonRequestBehavior.AllowGet);
                
              
            }
        }

        public JsonResult GetEbookquestiontype()
        {
            string apiUrl = APIHelper.GetApiUrl("errorQuestionType", apiconfig);
            var resultdata = HttpClientHelper.GETMethod(apiUrl);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentErrorSubject()
        {
            string apiUrl = APIHelper.GetApiUrl("errorQuestionSubject", apiconfig) + loginId; ;
            var resultData = HttpClientHelper.GETMethod(apiUrl);
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentErrorVersion(string subject)
        {
            string apiUrl = APIHelper.GetApiUrl("errorQuestionVersion", apiconfig);
            apiUrl += loginId + "/" + subject;
            var resultData = HttpClientHelper.GETMethod(apiUrl);
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStudentErrorGrade(string subject,string version)
        {
            string apiUrl = APIHelper.GetApiUrl("errorQuestionGrade", apiconfig);
            apiUrl += loginId + "/" + subject+"/"+version;
            var resultData = HttpClientHelper.GETMethod(apiUrl);
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentErrorCollect(string ChooseType,string SubjectId,string GradeId,string CatalogId,int PageIndex,int PageSize)
        {
            string apiUrl = APIHelper.GetApiUrl("studentErrorList", apiconfig);
            var postData = new
            {
                ChooseType,
                SubjectId,
                GradeId,
                CatalogId,
                UserId = loginId,
                PageIndex,
                PageSize
                
            };
            var resultData=HttpClientHelper.POSTMethod(apiUrl, postData);
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }
    }
}
