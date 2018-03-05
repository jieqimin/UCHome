using System.Web.Mvc;

namespace UCHome.Controllers
{
    public class SchoolZoomController : Controller
    {
        //
        // GET: /ClassZoom/

        public ActionResult SchoolZoom()
        {
            return View();
        }

        public ActionResult SchoolNews()
        {
            return PartialView("SchoolNews");
        }
        public ActionResult SchoolClass()
        {
            return PartialView("SchoolClass");
        }
    }
}
