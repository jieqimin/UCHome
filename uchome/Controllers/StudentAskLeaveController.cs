using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentAskLeave.Bll;
using StudentAskLeave.Model;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class StudentAskLeaveController : Controller
    {
        UCHomeBasePage ucbase = new UCHomeBasePage();
        private static UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        private Guid userid
        {
            get
            {
                return user.userid;
            }
        }
        private Guid xxid
        {
            get
            {
                return user.xxid;
            }
        }
        private string userName
        {
            get
            {
                return user.username;
            }
        }

        public ActionResult StudentAsk()
        {

            var teacher = StudentAskLeaveHelper.GetStudentClassInfo(userid);
            if (teacher.JSID != null)
            {
                ViewBag.teacherId = teacher.JSID;
                ViewBag.teacherName = teacher.JSMC;
            }
            return View();
        }

        [HttpPost]
        public JsonResult StudentApply(Uchome_StudentAskLeaveApply studentAskLeaveApply, Guid teacherId, string teacherName)
        {
            studentAskLeaveApply.Id = Guid.NewGuid();
            studentAskLeaveApply.CreateTime = DateTime.Now;
            studentAskLeaveApply.Status = (int)StudentAskLeaveStatus.Applyed;
            studentAskLeaveApply.ApplyerId = userid;
            studentAskLeaveApply.ApplyerName = userName;
            studentAskLeaveApply.SchoolId = xxid;
            try
            {
                var flag = StudentAskLeaveHelper.AddStudentLeaveApply(studentAskLeaveApply);
                Uchome_StudentAskLeaveAudit adAudit = new Uchome_StudentAskLeaveAudit()
                {
                    ApplyId = studentAskLeaveApply.Id,
                    ApplyerId = studentAskLeaveApply.ApplyerId,
                    ApplyerName = studentAskLeaveApply.ApplyerName,
                    AuditorId = teacherId,
                    AuditorName = teacherName,
                    CreateTime = DateTime.Now,
                    Status = (int)StudentAskLeaveStatus.Applyed,
                    Id = Guid.NewGuid()

                };
                flag = StudentAskLeaveHelper.AddStudentLeaveAudit(adAudit);
                return Json(new
                {
                    flag
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult StudentApplyList()
        {
            var list = StudentAskLeaveHelper.GetStudentApplyList(userid);
            return View(list);
        }

        public JsonResult DeleteApply(Guid applyId)
        {
            try
            {
                var flag = StudentAskLeaveHelper.DeleteStudentApply(applyId);
                return Json(new
                {
                    flag,
                    message = "null"
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new
                {
                    flag = false,
                    message = e.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DetailApply(Guid applyid)
        {
            try
            {
                var detail = StudentAskLeaveHelper.GetApplyDetail(applyid);
                return Json(new
                {
                    flag = true,
                    Detail = detail
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new
                {
                    flag = false,
                    message = e.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Teacherindex()
        {
            var list = StudentAskLeaveHelper.GetTeacherAuditApplyList(userid);
            return View(list);
        }

        public JsonResult TeacherAudit(Guid applyId, string auditResult, string remark)
        {
            try
            {

                if (auditResult == "transe")
                {
                    StudentAskLeaveHelper.AuditApplyTranse(userid, userName, applyId, remark);
                }
                else if (auditResult == "pass")
                {
                    StudentAskLeaveHelper.AuditApply(userid, applyId, true);
                }
                else if (auditResult == "deny")
                {
                    StudentAskLeaveHelper.AuditApply(userid, applyId, false);
                }
                return Json(new { flag = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SchoolIndex()
        {
            var list = StudentAskLeaveHelper.GetSchoolApplyList(xxid);
            return View(list);
        }

        public JsonResult StudentLeaveSchool(Guid applyId)
        {
            try
            {
                var flag = StudentAskLeaveHelper.StudentLeaveSchool(applyId);
                return Json(new
                {
                    flag
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    flag = false,
                    message = e.Message
                });
            }

        }
    }
}