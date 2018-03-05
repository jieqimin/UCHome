using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using OA.Model;
using UCHome.Model;
using VenueSetup.Model;

namespace UCHome.Controllers
{
    public class DataStatisticsController : Controller
    {
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        ZZ_MIFVSEntities entity = new ZZ_MIFVSEntities();
        private VenueEntities venue = new VenueEntities();
        private UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        public ActionResult LeaveStatistics()
        {
            return View();
        }
        private Guid xxid
        {
            get { return user.xxid; }
        }
        /// <summary>
        /// 维修统计
        /// </summary>
        /// <returns></returns>
        public ActionResult MaintenanceStatistics()
        {
            var result =
                entity.View_Repair.Where(x => x.wxxm != null && x.XXID == xxid && (x.RepairStatus=="已分派"|| x.RepairStatus=="已处理"))
                    .GroupBy(y => y.wxxm)
                    .Select(z => new RepairInfo
                    {
                        repairPerson = z.Key,
                        jstyCount = z.Sum(a => a.BaoXiuProject.Contains("投影") ? 1 : 0),
                        jkqCount = z.Sum(a => a.BaoXiuProject.Contains("监控") ? 1 : 0),
                        jsrjCount = z.Sum(a => a.BaoXiuProject.Contains("软件") ? 1 : 0),
                        jsjsjCount = z.Sum(a => a.BaoXiuProject.Contains("计算机") ? 1 : 0),
                        mcssCount = z.Sum(a => a.BaoXiuProject.Contains("门窗设施") ? 1 : 0),
                        mcCount = z.Sum(a => a.BaoXiuProject.Equals("门窗") ? 1 : 0),
                        gssbCount = z.Sum(a => a.BaoXiuProject.Contains("供水") ? 1 : 0),
                        gnsbCount = z.Sum(a => a.BaoXiuProject.Contains("供暖") ? 1 : 0),
                    }).ToList();
            return View(result);
        }

        public ActionResult MainPerStatistics()
        {
            var result =
                entity.View_Repair.Where(p => p.wxxm != null && p.XXID == xxid).GroupBy(p => p.wxxm).Select(q => new PersonInfo
                {
                    repairPerson = q.Key,
                    yfpCount = q.Sum(x => x.RepairStatus == "已分派" ? 1 : 0),
                    ywcCount = q.Sum(x => x.RepairStatus == "已处理" ? 1 : 0)
                }).ToList();
            ViewBag.ywxTotal = result.Sum(x => x.ywcCount);
            ViewBag.yfpTotal = result.Sum(x => x.yfpCount);
            return View(result);
        }

        public ActionResult VenueApplyStatistics()
        {
            var _xxid = xxid.ToString();
            var result =
                venue.ApplyMeeting.Where(x => x.CreatorDisplayName != null && x.HosterID == _xxid).GroupBy(x => x.CreatorDisplayName).Select(x => new VenueApply()
                    {
                        applyer = x.Key,
                        applyCount = x.Count(),
                        auditCount = x.Sum(a=>a.Status=="审核通过"?1:0)
                    }).ToList();
            ViewBag.allApplyCount = result.Sum(x => x.applyCount);
            ViewBag.allAuditConunt = result.Sum(x => x.auditCount);
            return View(result);
        }


        /// <summary>
        /// 下拉框绑定年份（当前年份及前五年）
        /// </summary>
        /// <returns></returns>
        public JsonResult Getyears()
        {
            int currentYear = DateTime.Now.Year;//获取当前年限
            var yearList = new List<SelectListItem>();
            for (int i = currentYear - 0; i >= currentYear - 5; i--)
            {
                if (i == currentYear)
                {
                    yearList.Add(new SelectListItem
                    {
                        Text = Convert.ToString(i+"年"),
                        Value = Convert.ToString(i),
                        Selected = true
                    });
                }
                else
                {
                    yearList.Add(new SelectListItem
                    {
                        Text = Convert.ToString(i+"年"),
                        Value = Convert.ToString(i),
                        Selected = false
                    });
                }
            }
            return Json(yearList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下拉框绑定月份
        /// </summary>
        /// <returns></returns>
        public JsonResult Getmonth()
        {
            int currentMonth = DateTime.Now.Month;//获取当前月
            var monthList=new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                monthList.Add(new SelectListItem
                {
                    Text = Convert.ToString(i + "月"),
                    Value = Convert.ToString(i),
                    Selected = false
                });
            }
            return Json(monthList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 请假数据统计
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="leaveType">请假类型</param>
        /// <returns></returns>
        public PartialViewResult GetLeaveList(string year, string month, string leaveType)
        {
            var resultList = entity.AL_Apply.Where(x => x.Status == "审核通过" && x.XXID==xxid).ToList();

            if (!string.IsNullOrWhiteSpace(year))
            {
                resultList = resultList.Where(x => x.StartDate.Contains(year)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(month))
            {
                if (month.Length == 1)
                    month = "0" + month;
                resultList = resultList.Where(x => x.StartDate.Substring(5, 2) == month).ToList();
            }
            if (!string.IsNullOrWhiteSpace(leaveType))
            {
                resultList = resultList.Where(x => x.Type == leaveType).ToList();
            }
            resultList = resultList.GroupBy(a => new { a.ApplicantID, a.ApplicantName })
                .Select(
                    b =>
                        new AL_Apply
                        {
                            Hour = b.Sum(s => s.Hour)/8,
                            ApplicantID = b.Key.ApplicantID,
                            ApplicantName = b.Key.ApplicantName,
                        }).ToList();
            ViewBag.TotalHour = resultList.Sum(p => p.Hour);
            return PartialView(resultList);
        }

        public class PersonInfo
        {
            public string repairPerson { get; set; }
            public int yfpCount { get; set; }
            public int ywcCount { get; set; }
        }

        public class VenueApply
        {
            public string applyer { get; set; }
            public int applyCount { get; set; }
            public int auditCount { get; set; }
        }

        public class RepairInfo
        {
            public string repairPerson { get; set; }
            public int jstyCount { get; set; }
            public int jkqCount { get; set; }
            public int jsrjCount { get; set; }
            public int jsjsjCount { get; set; }
            public int mcssCount { get; set; }
            public int mcCount { get; set; }
            public int gssbCount { get; set; }
            public int gnsbCount { get; set; }
        }
    }
}
