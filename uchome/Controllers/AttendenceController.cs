using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using ServiceStack.ServiceClient.Web;
using UCHome.Model;
using ServiceModel;
using ServiceStack.Common.Utils;
using Student.Model;

namespace UCHome.Controllers
{
    public class AttendenceController : Controller
    {
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private UserInfo user
        {
            get
            {
                return UCHomeBasePage.RequestUser;
            }
        }
        private Guid userId
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

        public ActionResult StuRecordIndex()
        {
            return View();
        }

        public JsonResult GetTeaChargeClasses()
        {
            using (StudentEntities studentEntities = new StudentEntities())
            {
                var UserId = userId.ToString();
                var classList = studentEntities.Basic_ZZJX0202.Where(p => p.BZRGH == UserId).Select(p => new
                {
                    className = p.XZBMC,
                    classId = p.BJID
                }).ToList();
                return Json(classList, JsonRequestBehavior.AllowGet);
            }
            
        }

        public JsonResult GetSchoolGrades()
        {
            using (StudentEntities studentEntities = new StudentEntities())
            {
                var grades=studentEntities.View_Basic_SchClass.Where(p => p.XXID == user.xxid)
                    .Select(p => new {p.NJDM, p.NJMC}).Distinct()
                    .ToList();
                return Json(grades, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSchoolClasses(string NJDM)
        {
            using (StudentEntities studentEntities = new StudentEntities())
            {
                var classes =
                    studentEntities.Basic_ZZJX0202.Where(p => p.XXID == user.xxid && p.NJDM == NJDM)
                        .Select(p => new {p.BJID, p.XZBMC}).OrderBy(p=>p.XZBMC)
                        .ToList();
                return Json(classes, JsonRequestBehavior.AllowGet);
            }

            
        }

        [HttpPost]
        public JsonResult GetStudentRecordsByClass(string classId, DateTime startTime, DateTime? endTime)
        {
            var url = System.Configuration.ConfigurationManager.AppSettings["APIHttp"];
            JsonServiceClient client = new JsonServiceClient(url+"/SNSAPI/");
            List<UserActiveEntryDto> result= client.Get(new GetUserAction()
            {
                ObjectType = "考勤",
                ClassID = classId,//classid
                Date = startTime,
                DateBak = endTime,
                PageSize = Int32.MaxValue,
                CurPage = 0
            });
            return Json(result.Select(p => new { p.UName, p.Comment, p.Date, p.DateBak,p.UID }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStudentRecordsByParent(DateTime startTime, DateTime endTime)
        {
            using (StudentEntities studentEntities = new StudentEntities())
            {
                
                var childs = studentEntities.Stu_FamilyStuRel.Where(p => p.JZID == userId).Select(p => p.XSID).ToList();
                if (childs.Count == 0) return Json("",JsonRequestBehavior.AllowGet);
                var url = System.Configuration.ConfigurationManager.AppSettings["APIHttp"];
                JsonServiceClient client = new JsonServiceClient(url + "/SNSAPI/");
                List<UserActiveEntryDto> result = new List<UserActiveEntryDto>();
                foreach (var child in childs)
                {
                    result.AddRange(client.Get(new GetUserAction()
                    {
                        ObjectType = "考勤",
                        UID = child.ToString(),
                        Date = startTime,
                        DateBak = endTime,
                        PageSize = Int32.MaxValue,
                        CurPage = 0
                    }));
                }
                
                return Json(result.Select(p => new { p.UName, p.Comment, p.Date }).OrderBy(p=>p.Date).ToList(), JsonRequestBehavior.AllowGet);
            }
        }
            

        public JsonResult GetStudentRecordsByParentId(DateTime startTime, DateTime endTime,Guid jzid)
        {
            using (StudentEntities studentEntities = new StudentEntities())
            {
                var childs = studentEntities.Stu_FamilyStuRel.Where(p => p.JZID == jzid).Select(p => p.XSID).ToList();
                if (childs.Count == 0) return Json("", JsonRequestBehavior.AllowGet);
                var url = System.Configuration.ConfigurationManager.AppSettings["APIHttp"];
                JsonServiceClient client = new JsonServiceClient(url + "/SNSAPI/");
                List<UserActiveEntryDto> result = new List<UserActiveEntryDto>();
                foreach (var child in childs)
                {
                    result.AddRange(client.Get(new GetUserAction()
                    {
                        ObjectType = "考勤",
                        UID = child.ToString(),
                        Date = startTime,
                        DateBak = endTime,
                        PageSize = Int32.MaxValue,
                        CurPage = 0
                    }));
                }
                return Json(result.Select(p => new { p.UName, p.Comment, p.Date }).OrderBy(p => p.Date).ToList(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SchoolStudentRecord()
        {
            return View();
        }
        public ActionResult ParentChildRecord()
        {
            return View();
        }

        //2017-3-27

        public ActionResult SchoolStudentRecordInSingleDay()
        {
            return View();
        }

    }
    
}
