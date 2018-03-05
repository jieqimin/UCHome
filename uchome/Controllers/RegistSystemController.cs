using System;
using System.Collections.Generic;
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
using TeacherAwards.Model.ViewModel;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class RegistSystemController : Controller
    {
        private readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private readonly SysDictHelper _sysDictHelper = new SysDictHelper();
        private readonly EntityHelper _entityHelper = new EntityHelper();
        private readonly RegistService _registService = new RegistService();
        private readonly string http = System.Web.Configuration.WebConfigurationManager.AppSettings["APIHttp"];

        private UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        private Guid userId
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
        private Guid schoolId
        {
            get { return user.xxid; }
        }
        private string loginName
        {
            get { return user.loginname; }
        }
        private string userName
        {
            get { return user.username; }
        }

        #region 活动报名页面HTML
        /// <summary>
        /// 活动报名页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult RegistIndex()
        {
            return View();
        }

        public ActionResult RegistIndexData()
        {
            var activeName = Request["ActiveName"];
            var startTime = Request["StartTime"];
            var endTime = Request["EndTime"];

            var list = _registService.GetRegistAcitveList(activeName, startTime, endTime, userId.ToString());
            return View(list);
        }

        #endregion

        #region 个人活动明细页面HTML
        /// <summary>
        /// 活动报名页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult TeacherSignIndex()
        {
            var activeCount = _registService.GetTeacherActiveCountAndScoreModel(userId.ToString());

            return View(activeCount);
        }

        public ActionResult SignIndexData()
        {
            var startTime = Request["StartTime"];
            var endTime = Request["EndTime"];

            var list = _registService.GetTeacherSignAcitveList(startTime, endTime, userId.ToString());
            return View(list);
        }

        #endregion

        #region 活动详情页面HTML
        /// <summary>
        /// 活动详情页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult DetailsAcitve()
        {
            var id = Request["id"];
            var active = _registService.GetActiveDetailsModel(new Guid(id));

            if (active != null)
            {
                active.ActiveFJ = http + "/RegistSystem.Web/UploadImage/" + active.ActiveFJ;
            }

            return View(active);
        }

        #endregion

        #region 报名表详情页面HTML
        /// <summary>
        /// 报名表详情页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult SignDetails()
        {
            var id = Request["id"];

            var active = _registService.GetSingDetailsModel(new Guid(id), userId.ToString());

            if (active != null)
            {
                ViewBag.AuditPJ = active.AdminPJ;
                ViewBag.ResgistXDTH = active.ResgistXDTH;
            }

            return View(active);
        }

        #endregion

        #region 个人报名表详情页面HTML
        /// <summary>
        /// 报名表详情页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult TeacherSignDetails()
        {
            var id = Request["id"];

            var active = _registService.GetSingDetailsModel(new Guid(id), userId.ToString());

            if (active != null)
            {
                active.ActiveFJ = http + "/RegistSystem.Web/UploadImage/" + active.ActiveFJ;
                ViewBag.AuditPJ = active.AdminPJ;
                ViewBag.ResgistXDTH = active.ResgistXDTH;
            }

            return View(active);
        }

        #endregion

        #region 活动报名页面HTML
        /// <summary>
        /// 活动报名页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult Sign()
        {
            var id = Request["id"];
            var active = _registService.GetActiveDetailsModel(new Guid(id));

            if (active != null)
            {
                active.ActiveFJ = http + "/RegistSystem.Web/UploadImage/" + active.ActiveFJ;
            }

            return View(active);
        }

        #endregion

        #region 报名编辑页面HTML
        /// <summary>
        /// 报名编辑页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult EditSign()
        {
            var id = Request["id"];
            var active = _registService.GetSingDetailsModel(new Guid(id), userId.ToString());
            return View(active);
        }

        #endregion

        #region 心得体会页面HTML
        /// <summary>
        /// 心得体会页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveExperience()
        {
            var id = Request["id"];
            var active = _registService.GetSingModel(new Guid(id), userId.ToString());

            return View(active);
        }

        #endregion

        #region 获取活动模板信息Json
        /// <summary>
        /// 获取活动模板信息Json
        /// </summary>
        /// <param name="modelId">模板ID</param>
        /// <returns></returns>
        public JsonResult AllModelListJson(string modelId)
        {
            var modelList = _registService.GetRegistTableModelAndHtmlList(modelId);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 获取活动模板内容信息Json
        /// <summary>
        /// 获取活动模板内容信息Json
        /// </summary>
        /// <param name="registId">报名ID</param>
        /// <returns></returns>
        public JsonResult AllModelValueListJson(Guid registId)
        {
            var modelList = _registService.GetRegistModelValueList(registId);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 获取下拉单选复选数据信息Json
        /// <summary>
        /// 获取下拉单选复选数据信息Json
        /// </summary>
        /// <param name="ddlValue">类型</param>
        /// <returns></returns>
        public JsonResult LoadDDLlListJson(string ddlValue)
        {
            var ddl = _registService.GetDDLIList(ddlValue);

            return Json(ddl, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 保存用户活动报名信息Json
        /// <summary>
        /// 保存用户活动报名信息Json
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="modelId"></param>
        /// <param name="ActiveAudit"></param>
        /// <returns></returns>
        public JsonResult SaveRegistJson(Guid activeId, string modelId, string ActiveAudit, string activeNum)
        {
            var result = "";
            try
            {
                var response = _registService.PostSignDelete(activeId, modelId, userId.ToString());

                if (response.Code == "Success")
                {
                    var registCount = _registService.GetSignList(activeId);

                    if (activeNum != null && registCount != null)
                    {
                        if (Convert.ToInt32(activeNum) > registCount.Count())
                        {
                            RegistTable regTab = new RegistTable();
                            regTab.RegistID = Guid.NewGuid();
                            regTab.ActiveID = activeId;
                            regTab.ModelID = modelId;
                            regTab.RegistTime = DateTime.Now;
                            regTab.RegistPeopleID = userId.ToString();
                            regTab.RegistPeopleName = userName;
                            regTab.GetActiveScore = 0;
                            regTab.QT1 = "1";   //1-校内人员报名，2-校外人员报名
                            regTab.QT2 = schoolId.ToString();  //部门ID

                            //0-未审核，1-审核通过，2-退回
                            if (ActiveAudit == "需要")
                                regTab.ResistStatus = "0";
                            else
                                regTab.ResistStatus = "1";

                            var addResponse = _registService.PostSignAdd(regTab);

                            if (addResponse.Code == "Success")
                            {
                                result = regTab.RegistID.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //result = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 保存用户活动模板信息Json
        /// <summary>
        /// 保存用户活动模板信息Json
        /// </summary>
        /// <param name="signViewModel"></param>
        /// <returns></returns>
        public JsonResult SaveModelValueJson(SignViewModel signViewModel)
        {
            var result = "成功";
            try
            {
                if (!string.IsNullOrEmpty(signViewModel.modelValue))
                {
                    signViewModel.modelValue = HttpUtility.UrlDecode(signViewModel.modelValue);
                }

                _registService.PostModelValueSave(signViewModel);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 用户取消报名Json
        /// <summary>
        /// 用户取消报名Json
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public JsonResult RemoveSignJson(Guid activeId)
        {
            var result = "取消报名失败！";
            try
            {
                var response = _registService.PostSignRemove(activeId, userId.ToString());
                result = response.Message;
            }
            catch (Exception ex)
            {
                //result = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 加载心得体会Json
        /// <summary>
        /// 加载心得体会Json
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public JsonResult LoadExperienceJson(Guid activeId)
        {
            var result = "";
            try
            {
                var modelVal = _registService.GetSingModel(activeId, userId.ToString());

                if (modelVal != null)
                {
                    result = modelVal.ResgistXDTH;
                }
            }
            catch (Exception ex)
            {
                //result = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 保存心得体会Json
        /// <summary>
        /// 保存心得体会Json
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="experience"></param>
        /// <returns></returns>
        public JsonResult SaveExperienceJson(Guid activeId, string experience)
        {
            var result = "保存失败！";
            try
            {
                if (!string.IsNullOrEmpty(experience))
                {
                    experience = HttpUtility.UrlDecode(experience);
                }

                var response = _registService.PostExperienceSave(activeId, userId.ToString(), experience);

                result = response.Message;
            }
            catch (Exception ex)
            {
                //result = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}