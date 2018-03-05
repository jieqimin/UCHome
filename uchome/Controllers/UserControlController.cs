using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCHome.BLL;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class UserControlController : Controller
    {
        private readonly SelectUserServices services = new SelectUserServices();
        private readonly UCHomeBasePage page = new UCHomeBasePage();
        private static UserInfo u
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        public ActionResult SelectUser()
        {
            var selRole = Request["selRole"];
            var selType = Request["selType"];
            var selArea = Request["selArea"];
            ViewBag.selRole = selRole;
            ViewBag.selType = selType;
            ViewBag.selArea = selArea;
            SelectUserInfo model = services.GetSelectUserInfo(u.userid, u.orgid.Value, page.CurrentSchTerm, selRole, selArea, u.xxid.ToString());
            return View(model);
        }

        public JsonResult QueryUser(string selRole, string selArea, string query)
        {
            var list = services.GetQueryUserList(u.userid, u.orgid.Value, page.CurrentSchTerm, selRole, selArea, query, u.xxid.ToString());

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAttentionUserList()
        {
            var attentionUserList = services.GetAttentionUserList(u.userid);

            return Json(attentionUserList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentList(string bjid, string selRole)
        {
            Guid _bjid = Guid.NewGuid();
            if (selRole == "tea")
                _bjid = Guid.Parse(bjid);
            else if (selRole == "stu")
            {
                _bjid = u.orgid.Value;
            }
            var studentList = services.GetStudentList(_bjid);

            return Json(studentList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStuParentList(string bjid, string selRole)
        {
            Guid _bjid = Guid.NewGuid();
            if (selRole == "tea")
                _bjid = Guid.Parse(bjid);
            else
            {
                _bjid = u.orgid.Value;
            }
            var stuParentList = services.GetStuParentList(_bjid);

            return Json(stuParentList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeptUserList(Guid deptid)
        {
            var stuParentList = services.GetTeaDeptByidList(deptid);

            return Json(stuParentList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMyTeacherList()
        {
            var teaList = services.GetMyTeacherList(u.orgid.Value, page.CurrentSchTerm);

            return Json(teaList, JsonRequestBehavior.AllowGet);
        }
    }
}
