using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack.Common.Utils;
using UCHome.Model;
using VenueSetup.Model;
using VenueSetup.Bll;

namespace UCHome.Controllers
{
    public class VenueApplyController : Controller
    {
        //
        // GET: /VenueApply/
        private int currentDisplayYear; //=DateTime.Now.Year;
        private int currentDisplayMonth;//= DateTime.Now.Month;
        private int currentDisplayMonday; //=DateTime.Now.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).Day;
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();

        string[] Days = new string[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };

        private DateTime display;

        private string DataToData;

        private string FirstData;

        private List<WeekDay> timeList = new List<WeekDay>();


        private VenueSetup.Bll.VenueSetup bll = new VenueSetup.Bll.VenueSetup();

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

        private string xxmc
        {
            get
            {
                try
                {
                    return user.xxmc;
                }
                catch (Exception)
                {
                    return String.Empty;
                }
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


        public ActionResult VenueApply(string dateTime = "", string week = "")
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
            List<ApplyMeeting> apply = bll.GetVenueAppUseList();//会议室占用列表

            venueUseList.MeetingRoomList = list;
            venueUseList.WeekDayList = timeList;
            venueUseList.ApplyMeetingList = apply;

            return PartialView(venueUseList);
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

        public ActionResult ApplyDetail(Guid roomid, string timetype, string nyr)
        {
            var result = bll.GetApplyList(roomid, timetype, nyr);
            return PartialView(result);
        }

        public ActionResult ApplyUseVenue(Guid roomId, string type, string yearmd)
        {
            ApplyMeeting applymeet = new ApplyMeeting();
            applymeet.ID = Guid.NewGuid();
            ViewBag.Time = yearmd;
            ViewBag.type = type;
            ViewBag.RoomId = roomId;
            return PartialView(applymeet);
        }

        [HttpPost]
        public ActionResult ApplyUseVenue(ApplyMeeting applymeet)
        {
            string statuscode = "200";
            string msg = "";
            applymeet.MeetingRoomID = applymeet.MeetingRoomID;
            applymeet.ApplierName = loginId.ToString();//申请人id
            applymeet.Department = xxmc;
            applymeet.HosterID = xxid.ToString();
            applymeet.ApplyDate=DateTime.Now;
            applymeet.CreatorDisplayName = user.username;
            applymeet.CreateTime=DateTime.Now;
            applymeet.Status = "未审核";
            try
            {
                bll.SaveApplyMeeting(applymeet);
            }
            catch (Exception ex)
            {
                statuscode = "500";
                msg = ex.ToString();
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, msg } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }

        private void initCurrentTime()
        {
            DateTime dt = DateTime.Now.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));
            currentDisplayMonday = dt.Day;
            currentDisplayMonth = dt.Month;
            currentDisplayYear = dt.Year;
        }
        /// <summary>
        /// 验证时间冲突
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public JsonResult CheckTime(string startTime, string endTime, Guid roomId)
        {
            var satart = DateTime.Parse(startTime);
            var ent = DateTime.Parse(endTime);
            var result = bll.CheckTimes(satart, ent, roomId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VenueApplyRecord()
        {
            var result = bll.GetVenuePersonRec(loginId.ToString());
            foreach (var item in result)
            {
                item.MeetingRoom = bll.GetRoomNamebyRid(item.MeetingRoomID);
            }
            return PartialView(result);
        }

        public ActionResult DetailApply(Guid id)
        {
            var applydetail = bll.GetAuditApplyInfo(id);
            applydetail.MeetingRoom = bll.GetRoomNamebyRid(applydetail.MeetingRoomID);
            return PartialView(applydetail);
        }

        public JsonResult DeliteApply(Guid id)
        {
            var code = "1";
            try
            {
                bll.deteteApply(id);
            }
            catch (Exception)
            {
                code = "0";
            }

            return Json(code, JsonRequestBehavior.AllowGet);
        }

    }
}
