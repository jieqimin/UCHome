using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using OA.BLL;
using OA.Model;
using OA.Model.ViewModel;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class AskForLeaveController : Controller
    {
        OABll oabll = new OABll();
        UCHomeBasePage ucbase=new UCHomeBasePage();
        ZZ_MIFVSEntities entity = new ZZ_MIFVSEntities();
        private static UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        private Guid userid
        {
            get
            {
                return user.userid;
            }
        }
        private Guid xxid
        {
            get
            {
                return user.xxid;
            }
        }
        private string applayname
        {
            get
            {
                return user.username;
            }
        }
        private readonly string _dayHour = ConfigurationManager.AppSettings["DayHour"];
        public ActionResult Edit()
        {
            AL_Apply al = new AL_Apply();
            al.ID = Guid.NewGuid();
            return View(al);
        }
        public ActionResult Index()
        {
            var list = oabll.GetMyLeaveList1(userid,xxid);
         
            return View(list);
        }
        public ActionResult IndexAduit()
        {
            var list = oabll.GetMyAduitList(userid);
            return View(list);
        }
        public ActionResult LeaveStatistics()
        {
            return View();
        }
        public ActionResult TeacherLeaveStatistics()
        {
            return View();
        }
        public ActionResult LeaveDetail()
        {
            ViewBag.SelectYear = Request["SelectYear"];
            ViewBag.UserId = Request["UserId"];

            return View();
        }
        public ActionResult Detials()
        {
            var id = Request["ID"];

            var list = oabll.GetDetials(Guid.Parse(id));
            ViewBag.Approver1 = oabll.GetTeacherGH(new Guid(list.Approver1)).DisName;
            if (string.IsNullOrEmpty(list.Approver2))
            {
                ViewBag.Approver2 = "";
            }
            else
            {
                ViewBag.Approver2 = oabll.GetTeacherGH(new Guid(list.Approver2)).DisName;
            }
        
            return View(list);
        }
        public ActionResult AduitLeave()
        {
            var id = Request["ID"];
            ViewBag.id = id;
            var list = oabll.GetDetials(Guid.Parse(id));
            ViewBag.approver1 = oabll.GetTeacherGH(new Guid(list.Approver1)).Id;  //zy
            //ViewBag.Approver1 = oabll.GetTeacherGH(new Guid(list.Approver1)).DisName;
            if (string.IsNullOrEmpty(list.Approver2))
            {
                ViewBag.Approver2 = "";
            }
            else
            {
                ViewBag.Approver2 = oabll.GetTeacherGH(new Guid(list.Approver2)).DisName;
            }
            //var a = list.ApplicantName;
            //ViewBag.Apptypeof = list.ApplicantName;
            return View(list);
        }
        public ActionResult EditLeave()
        {
            Guid iD = Guid.Parse(Request["ID"]);
            var info = oabll.AL_Apply(iD);
            ViewBag.type = info.Type;
            ViewBag.approver1 = oabll.GetTeacherGH(new Guid(info.Approver1)).Id;
            if (string.IsNullOrEmpty(info.Approver2))
            {
                ViewBag.approver2 = "";
            }
            else
            {
                ViewBag.approver2 = oabll.GetTeacherGH(new Guid(info.Approver2)).Id;
            }
           
            return View(info);
        }
        //转交  转状态是审核中
        public JsonResult EditTransfer(Guid id, Guid ApplicantID)
        {

            var model = entity.AL_Apply.FirstOrDefault(w => w.ID ==id);

            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {

              
                model.Status = "审核中";
                model.Approver1 = ApplicantID.ToString();
                model.Comment2 +=applayname + "转交给了" + model.ApplicantName;
                model.CJRQ = DateTime.Now.ToShortDateString();
                model.LastIP = Request.UserHostAddress;
                // model.Hour = Convert.ToInt32(Request["Day"]) * 8;
              
                // oabll.EditSend(model);
                try
                {
                    entity.SaveChanges();

                   // oabll.EditSend(model);
                }
                catch (Exception ex)
                {
                    statuscode = "500";
                    msg = ex.ToString();
                }
            }

            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditPLeave(AL_Apply model)
        {
            string statuscode = "200";
            string msg = "";

         //   model.StartDate = model.StartDate.Substring(0, 4) + model.StartDate.Substring(5, 2) + model.StartDate.Substring(8, 2);
          //  model.EndDate = model.EndDate.Substring(0, 4) + model.EndDate.Substring(5, 2) + model.EndDate.Substring(8, 2);
           
            if (ModelState.IsValid)
            {
                model.CreateTime = DateTime.Now;
                model.XXID = xxid;
                model.ApplicantID = userid;
                model.ApplicantName = applayname;
                model.Status = "未审核";
                model.CJR = oabll.GetTypeName(Request["Type"]);
                model.CJRQ = DateTime.Now.ToShortDateString();
                model.LastIP = Request.UserHostAddress;
               // model.Hour = Convert.ToInt32(Request["Day"]) * 8;
                model.Hour = Convert.ToInt32(Convert.ToDouble(Request["Day"]) * 8);
                model.Memo = Request["Day"];
                var name = oabll.GetTeacherGH(new Guid(model.Approver1)).DisName;
               // model.Comment2 += "待" + name + "审核";
                // oabll.EditSend(model);
                try
                {
                    oabll.EditLeave(model);
                }
                catch (Exception ex)
                {
                    statuscode = "500";
                    msg = ex.ToString();
                }
            }

            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult EditSend(AL_Apply model)
        {
            string statuscode = "200";
            string msg = "";

            if (ModelState.IsValid)
            {
                try
                {
                    model.CreateTime = DateTime.Now;
                    model.XXID = xxid;
                    model.ApplicantID = userid;
                    model.ApplicantName = applayname;
                    model.Status = "未审核";
                    model.CJR = oabll.GetTypeName(Request["Type"]);
                    model.CJRQ = DateTime.Now.ToShortDateString();
                    model.LastIP = Request.UserHostAddress;
                    //model.Hour = Convert.ToInt32(Request["Day"]) * 8;
                    model.Hour = Convert.ToInt32(Convert.ToDouble(Request["Day"]) * 8);
                    model.Memo = Request["Day"];
                    var name = oabll.GetTeacherGH(new Guid(model.Approver1)).DisName;
                  //  model.Comment2 += "待" + name + "审核";
                    oabll.EditSend(model);
                }
                catch (Exception ex)
                {
                    statuscode = "500";
                    msg = ex.ToString();
                }
            }

            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLeaveTypeList()
        {
            IEnumerable<SelectListItem> list = oabll.GetTypeList().Select(p => new SelectListItem()
            {
                Text = p.CodeName,
                Value = p.CodeID.ToString()
            });

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApprove1PeopleList()
        {
            IEnumerable<SelectListItem> list = oabll.GetDeptApproveList(xxid).Select(p => new SelectListItem()
            {
                Text = p.XM,
                Value = p.DriverID.ToString()
            });

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApprove2PeopleList()
        {
            IEnumerable<SelectListItem> list = oabll.GetSchApproveList(xxid).Select(p => new SelectListItem()
            {
                Text = p.XM,
                Value = p.DriverID.ToString()
            });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NoPass(string Id)
        {
            Guid id = new Guid(Id);
            string code = "1";
            string msg = "";
            try
            {
                oabll.NoPass(id, userid);
            }
            catch (Exception)
            {
                code = "0";
            }

            JsonResult rlt = new JsonResult { Data = new { code, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
            //return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Pass(string Id)
        {
            Guid id = new Guid(Id);
            string code = "1";
            string msg = "";
            try
            {
                oabll.Pass(id, userid);
            }
            catch (Exception)
            {
                code = "0";
            }

            JsonResult rlt = new JsonResult { Data = new { code, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteLeave(Guid Id)
        {
            var code = "1";
            try
            {
                oabll.DeleteLeave(Id);
            }
            catch (Exception)
            {
                code = "0";
            }

            return Json(code, JsonRequestBehavior.AllowGet);
        }

        #region 获取教师请假情况
        /// <summary>
        /// 获取教师请假情况
        /// </summary>
        /// <param name="selectUserID">教师IDs</param>
        /// <param name="selectUserName">教师Names</param>
        /// <param name="SelectYear">年份</param>
        /// <param name="type">按年还是请假类型</param>
        /// <returns></returns>
        public ActionResult GetTeacherLeave(string selectUserID, string selectUserName, string SelectYear, string type)
        {
            var teahcerLeaveViewModel = new TeahcerLeaveViewModel();

            try
            {
                var userIds = selectUserID.Split(',');

                List<Guid?> userIdList = new List<Guid?>();

                var teacherLeaveList = new List<AL_Apply>();

                List<Series> serList = new List<Series>();

                teahcerLeaveViewModel.TeacherNames = selectUserName.Split(',');

                foreach (var uIds in userIds)
                {
                    userIdList.Add(new Guid(uIds));
                }

                switch (type)
                {
                    case "年份":
                        #region 年份

                        teacherLeaveList = oabll.GetLeaveListByTeahcerIdListAndYear(userIdList, SelectYear);

                        if (teacherLeaveList.Any())
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                Series s = new Series();
                                s.id = i;
                                s.type = "column";
                                s.name = i + "月";

                                var month = "";
                                if (i < 10)
                                    month = "0" + i;
                                else
                                    month = i.ToString();

                                float?[] d = new float?[userIdList.Count];

                                for (int j = 0; j < userIdList.Count; j++)
                                {
                                    var teacherHour =
                                        teacherLeaveList.Where(
                                            e =>
                                                e.ApplicantID == userIdList[j] &&
                                                    e.StartDate.StartsWith(SelectYear + "-" + month));
                                              //  e.StartDate.StartsWith(SelectYear +"-"+ month));

                                    if (teacherHour.Any())
                                    {
                                        d[j] = FormateHour((int)teacherHour.Sum(e => e.Hour));
                                    }
                                    else
                                    {
                                        d[j] = 0;
                                    }
                                }

                                s.data = d;

                                serList.Add(s);
                            }

                            teahcerLeaveViewModel.SeriesList = serList;
                        }

                        #endregion
                        break;
                    case "请假类型":
                        #region 请假类型

                        teacherLeaveList = oabll.GetLeaveListByTeahcerIdListAndYear(userIdList, SelectYear);

                        if (teacherLeaveList.Any())
                        {
                            var typeList = oabll.GetTypeNameList();

                            for (int i = 0; i < typeList.Count; i++)
                            {
                                Series s = new Series();
                                s.id = i;
                                s.type = "column";
                                s.name = typeList[i];

                                float?[] d = new float?[userIdList.Count];

                                for (int j = 0; j < userIdList.Count; j++)
                                {
                                    var teacherHour =
                                        teacherLeaveList.Where(
                                            e =>
                                                e.ApplicantID == userIdList[j] &&
                                                e.CJR == typeList[i]);

                                    if (teacherHour.Any())
                                    {
                                        d[j] = FormateHour((int)teacherHour.Sum(e => e.Hour));
                                    }
                                    else
                                    {
                                        d[j] = 0;
                                    }
                                }

                                s.data = d;

                                serList.Add(s);
                            }

                            teahcerLeaveViewModel.SeriesList = serList;
                        }

                        #endregion
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(NewtonsoftJson(teahcerLeaveViewModel));
        }
        #endregion

        #region 获取单个教师请假情况
        /// <summary>
        /// 获取单个教师请假情况
        /// </summary>
        /// <param name="SelectYear">年份</param>
        /// <param name="type">按年还是请假类型</param>
        /// <param name="userType">判断当前登录教师还是选择的教师</param>
        /// <param name="userId">教师ID</param>
        /// <returns></returns>
        public JsonResult GetTeacherLeaveByID(string SelectYear, string type, string userType = "", string userId = "")
        {
            var teahcerLeaveViewModel = new TeahcerLeaveViewModel();

            try
            {
                var teacherLeaveList = new List<AL_Apply>();
                List<Guid?> userIdList = new List<Guid?>();

                if (!string.IsNullOrEmpty(userType))
                    userIdList.Add(new Guid(userId));
                else
                    userIdList.Add(userid);

                List<SeriesPie> serList = new List<SeriesPie>();

                switch (type)
                {
                    case "年份":
                        #region 年份

                        teacherLeaveList = oabll.GetLeaveListByTeahcerIdListAndYear(userIdList, SelectYear);

                        if (teacherLeaveList.Any())
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                SeriesPie s = new SeriesPie();
                                s.name = i + "月";

                                var month = "";
                                if (i < 10)
                                    month = "0" + i;
                                else
                                    month = i.ToString();

                                var teacherHour =
                                        teacherLeaveList.Where(
                                       //  e => e.StartDate.StartsWith(SelectYear + month));
                                            e => e.StartDate.StartsWith(SelectYear + "-" + month));

                                if (teacherHour.Any())
                                {
                                    s.data = FormateHour((int)teacherHour.Sum(e => e.Hour));

                                    serList.Add(s);
                                }
                            }

                            teahcerLeaveViewModel.SeriesPieList = serList;
                        }

                        #endregion
                        break;
                    case "请假类型":
                        #region 请假类型

                        teacherLeaveList = oabll.GetLeaveListByTeahcerIdListAndYear(userIdList, SelectYear);

                        if (teacherLeaveList.Any())
                        {
                            var typeList = oabll.GetTypeNameList();

                            for (int i = 0; i < typeList.Count; i++)
                            {
                                SeriesPie s = new SeriesPie();
                                s.name = typeList[i];

                                var teacherHour =
                                        teacherLeaveList.Where(
                                            e => e.CJR == typeList[i]);

                                if (teacherHour.Any())
                                {
                                    s.data = FormateHour((int)teacherHour.Sum(e => e.Hour));
                                   // s.data = FormateHour((int)teacherHour.Sum(e => e.Hour));
                                    serList.Add(s);
                                }
                            }

                            teahcerLeaveViewModel.SeriesPieList = serList;
                        }

                        #endregion
                        break;
                }

                if (teacherLeaveList.Any())
                {
                    List<LeaveViewModel> leaveViewModelList = new List<LeaveViewModel>();

                    foreach (var leave in teacherLeaveList)
                    {
                        LeaveViewModel _leaveView = new LeaveViewModel
                        {
                            Date = leave.StartDate + "—" + leave.EndDate,
                            DayNum = FormateHour((int)leave.Hour.Value),
                            LeaveType = leave.CJR
                        };
                        leaveViewModelList.Add(_leaveView);
                    }
                    teahcerLeaveViewModel.TeacherLeaveDetailList = leaveViewModelList;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(teahcerLeaveViewModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 小时数换算到天
        /// <summary>
        /// 小时数换算到天
        /// </summary>
        /// <param name="hour">小时数</param>
        /// <returns></returns>
        private float FormateHour(int hour)
        {
            float temp = float.Parse(_dayHour);
            float result = 0;
            if (hour > 0)
                result = (hour / temp);
            return result;
        }

        #endregion

        #region 转换JSON
        /// <summary>
        /// 转换JSON
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <returns></returns>
        public static string NewtonsoftJson(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None);
        }
        #endregion
    }
}