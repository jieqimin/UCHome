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
    public class VenueManageController : Controller
    {
        //
        // GET: /VenueManage/
        private VenueSetup.Bll.VenueSetup bll = new VenueSetup.Bll.VenueSetup();
        private int currentDisplayYear; //=DateTime.Now.Year;
        private int currentDisplayMonth;//= DateTime.Now.Month;
        private int currentDisplayMonday; //=DateTime.Now.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).Day;
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();

        string[] Days = new string[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };

        private DateTime display;

        private string DataToData;

        private string FirstData;

        private List<WeekDay> timeList = new List<WeekDay>();

        public ActionResult VenuePreview(string dateTime = "", string week = "")
        {
            var venueUseList = new VenueUseList();
            if (!string.IsNullOrEmpty(dateTime))
            {
                var thisData = Convert.ToDateTime(dateTime);

                switch (week)
                {
                    case "prevWeek":
                        DateTime newPrevData = thisData.AddDays(-7);
                        currentDisplayMonday = newPrevData.Day;
                        currentDisplayMonth = newPrevData.Month;
                        currentDisplayYear = newPrevData.Year;

                        break;
                    case "nowWeek":
                        initCurrentTime(); //当前年月日
                        break;
                    case "nextWeek":
                        DateTime newNextData = thisData.AddDays(7);
                        currentDisplayMonday = newNextData.Day;
                        currentDisplayMonth = newNextData.Month;
                        currentDisplayYear = newNextData.Year;
                        break;
                }
            }
            else
            {
                initCurrentTime(); //当前年月日
            }

            fillTimeList();

            ViewBag.DataToData = DataToData;//本周开始时间结束时间
            ViewBag.FirstData = FirstData;

            List<MeetingRoom> list = bll.GetVenueListAvailable();//会议室
            List<ApplyMeeting> apply = bll.GetVenueUseListAudited();//会议室占用列表

            venueUseList.MeetingRoomList = list;
            venueUseList.WeekDayList = timeList;
            venueUseList.ApplyMeetingList = apply;

            return View(venueUseList);
        }

        private void fillTimeList()
        {
            DateTime temp, sunday;
            display = new DateTime(currentDisplayYear, currentDisplayMonth, currentDisplayMonday);
            sunday = display;

            var week = Days[Convert.ToInt32(sunday.DayOfWeek.ToString("d"))];

            timeList.Add(new WeekDay { Week = week, Time = sunday.ToString("MM.dd"), YearMd = sunday.ToString("yyyy.MM.dd") });//周日期

            FirstData = sunday.ToString("yyyy.MM.dd");

            DataToData = sunday.ToString("yyyy.MM.dd");

            for (int i = 1; i < 7; i++)
            {
                temp = sunday.AddDays(i);
                timeList.Add(new WeekDay
                {
                    Week = Days[Convert.ToInt32(temp.DayOfWeek.ToString("d"))],
                    Time = temp.ToString("MM.dd"),
                    YearMd = temp.ToString("yyyy.MM.dd")
                });

                if (i == 6)
                {
                    DataToData += "-" + temp.ToString("yyyy.MM.dd");
                }
            }
        }

        private void initCurrentTime()
        {
            DateTime dt = DateTime.Now.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));
            currentDisplayMonday = dt.Day;
            currentDisplayMonth = dt.Month;
            currentDisplayYear = dt.Year;
        }

        //场馆审核
        public ActionResult VenueAudit()
        {
            var result = bll.GetVenueUseList();
            foreach (var item in result)
            {
                item.MeetingRoom = bll.GetRoomNamebyRid(item.MeetingRoomID);
            }
            return View(result);
        }
        //场馆审核
        public ActionResult AduitApply(Guid Id)
        {
            var auditapply = bll.GetAuditApplyInfo(Id);
            auditapply.MeetingRoom = bll.GetRoomNamebyRid(auditapply.MeetingRoomID);
            ViewBag.Id = Id;
            return View(auditapply);
        }

        //场馆审核
        public JsonResult AuditType(Guid id, string auditType, string reason)
        {
            string code = "1";
            string msg = "";
            try
            {
                bll.ChangeApplyStatus(id, auditType, reason);
            }
            catch (Exception)
            {
                code = "0";
            }
            JsonResult rlt = new JsonResult { Data = new { code, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApplyDetail(Guid roomid, string timetype, string nyr)
        {
            var result = bll.GetApplyListbyAudited(roomid, timetype, nyr);
            return View(result);
        }

    }
}
