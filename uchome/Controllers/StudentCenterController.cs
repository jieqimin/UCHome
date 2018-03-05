using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class StudentCenterController : Controller
    {
        //
        // GET: /TeacherCenter/
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
        public ActionResult Query(Guid id)
        {
            UserInfo u = ucbase.GetUserInfoByRequestId(id);
            return View(u);
        }
        public ActionResult Index()
        {
            return View(loginId);
        }
        public ActionResult Chat(string classlist, string stuname = null)
        {
            return View();
        }

        public ActionResult ClassKB(Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            Guid xsid = id;
            View_Simple_StuInfo stu = uc.View_Simple_StuInfo.SingleOrDefault(u => u.xsid == xsid);
            return PartialView("ClassKB", stu);
        }
        public ActionResult PersonInfo(Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            Guid xsid = id;//LoginSet.Consts.CurrentUser.ID;
            StudentAllInfo sai = new StudentAllInfo();
            View_Simple_StuInfo stu = uc.View_Simple_StuInfo.SingleOrDefault(u => u.xsid == xsid);
            if (stu != null) sai.VSS = stu; else sai.VSS = new View_Simple_StuInfo();
            UCHome_BaseInfo ubi = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == xsid);
            if (ubi != null) sai.UBI = ubi; else sai.UBI = new UCHome_BaseInfo();
            return PartialView("PersonInfo", sai);
        }
        public ActionResult PersonInfo2(Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            Guid xsid = id;//LoginSet.Consts.CurrentUser.ID;
            StudentAllInfo sai = new StudentAllInfo();
            View_Simple_StuInfo stu = uc.View_Simple_StuInfo.SingleOrDefault(u => u.xsid == xsid);
            if (stu != null) sai.VSS = stu; else sai.VSS = new View_Simple_StuInfo();
            UCHome_BaseInfo ubi = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == xsid);
            if (ubi != null) sai.UBI = ubi; else sai.UBI = new UCHome_BaseInfo();
            return PartialView("PersonInfo2", sai);
        }
        public ActionResult Studentlist(string bjid, Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            MyClass myclass = new MyClass();

            if (string.IsNullOrEmpty(bjid))
            {
                Guid jsid = id;
                var techlist = uc.View_Simple_StuInfo.SingleOrDefault(u => u.xsid == jsid);
                if (techlist != null)
                {
                    myclass.xxid = new Guid(techlist.xxid.ToString());
                    myclass.bjid = new Guid(techlist.BJID.ToString());
                    myclass.bjmc = techlist.bjmc;
                }
            }
            else
            {
                View_Simple_SchoolClass schclass = uc.View_Simple_SchoolClass.SingleOrDefault(b => b.bjid == new Guid(bjid));
                myclass.bjid = new Guid(bjid);
                if (schclass != null)
                {
                    if (schclass.xxid != null) myclass.xxid = schclass.xxid.Value;
                    myclass.bjmc = schclass.xzbmc;
                }
            }
            return PartialView("../TeacherCenter/Studentlist", myclass);
        }
        public ActionResult TeacherList(string bjid, Guid id)
        {
            UCHomeEntities uc = new UCHomeEntities();
            MyClass myclass = new MyClass();

            if (string.IsNullOrEmpty(bjid))
            {
                Guid jsid = id;
                var techlist = uc.View_Simple_StuInfo.SingleOrDefault(u => u.xsid == jsid);
                if (techlist != null)
                {
                    myclass.bjid = new Guid(techlist.BJID.ToString());
                    myclass.bjmc = techlist.bjmc;
                    myclass.xxid = new Guid(techlist.xxid.ToString());
                }
            }
            else
            {
                View_Simple_SchoolClass schclass = uc.View_Simple_SchoolClass.SingleOrDefault(b => b.bjid == new Guid(bjid));
                myclass.bjid = new Guid(bjid);
                if (schclass != null)
                {
                    if (schclass.xxid != null) myclass.xxid = schclass.xxid.Value;
                    myclass.bjmc = schclass.xzbmc;
                }
            }
            return PartialView("TeacherList", myclass);
        }

        public ActionResult ShowResource(string learntype)
        {
            string ltype = HttpUtility.UrlDecode(learntype);
            ViewBag.LearnType = ltype;
            return PartialView("ShowResource");
        }
        public ActionResult ResourcesList(string userType, string elementName, string elementValue, string userid, int top)
        {
            ViewBag.userType = userType;
            ViewBag.elementName = elementName;
            ViewBag.elementValue = elementValue;
            ViewBag.userid = userid;
            ViewBag.top = top;
            return PartialView(null);
        }
    }
    public class StudentAllInfo
    {
        public View_Simple_StuInfo VSS { get; set; }
        public UCHome_BaseInfo UBI { get; set; }
    }
}
