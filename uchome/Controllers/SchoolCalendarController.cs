using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.Controllers
{
    public class SchoolCalendarController : Controller
    {
        //
        // GET: /SchoolCalendar/
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private string apiconfig
        {
            get
            {
                return Server.MapPath("~\\API.config");
            }
        }
        private UserInfo user
        {
            get
            {
                return UCHomeBasePage.RequestUser;
            }
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
        private string loginname
        {
            get
            {
                return user.username;
            }
        }
        private string userType
        {
            get
            {
                return user.usertype;
            }
        }
        private string xxmc
        {
            get
            {
                return user.xxmc;
            }
        }
        private string areacode
        {
            get
            {
                try
                {
                    return user.areacode;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        private Guid XXID
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

        public string year
        {
            get
            {
                string yearterm = ucbase.CurrentSchTerm;
                if (yearterm != null)
                {
                    return yearterm.Substring(0, 4);
                }
                return DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
            }
        }
        public ActionResult QueryCurrSchoolCalendar()
        {
            var userid = loginId;
            return View();
        }

        public JsonResult GetCurrSchoolCalendar()
        {
            string apiurl = APIHelper.GetApiUrl("schoolcalendar", apiconfig);
            apiurl = string.Format("{2}GetSchoolCalendars?xxid={0}&year={1}", XXID, year,apiurl);
            var resultdata = HttpClientHelper.GETMethod(apiurl);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
    }
}
