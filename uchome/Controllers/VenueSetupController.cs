using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCHome.Model;
using VenueSetup.Model;
using VenueSetup.Bll;

namespace UCHome.Controllers
{
    public class VenueSetupController : Controller
    {
        //
        // GET: /VenueSetup/
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private VenueSetup.Bll.VenueSetup bll = new VenueSetup.Bll.VenueSetup();
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

        private UserInfo user
        {
            get
            {
                return UCHomeBasePage.RequestUser;
            }
        }

        private Guid xxid
        {
            get
            {
                try
                {
                    return user.xxid;
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }
        }

        public ActionResult VenueSetup()
        {
            MeetingRoom app = new MeetingRoom();
            app.ID = Guid.NewGuid();
            return View(app);
        }

        [HttpPost]
        public JsonResult VenueSetup(MeetingRoom meetRoom)
        {
            string statuscode = "200";
            string msg = "";
            meetRoom.ID=Guid.NewGuid();
            meetRoom.XXID = xxid;
            if (ModelState.IsValid)
            {
                meetRoom.CreatorName = user.loginname;
                meetRoom.CreatorDisplayName = user.username;
                meetRoom.CreateTime = DateTime.Now;
                meetRoom.Memo = "启用";
                try
                {
                    bll.addVenueSet(meetRoom);
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

        public ActionResult VenueList()
        {
            List<MeetingRoom> list = bll.GetVenueList();
            return View(list);
        }

        public JsonResult ChangeMemoById(string status, Guid id)
        {
            bool code;
            try
            {
                code=bll.ChangeMemobyId(status, id);
            }
            catch (Exception)
            {
                code =false;
            }
            return Json(code, JsonRequestBehavior.AllowGet);
        }

       

    }
}
