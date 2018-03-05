using System;
using System.Linq;
using System.Web.Mvc;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class ClassZoomController : Controller
    {
        //
        // GET: /ClassZoom/
        public static Guid BJID;
        public UCHomeEntities uc = new UCHomeEntities();
        public ActionResult ClassZoom(string bjid)
        {
            View_Simple_SchoolClass sclass;
            if (!string.IsNullOrEmpty(bjid))
            {
                BJID = new Guid(bjid);
                sclass = uc.View_Simple_SchoolClass.SingleOrDefault(b => b.bjid == BJID);
                return View(sclass);
            }
            return View();
        }

        public ActionResult ClassInfo(string bjid)
        {
            View_Simple_SchoolClass sclass= uc.View_Simple_SchoolClass.SingleOrDefault(b => b.bjid == BJID);
            return PartialView("ClassInfo", sclass);
        }
        public ActionResult ClassNews(string bjid)
        {
            View_Simple_SchoolClass sclass = uc.View_Simple_SchoolClass.SingleOrDefault(b => b.bjid == BJID);
            return PartialView("ClassNews", sclass);
        }
        public ActionResult ClassTeaNews()
        {
            return PartialView("ClassNews");
        }
        public ActionResult ClassStuNews()
        {
            return PartialView("ClassNews");
        }

        public ActionResult ClassKB(string bjid)
        {
            return PartialView("ClassKB", bjid);
        }
    }
}
