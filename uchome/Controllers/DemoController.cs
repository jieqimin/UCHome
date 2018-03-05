using HomeWork.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class DemoController : Controller
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
        public ActionResult Index()
        {
            Homework_HwPublish hw = new Homework_HwPublish();
            hw.HomeworkID = Guid.NewGuid();
            return View(hw);
        }
        public ActionResult UpFileDemo()
        {
            Homework_HwPublish hw = new Homework_HwPublish();
            hw.HomeworkID = Guid.NewGuid();
            return View(hw);
        }
        public ActionResult datetimepickerDemo()
        {
            Homework_HwPublish hw = new Homework_HwPublish();
            hw.HomeworkID = Guid.NewGuid();
            return View(hw);
        }
        public ActionResult uidemo()
        {
            Homework_HwPublish hw = new Homework_HwPublish();
            hw.HomeworkID = Guid.NewGuid();
            return View(hw);
        }
        public ActionResult treeviewDemo()
        {
            return View();
        }
        public ActionResult ValidateDemo()
        {
            Homework_HwPublish hw = new Homework_HwPublish();
            hw.HomeworkID = Guid.NewGuid();
            return View(hw);
        }
        [HttpPost]
        public JsonResult ValidateDemo(Homework_HwPublish hwpublish)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                hwpublish.PublishState = 1;
                hwpublish.PublishDate = DateTime.Now;
                hwpublish.FinishDateStart = DateTime.Now;
                hwpublish.Publisher = user.username;

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
        public ActionResult DatatableDemo()
        {
            List<Homework_HwPublish> hwlist = bll.GetMyHomeWork(user.userid.ToString());
            return View(hwlist);
        }
        public ActionResult SelUserDemo()
        {
            return View();
        }
        public ActionResult UEditorDemo()
        {
            return View();
        }
        public PartialViewResult FormValid()
        {
            return PartialView();
        }
    }
}
