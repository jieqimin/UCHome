using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCHome.Model;
using Evaluation.Bll;
using Evaluation.Model;
using Evaluation.Model.Models;

namespace UCHome.Controllers
{
    public class EvaluationController : Controller
    {
        private readonly UCHomeBasePage page = new UCHomeBasePage();
        private readonly EvaluationServices services = new EvaluationServices();

        #region 属性

        private UserInfo user
        {
            get
            {
                return UCHomeBasePage.RequestUser;
            }
        }
        private Guid userId
        {
            get { return user.userid; }
        }
        private string userName
        {
            get { return user.username; }
        }
        private Guid userXXID
        {
            get { return user.xxid; }
        }
        private Guid stuBJID
        {
            get { return user.orgid == null ? Guid.Empty : user.orgid.Value; }
        }
        private string currentYearTerm
        {
            get { return page.CurrentSchTerm; }
        }
        private string userType
        {
            get { return user.usertype.ToLower(); }
        }
        #endregion

        #region 学生评价（学生）
        public ActionResult Index()
        {
            if (userType != "s")
                return PartialView("NoPermission");

            EvaluationInfo model = services.GetData(userId, stuBJID, currentYearTerm);

            return View(model);
        }

        #region 学生自评

        public ActionResult SelfIndex()
        {
            if (userType != "s")
                return PartialView("NoPermission");

            var evaluationInfo = services.GetEvaluationInfo(userId, currentYearTerm, (int)EvalRole.Self, userId);
            if (evaluationInfo == null)
            {
                evaluationInfo = new UCHome_Evaluation();
                evaluationInfo.EvalID = Guid.NewGuid();
            }

            return View(evaluationInfo);
        }

        public ActionResult SelfIndexGrade()
        {
            var gradeList = services.GetGradeList(userXXID);

            return View(gradeList);
        }

        public ActionResult SelfEvalHistory()
        {
            var gradeCode = Request["gradeCode"];
            List<EvaluationGuageGuide> list = new List<EvaluationGuageGuide>();

            var evaluationList = services.GetEvaluationList(userId, gradeCode, (int)EvalRole.Self, userId);
            foreach (var item in evaluationList)
            {
                EvaluationGuageGuide model = new EvaluationGuageGuide();
                var guageId = services.GetGuageId(item.YearTerm, (int)EvalRole.Self);
                model.listGuageGuide = services.GetGuageGuideList(guageId);
                model.listEvaluationItem = services.GetEvaluation_itemListByEvalID(item.EvalID);
                model.YearTerm = item.YearTerm;
                model.EvalContent = item.EvalContent;
                model.EvalTotalScore = item.EvalTotalScore == null ? 0 : item.EvalTotalScore.Value;
                list.Add(model);
            }

            return View(list);
        }

        [HttpPost]
        public JsonResult SaveSelfEvalContent(UCHome_Evaluation model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var evaluationInfo = services.GetEvaluationInfo(userId, currentYearTerm, (int)EvalRole.Self, userId);
                    if (evaluationInfo == null)
                    {
                        model.EvalID = Guid.NewGuid();
                        model.XSID = userId;
                        model.YearTerm = currentYearTerm;
                        model.NJDM = services.GetGradeCode(userXXID, stuBJID);
                        model.EvalRoleCode = (int)EvalRole.Self;
                        model.EvalUserID = userId;
                        model.EvalUserDisplayName = userName;
                        model.EvalDate = DateTime.Now;

                        services.AddUCHome_Evaluation(model);
                    }
                    else
                    {
                        services.EditUCHome_EvalContent(model);
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

        public ActionResult SelfEvalModalContent()
        {
            EvaluationGuageGuide model = new EvaluationGuageGuide();

            var guageId = services.GetGuageId(currentYearTerm, (int)EvalRole.Self);
            model.listGuageGuide = services.GetGuageGuideList(guageId);
            var evaluationInfo = services.GetEvaluationInfo(userId, currentYearTerm, (int)EvalRole.Self, userId);
            if (evaluationInfo != null)
            {
                model.listEvaluationItem = services.GetEvaluation_itemListByEvalID(evaluationInfo.EvalID);
                model.EvalID = evaluationInfo.EvalID;
                model.EvalContent = evaluationInfo.EvalContent;
                model.EvalTotalScore = evaluationInfo.EvalTotalScore == null ? 0 : evaluationInfo.EvalTotalScore.Value;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult SaveSelfEvalScore(EvaluationGuageGuide model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var array1 = Request["GuageGuideLineId"].Split(',');
                    var array2 = Request["EvalItem_Score"].Split(',');
                    decimal totalScore = 0;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        totalScore += decimal.Parse(array2[i]);
                    }

                    if (model.EvalID == Guid.Empty)
                    {
                        UCHome_Evaluation e = new UCHome_Evaluation();
                        e.EvalID = Guid.NewGuid();
                        e.XSID = userId;
                        e.YearTerm = currentYearTerm;
                        e.NJDM = services.GetGradeCode(userXXID, stuBJID);
                        e.EvalTotalScore = totalScore;
                        e.EvalRoleCode = (int)EvalRole.Self;
                        e.EvalUserID = userId;
                        e.EvalUserDisplayName = userName;
                        e.EvalDate = DateTime.Now;

                        services.AddUCHome_Evaluation(e);
                        services.AddUCHome_Evaluation_item(e.EvalID, array1, array2);
                    }
                    else
                    {
                        services.EditUCHome_EvalTotalScore(model.EvalID, totalScore);
                        services.AddUCHome_Evaluation_item(model.EvalID, array1, array2);
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

        #region 学生互评
        public ActionResult HupingIndex()
        {
            if (userType != "s")
                return PartialView("NoPermission");

            var evaluationStudent = services.GetEvaluationStudentInfo(stuBJID, currentYearTerm, (int)EvalRole.Classmate, userId);

            return View(evaluationStudent);
        }

        public ActionResult HupingModalContent()
        {
            EvaluationGuageGuide model = new EvaluationGuageGuide();
            model.XSID = Guid.Parse(Request["xsid"]);

            var guageId = services.GetGuageId(currentYearTerm, (int)EvalRole.Classmate);
            model.listGuageGuide = services.GetGuageGuideList(guageId);
            var evaluationInfo = services.GetEvaluationInfo(model.XSID, currentYearTerm, (int)EvalRole.Classmate, userId);
            if (evaluationInfo != null)
            {
                model.listEvaluationItem = services.GetEvaluation_itemListByEvalID(evaluationInfo.EvalID);
                model.EvalID = evaluationInfo.EvalID;
                model.EvalContent = evaluationInfo.EvalContent;
                model.EvalTotalScore = evaluationInfo.EvalTotalScore == null ? 0 : evaluationInfo.EvalTotalScore.Value;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult SaveHupingScore(EvaluationGuageGuide model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var array1 = Request["GuageGuideLineId"].Split(',');
                    var array2 = Request["EvalItem_Score"].Split(',');
                    decimal totalScore = 0;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        totalScore += decimal.Parse(array2[i]);
                    }

                    if (model.EvalID == Guid.Empty)
                    {
                        UCHome_Evaluation e = new UCHome_Evaluation();
                        e.EvalID = Guid.NewGuid();
                        e.XSID = model.XSID;
                        e.YearTerm = currentYearTerm;
                        e.NJDM = services.GetGradeCode(userXXID, stuBJID);
                        e.EvalContent = model.EvalContent;
                        e.EvalTotalScore = totalScore;
                        e.EvalRoleCode = (int)EvalRole.Classmate;
                        e.EvalUserID = userId;
                        e.EvalUserDisplayName = userName;
                        e.EvalDate = DateTime.Now;

                        services.AddUCHome_Evaluation(e);
                        services.AddUCHome_Evaluation_item(e.EvalID, array1, array2);
                    }
                    else
                    {
                        services.EditUCHome_Evaluation(model.EvalID, model.EvalContent, totalScore);
                        services.AddUCHome_Evaluation_item(model.EvalID, array1, array2);
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

        public ActionResult HupingModalContentDetail()
        {
            EvaluationGuageGuide model = new EvaluationGuageGuide();
            model.XSID = Guid.Parse(Request["xsid"]);

            var guageId = services.GetGuageId(currentYearTerm, (int)EvalRole.Classmate);
            model.listGuageGuide = services.GetGuageGuideList(guageId);

            var evaluationInfo = services.GetEvaluationInfo(model.XSID, currentYearTerm, (int)EvalRole.Classmate, userId);
            model.listEvaluationItem = services.GetEvaluation_itemListByEvalID(evaluationInfo.EvalID);
            model.EvalID = evaluationInfo.EvalID;
            model.EvalContent = evaluationInfo.EvalContent;
            model.EvalTotalScore = evaluationInfo.EvalTotalScore == null ? 0 : evaluationInfo.EvalTotalScore.Value;

            return View(model);
        }

        public ActionResult PilinagIndex()
        {
            var evaluationStudent = services.GetNotEvaluatedStuList(stuBJID, currentYearTerm, (int)EvalRole.Classmate, userId);

            return View(evaluationStudent);
        }

        [HttpPost]
        public JsonResult SavePiliangHupingScore()
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var array1 = Request["StuID"].Split(',');
                    var array2 = Request["EvalTotalScore"].Split(',');
                    var array3 = Request["StrEvalContent"].Split('|');

                    var njdm = services.GetGradeCode(userXXID, stuBJID);
                    services.PiliangAddUCHome_Evaluation(array1, array2, array3, currentYearTerm, njdm, (int)EvalRole.Classmate, userId, userName);
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

        #endregion

        #region 学生评价（班主任）
        public ActionResult BZRIndex()
        {
            var bjid = services.GetBJID(userId.ToString());
            var evaluationStudent = services.GetEvaluationStudentInfo(bjid, currentYearTerm, (int)EvalRole.Headmaster, userId);

            return View(evaluationStudent);
        }

        public ActionResult BZRIndexModalContent()
        {
            EvaluationGuageGuide model = new EvaluationGuageGuide();
            model.XSID = Guid.Parse(Request["xsid"]);

            var guageId = services.GetGuageId(currentYearTerm, (int)EvalRole.Headmaster);
            model.listGuageGuide = services.GetGuageGuideList(guageId);
            var evaluationInfo = services.GetEvaluationInfo(model.XSID, currentYearTerm, (int)EvalRole.Headmaster, userId);
            if (evaluationInfo != null)
            {
                model.listEvaluationItem = services.GetEvaluation_itemListByEvalID(evaluationInfo.EvalID);
                model.EvalID = evaluationInfo.EvalID;
                model.EvalContent = evaluationInfo.EvalContent;
                model.EvalTotalScore = evaluationInfo.EvalTotalScore == null ? 0 : evaluationInfo.EvalTotalScore.Value;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult SaveBZRIndexScore(EvaluationGuageGuide model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var array1 = Request["GuageGuideLineId"].Split(',');
                    var array2 = Request["EvalItem_Score"].Split(',');
                    decimal totalScore = 0;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        totalScore += decimal.Parse(array2[i]);
                    }

                    if (model.EvalID == Guid.Empty)
                    {
                        var bjid = services.GetBJID(userId.ToString());

                        UCHome_Evaluation e = new UCHome_Evaluation();
                        e.EvalID = Guid.NewGuid();
                        e.XSID = model.XSID;
                        e.YearTerm = currentYearTerm;
                        e.NJDM = services.GetGradeCode(userXXID, bjid);
                        e.EvalContent = model.EvalContent;
                        e.EvalTotalScore = totalScore;
                        e.EvalRoleCode = (int)EvalRole.Headmaster;
                        e.EvalUserID = userId;
                        e.EvalUserDisplayName = userName;
                        e.EvalDate = DateTime.Now;

                        services.AddUCHome_Evaluation(e);
                        services.AddUCHome_Evaluation_item(e.EvalID, array1, array2);
                    }
                    else
                    {
                        services.EditUCHome_Evaluation(model.EvalID, model.EvalContent, totalScore);
                        services.AddUCHome_Evaluation_item(model.EvalID, array1, array2);
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

        public ActionResult BZRIndexModalContentDetail()
        {
            EvaluationGuageGuide model = new EvaluationGuageGuide();
            model.XSID = Guid.Parse(Request["xsid"]);

            var guageId = services.GetGuageId(currentYearTerm, (int)EvalRole.Headmaster);
            model.listGuageGuide = services.GetGuageGuideList(guageId);

            var evaluationInfo = services.GetEvaluationInfo(model.XSID, currentYearTerm, (int)EvalRole.Headmaster, userId);
            model.listEvaluationItem = services.GetEvaluation_itemListByEvalID(evaluationInfo.EvalID);
            model.EvalID = evaluationInfo.EvalID;
            model.EvalContent = evaluationInfo.EvalContent;
            model.EvalTotalScore = evaluationInfo.EvalTotalScore == null ? 0 : evaluationInfo.EvalTotalScore.Value;

            return View(model);
        }

        public ActionResult BZRPiliangIndex()
        {
            var bjid = services.GetBJID(userId.ToString());
            var evaluationStudent = services.GetNotEvaluatedStuList(bjid, currentYearTerm, (int)EvalRole.Headmaster, userId);

            return View(evaluationStudent);
        }

        [HttpPost]
        public JsonResult SaveBZRPiliangIndexScore()
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var array1 = Request["StuID"].Split(',');
                    var array2 = Request["EvalTotalScore"].Split(',');
                    var array3 = Request["StrEvalContent"].Split('|');

                    var bjid = services.GetBJID(userId.ToString());
                    var njdm = services.GetGradeCode(userXXID, bjid);
                    services.PiliangAddUCHome_Evaluation(array1, array2, array3, currentYearTerm, njdm, (int)EvalRole.Headmaster, userId, userName);
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

    }
}
