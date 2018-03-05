using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using OA.BLL;
using TeacherAwards.BLL.Public;
using TeacherAwards.BLL.TeacherAward;
using TeacherAwards.Model.Entity;
using TeacherAwards.Model.QueryModel;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class TeacherInfoController : Controller
    {
        private readonly TeacherContact _teacherContact = new TeacherContact();
        readonly  UCHomeBasePage ucbase = new UCHomeBasePage();
        private readonly TeacherAwardService _teacherAwardService = new TeacherAwardService();
        private readonly TeacherService _teacherService = new TeacherService();
        private readonly SysDictHelper _sysDictHelper = new SysDictHelper();
        private readonly string http = System.Web.Configuration.WebConfigurationManager.AppSettings["APIHttp"];
        private readonly EntityHelper _entityHelper = new EntityHelper();
        private UserInfo user
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
        private string userName
        {
            get { return user.username; }
        }
        private string loginName
        {
            get { return user.loginname; }
        }
        private Guid schoolId
        {
            get { return user.xxid; }
        }
        #region 教师基本信息页面HTML
        /// <summary>
        /// 教师基本信息页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Http = http;
            ViewBag.TeacherId = loginId;
            return View();
        }

        #endregion

        #region 教师基本信息页面HTML
        /// <summary>
        /// 教师基本信息页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentScore()
        {
            return View();
        }

        #endregion

        #region 教师家庭信息页面HTML
        /// <summary>
        /// 教师家庭信息页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexFamily()
        {
            var result = _teacherService.GetTeacherFamilyList(loginId);
            return View(result);
        }

        #endregion

        #region 教师家庭信息详情页面HTML
        /// <summary>
        /// 教师家庭信息详情页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult DetailsFamily(Guid id)
        {
            var result = _teacherService.GetTeacherFamilyInfoModel(id);
            return View(result);
        }

        #endregion

        #region 教师家庭信息创建页面HTML
        /// <summary>
        /// 教师家庭信息创建页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexFamilyCreate()
        {
            var teacher_ZZJG0302 = new Teacher_ZZJG0302();
            teacher_ZZJG0302.JTCYID = Guid.NewGuid();

            return View("EditFamily", teacher_ZZJG0302);
        }

        #endregion

        #region 教师家庭信息编辑页面HTML
        /// <summary>
        /// 教师家庭信息编辑页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexFamilyEdit(Guid id)
        {
            var teacher_ZZJG0302 = _teacherService.GetTeacherFamilyInfoModel1(id);

            return View("EditFamily", teacher_ZZJG0302);
        }

        #endregion

        #region 教师家庭信息编辑Edit
        /// <summary>
        /// 教师家庭信息编辑Edit
        /// </summary>
        /// <param name="teacher_ZZJG0302">家庭信息类</param>
        /// <returns></returns>
        public JsonResult PostFamilyEdit(Teacher_ZZJG0302 teacher_ZZJG0302)
        {
            teacher_ZZJG0302.XXID = schoolId;
            teacher_ZZJG0302.JSID = loginId;
            teacher_ZZJG0302.CJR = loginName;
            return Json(_teacherService.PostFamilyEdit(teacher_ZZJG0302), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 删除教师家庭信息Delete
        /// <summary>
        /// 删除教师家庭信息Delete
        /// </summary>
        /// <param name="Id">主键Id</param>
        /// <returns></returns>
        public JsonResult PostFamillyDelete(Guid Id)
        {
            return Json(_teacherService.PostFamillyDelete(Id), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 教师工作信息页面HTML
        /// <summary>
        /// 教师工作信息页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexWork()
        {
            var result = _teacherService.GetTeacherWorkList(loginId);
            return View(result);
        }

        #endregion

        #region 教师工作信息详情页面HTML
        /// <summary>
        /// 教师工作信息详情页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult DetailsWork(Guid id)
        {
            var result = _teacherService.GetTeacherWorkInfoModel(id);
            return View(result);
        }

        #endregion

        #region 教师工作信息创建页面HTML
        /// <summary>
        /// 教师工作信息创建页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexWorkCreate()
        {
            var teacher_ZZJG05 = new Teacher_ZZJG05();
            teacher_ZZJG05.GZJLID = Guid.NewGuid();

            return View("EditWork", teacher_ZZJG05);
        }

        #endregion

        #region 教师工作信息编辑页面HTML
        /// <summary>
        /// 教师工作信息编辑页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexWorkEdit(Guid id)
        {
            var teacher_ZZJG05 = _teacherService.GetTeacherWorkInfoModel(id);

            return View("EditWork", teacher_ZZJG05);
        }

        #endregion

        #region 教师工作信息编辑Edit
        /// <summary>
        /// 教师工作信息编辑Edit
        /// </summary>
        /// <param name="teacher_ZZJG05">工作信息类</param>
        /// <returns></returns>
        public JsonResult PostWorkEdit(Teacher_ZZJG05 teacher_ZZJG05)
        {
            teacher_ZZJG05.XXID = schoolId;
            teacher_ZZJG05.XM = userName;
            teacher_ZZJG05.JZGID = loginId;
            teacher_ZZJG05.CJR = loginName;
            teacher_ZZJG05.GH = loginName;
            return Json(_teacherService.PostWorkEdit(teacher_ZZJG05), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 删除教师工作信息Delete
        /// <summary>
        /// 删除教师工作信息Delete
        /// </summary>
        /// <param name="Id">主键Id</param>
        /// <returns></returns>
        public JsonResult PostWorkDelete(Guid Id)
        {
            return Json(_teacherService.PostWorkDelete(Id), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 学生基本信息页面HTML
        /// <summary>
        /// 学生基本信息页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentIndex()
        {
            ViewBag.Http = http;
            ViewBag.hidStuId = loginId;
            return View();
        }

        #endregion

        #region 教师通讯录页面HTML
        /// <summary>
        /// 教师通讯录页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult TeacherIndex()
        { 
            var list = _teacherContact.GetMySchoolTeacherList(schoolId);
            return View(list);
        }

        #endregion

        #region 教师基本信息详情页面HTML
        /// <summary>
        /// 教师基本信息详情页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult Details()
        {
            ViewBag.Http = http;
            ViewBag.TeacherId = loginId;
            return View();
        }

        #endregion

        #region 教师奖励信息页面HTML
        /// <summary>
        /// 教师奖励信息页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexTeaAwards()
        {
            CommonQueryModel commonQueryModel = new CommonQueryModel();
            commonQueryModel.TeacherId = loginId;
            var teacherAwardsList = _teacherAwardService.TeacherAwardsList(commonQueryModel).ToList();
            return View(teacherAwardsList);
        }

        #endregion

        #region 教师奖励信息审核页面HTML
        /// <summary>
        /// 教师奖励信息审核页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexAdminAwards()
        {
            CommonQueryModel commonQueryModel = new CommonQueryModel();
            commonQueryModel.AdminTeacherId = loginId.ToString();
            var teacherAdminAwardsList = _teacherAwardService.TeacherAdminAwardsList(commonQueryModel).ToList();
            return View(teacherAdminAwardsList);
        }

        #endregion

        #region 教师奖励信息新建页面HTML
        /// <summary>
        /// 教师奖励信息编辑页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult TeaAwardsCreate()
        {
            Teacher_JCJG0110 award = new Teacher_JCJG0110();
            award.JLID = Guid.NewGuid();
            ViewBag.Images = "";
            return View("TeaAwardsEdit", award);
        }

        #endregion

        #region 教师奖励信息编辑页面HTML
        /// <summary>
        /// 教师奖励信息编辑页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult TeaAwardsEdit(Guid ID)
        {
            var teaAward = _teacherAwardService.TeacherAwards(ID);
            ViewBag.Images = teaAward.FJUrl;
            return View(teaAward);
        }

        [HttpPost]
        public JsonResult EditSend(Teacher_JCJG0110 model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    model.XXID = schoolId;
                    model.JSID = loginId;
                    model.JLJBM = Request.Form["JLJBM1"];
                    model.JLDJM = Request.Form["JLDJM1"];
                    model.HJLBM = Request.Form["HJLBM1"];
                    model.JLFSM = Request.Form["JLFSM1"];
                    model.LastTime = DateTime.Now.ToString("yyyyMMdd");
                    model.LastIP = Request.UserHostAddress;
                    model.CJRQ = DateTime.Now.ToString("yyyyMMdd");
                    model.LogID = Guid.NewGuid();
                    //model.CJR = Request.Form["Approver"];//审核人
                    model.FJUrl = Request.Form["hidImages"];
                    model.HJRQ = Convert.ToDateTime(Request.Form["HJRQ"]).ToString("yyyyMMdd");
                    var response = _teacherAwardService.PostTeacherAwardEdit(model);
                    if (response.Code == "Success")
                    {
                        msg = response.Message;
                    }
                    else
                    {
                        statuscode = "500";
                        msg = response.Message;
                    }
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

        #endregion

        #region 教师奖励信息详情页面HTML
        /// <summary>
        /// 教师奖励信息详情页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult AwardDetails(string ID)
        {
            //var id = PKID;
            var award = _teacherAwardService.TeacherAwardDetails(new Guid(ID));
            ViewBag.Images = award.FJUrl;
            return View(award);
        }

        #endregion

        #region 教师奖励信息详情页面HTML
        /// <summary>
        /// 教师奖励信息详情页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult AwardAudit()
        {
            var id = Request["ID"];
            var award = _teacherAwardService.TeacherAwardDetails(new Guid(id));

            ViewBag.Images = award.FJUrl;

            return View(award);
        }

        #endregion

        #region 删除教师奖励信息Delete
        /// <summary>
        /// 删除教师奖励信息Delete
        /// </summary>
        /// <param name="Id">主键Id</param>
        /// <returns></returns>
        public JsonResult PostTeacherAwardsDelete(Guid Id)
        {
            return Json(_teacherAwardService.PostTeacherAwardsDelete(Id), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取下拉框列表Json
        /// <summary>
        /// 获取下拉框列表Json
        /// </summary>
        /// <para name="codeType">类别</para>
        /// <returns></returns>
        public JsonResult GetSyDictSelectItemJson(string codeType)
        {
            return Json(_sysDictHelper.GetSyDictSelectItemList(codeType), JsonRequestBehavior.AllowGet);
        }
        #endregion

        //#region 获取校审核人员Json
        ///// <summary>
        ///// 获取校审核人员Json
        ///// </summary>
        ///// <para name="type">类型</para>
        ///// <returns></returns>
        //public JsonResult GetSchApproveJson(string type)
        //{
        //    var msc = "";
        //    try
        //    {
        //        var resultInfo = _teacherAwardService.GetSchApproveList(type, schoolId);
        //        return Json(resultInfo, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        msc = ex.Message;
        //        return Json(msc, JsonRequestBehavior.AllowGet);
        //    }
            
        //}
        //#endregion

        //#region 奖励审核Json
        ///// <summary>
        ///// 奖励审核Json
        ///// </summary>
        ///// <param name="Id">奖励ID</param>
        ///// <param name="type">审核状态</param>
        ///// <returns></returns>
        //public JsonResult PassOrNoPass(Guid Id, string type)
        //{
        //    string code = "1";
        //    string msg = "";
        //    try
        //    {
        //        if (type == "2")
        //        {
        //            //通过
        //            var response = _teacherAwardService.PostTeacherAwardsAudit(Id, type);

        //            if (response.Code != "Success")
        //            {
        //                msg = response.Message;
        //                code = "0";
        //            }
        //        }
        //        else
        //        {
        //            //不通过
        //            var response = _teacherAwardService.PostTeacherAwardsAudit(Id, type);
        //            if (response.Code != "Success")
        //            {
        //                msg = response.Message;
        //                code = "0";
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        code = "0";
        //    }

        //    JsonResult rlt = new JsonResult { Data = new { code, msg } };
        //    return Json(rlt, JsonRequestBehavior.AllowGet);
        //    //return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //#endregion

        #region 获取家庭关系列表Json
        /// <summary>
        /// 获取家庭关系列表Json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFamillyGXJson()
        {
            return Json(_entityHelper.GetFamillyList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 获取政治面貌列表Json
        /// <summary>
        /// 获取政治面貌列表Json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetZZMMJson()
        {
            return Json(_entityHelper.GetZhengZhiMianMao(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 获取专业技术职务列表Json
        /// <summary>
        /// 获取专业技术职务列表Json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWorkZWJson()
        {
            return Json(_entityHelper.GetWorkZYList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 教师学期计划页面HTML
        /// <summary>
        /// 教师学期计划页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult TeacherTermPlan()
        {
            var yearTerm = "";
            int defaultYear = DateTime.Now.Year;
            DateTime time = DateTime.Parse(DateTime.Now.Year + "-09-01");
            DateTime nowDate = DateTime.Now;

            if (nowDate < time)
                defaultYear--;
            var month = DateTime.Now.Month.ToString();
            if ("9,10,11,12,1,2".Contains(month))
                yearTerm = defaultYear + "1";
            else
                yearTerm = defaultYear + "2";

            var termPlan = _teacherService.GetTeacherTermPlanModel(loginId, yearTerm);

            BindDropByDownLoad(yearTerm);

            return View(termPlan);
        }

        #endregion

        #region 下拉框绑定
        /// <summary>
        /// 下拉框绑定
        /// </summary>
        /// <param name="yearTerm">学年学期</param>
        public void BindDropByDownLoad(string yearTerm = "")
        {
            var xnxqList = _sysDictHelper.GetXNXQList(yearTerm);

            ViewBag.SchYearTerm = xnxqList;
        }
        #endregion

        #region 获取教师学期计划Json
        /// <summary>
        /// 获取教师学期计划Json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTeacherTermPlanJson(string yearTerm)
        {
            return Json(_teacherService.GetTeacherTermPlanModel(loginId, yearTerm), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 保存教师学期计划Json
        /// <summary>
        /// 保存教师学期计划Json
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveTeacherTermPlanJson(TeacherTermPlan teacherTermPlan)
        {
            if (!string.IsNullOrEmpty(teacherTermPlan.TermPlan))
            {
                teacherTermPlan.TermPlan = HttpUtility.UrlDecode(teacherTermPlan.TermPlan);
            }
            if (!string.IsNullOrEmpty(teacherTermPlan.TermSummary))
            {
                teacherTermPlan.TermSummary = HttpUtility.UrlDecode(teacherTermPlan.TermSummary);
            }

            teacherTermPlan.PlanId = Guid.NewGuid();
            teacherTermPlan.TeacherId = loginId;

            return Json(_teacherService.PostTeacherTermPlanEdit(teacherTermPlan), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}