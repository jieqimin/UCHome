using HomeWork.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OA.Model;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class StuHomeworkController : Controller
    {
        //
        // GET: /AppCenter/
        HomeWork.BLL.HwPublishBll bll = new HomeWork.BLL.HwPublishBll();
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
        public ActionResult HW_Deploy()
        {
            Homework_HwPublish hw = new Homework_HwPublish();
            hw.HomeworkID = Guid.NewGuid();
            ViewBag.BJMC = bll.getBjmcByBZr(loginId.ToString());//获取班主任所带的班级
            return View(hw);
        }
        [HttpPost]
        public JsonResult Hw_Deploy(Homework_HwPublish hwpublish)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                hwpublish.PublishState = 1;
                hwpublish.PublishDate = DateTime.Now;
                hwpublish.FinishDateStart = DateTime.Now;
                hwpublish.Publisher = user.userid.ToString();

                try
                {
                    bll.AddHw(hwpublish);
                }
                catch (Exception ex)
                {
                    statuscode = "500";
                    msg = ex.ToString();
                }
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index(string id)
        {
            List<Vw_Homework_HwPublish> hwlist = bll.GetMyHomeWorkVw(user.userid.ToString());
            return View(hwlist);
        }
        //我的作业
        public ActionResult MyHomework()
        {
            List<Vw_Homework_HwPublish> stuHlist = bll.GetStuHomeWork(user.orgid.ToString());
            return View(stuHlist);
        }
        //提交作业
        public ActionResult SubHw(string homeworkid)
        {

            Homework_StuHomeworks stuHw = new Homework_StuHomeworks();
            stuHw.HwID=Guid.NewGuid();
            stuHw.HomeworkID = Guid.Parse(homeworkid);
            return View(stuHw);
        }

        //提交作业
        [HttpPost]
        public ActionResult SubHw(Homework_StuHomeworks stuhomework)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                stuhomework.SubmitDate = DateTime.Now;
                stuhomework.XSID = loginId;
                try
                {
                    bll.AddStuHw(stuhomework);
                }
                catch (Exception ex)
                {
                    statuscode = "500";
                    msg = ex.ToString();
                }
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }
        //学生查看作业详情
        public ActionResult StuHwDetials()
        {
            var homworkid = Request["ID"];
            var stuhwDlist = bll.GetStuHwDeatil(new Guid(homworkid));
            var attachment = stuhwDlist.Attachment.Split('/');
            ViewBag.Attachment = attachment.Last();
            return View(stuhwDlist);
        }

        //教师查看学生提交作业详情
        public ActionResult TeaSeeStuDetail()
        {
            Guid hwid = Guid.Parse(Request["ID"]);
            var stuhwDlist = bll.GetStuDetail(hwid);
            var attachment = stuhwDlist.HwAttachment.Split('/');
            ViewBag.Attachment = attachment.Last();
            return View(stuhwDlist);
        }

        public ActionResult Hw_Stu(Guid id)
        {
            List<Vw_Homework_StuHomeworks_Search> stuhw = bll.GetStuHomeWorkById(id);
            return PartialView(stuhw);
        }
        protected override void Dispose(bool disposing)
        {
            bll.Dispose();
            base.Dispose(disposing);
        }
    }
}
