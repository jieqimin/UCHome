using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.Expressions;
using EvalutionSystem.Bll;
using EvalutionSystem.Model;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.OpenXml4Net.OPC.Internal;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using ServiceStack.Common.Extensions;
using Student.Model;
using StudentAffaris.BLL;
using StudentAffaris.Model;
using UCHome.Model;
using UCHome.Models;
using Wicresoft.Common.CCS;
using EvaluationSystemModel = EvalutionSystem.Model.EvaluationSystem;
namespace UCHome.Controllers
{
    public class StudentAffairsController : Controller
    {
        #region UserInfo
        private static UserInfo user
        {
            get
            {
                return UCHomeBasePage.RequestUser;
            }
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
        private string userName
        {
            get
            {
                return user.username;
            }
        }
        #endregion

        #region 班级管理

        #region 活动管理
        public ActionResult ClassActivity(string type)
        {
            ViewBag.Type = type;
            return View();
        }

        public ActionResult ClassActivityList()
        {
            string classId = "";
            if (user.usertype == "p")
            {
                classId = user.childinfo.childbjid.ToString();
            }
            if (user.usertype == "s")
            {
                classId = (user.orgid ?? Guid.Empty).ToString();
            }
            var list = StudentAffarisHelper.getClassActivitiesByType(classId, "");
            return View(list);
        }

        public JsonResult GetBZRClasses()
        {
            try
            {
                var result = StudentAffarisHelper.GetTeacherChargeClass(userid);
                return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetClassActivities(string classId, string type = "", int top = 0)
        {
            try
            {
                var result = StudentAffarisHelper.getClassActivitiesByType(classId, type, top)
                    .Select(
                        p => new
                        {
                            p.ID,
                            p.Title,
                            p.Place,
                            p.DateTime,
                            p.TermID,
                            p.CreateTime,
                            p.Type
                        });
                return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Create()
        {
            Class_SocialPractice practice = new Class_SocialPractice();
            practice.ID = Guid.NewGuid();
            ViewBag.Classes =
                StudentAffarisHelper.GetTeacherChargeClass(userid)
                    .Select(p => new SelectListItem() { Text = p.Value, Value = p.Key.ToString() })
                    .ToList();
            return View(practice);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult CreateSend(Class_SocialPractice model)
        {
            try
            {
                model.DateTime = model.DateTime.Replace("-", "");
                model.Creator = userName;
                model.CreateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                model.LogID = userid;
                model.TermID = StudentAffarisHelper.GetSchoolCurrentTerm(xxid);
                var flag = StudentAffarisHelper.AddOrUpdateClassSocialPractice(model);
                return Json(new { flag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ActivityDetail(Guid activityID)
        {
            try
            {
                var result = StudentAffarisHelper.GetDetail(activityID);
                return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteActivity(Guid activityId)
        {
            try
            {
                var result = StudentAffarisHelper.DeleteActivity(activityId);
                return Json(new { flag = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult ClassActivityStatistic()
        {
            return View();
        }

        public JsonResult GetClassTerms(string classId)
        {
            try
            {
                var result = StudentAffarisHelper.GetClassTerms(classId);
                return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult StatiscClassActivity(Guid classId, string termid)
        {
            try
            {
                var result = StudentAffarisHelper.StatisticClassActivity(classId, termid);
                return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 班级评比

        #endregion
        #endregion

        #region Evaluationg System
        public ActionResult EvaluationSystemIndex()
        {
            return View();
        }
        public JsonResult GetAllEvaluationSystem()
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var result = helper.GetAllSystem().Select(p => new
                    {
                        p.Id,
                        pId = p.ParentId,
                        p.Name,
                        p.Order,
                        p.Describe
                    }).ToList().Select(p => new
                    {
                        p.Id,
                        p.pId,
                        p.Name,
                        p.Order,
                        p.Describe,
                        //isParent=p.Count>0
                    }).OrderBy(p => p.Order);
                    return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult AddEvaluationSystem(EvaluationSystemModel model)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var flag = helper.AddSystem(model);
                    return Json(new { flag }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UpdateEvaluationSystems(List<EvaluationSystemModel> models)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var flag = helper.UpdateSystems(models);
                    return Json(new { flag }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UpdateEvaluationSystem(EvaluationSystemModel model)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var flag = helper.UpdateSystem(model);
                    return Json(new { flag = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteEvaluationSystem(Guid id)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var flag = helper.DeleteSystem(id);
                    return Json(new { flag }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message });
            }
        }
        public ActionResult EvaluationProjectIndex(string projectId = "")
        {
            ViewBag.ProjectId = projectId;
            return View();
        }
        public JsonResult GetAllSystemRecursion()
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var result = helper.GetAllSystem().Where(p => p.ParentId == null).Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Order,
                        p.Describe,
                        Properties = p.EvaluationSystem1.Select(q => new
                        {
                            q.Id,
                            q.Name,
                            q.Order,
                            q.Describe,
                            Properties = q.EvaluationSystem1.Select(m => new
                            {
                                m.Id,
                                m.Name,
                                m.Order,
                                m.Describe,
                            })
                        })
                    }).ToList().OrderBy(p => p.Order).Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Describe,
                        Properties = p.Properties.Select(q => new
                            {
                                q.Id,
                                q.Name,
                                q.Order,
                                q.Describe,
                                Properties = q.Properties.OrderBy(m => m.Order).ToList()
                            }).OrderBy(q => q.Order).ToList()
                    });
                    return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message });
            }
        }
        public JsonResult AddOrUpdateEvalutionProject(EvaluationProject model, List<Evaluation_RelSystemAndProject> propertyIds)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var flag = false;
                    if (model.Id == Guid.Empty)
                    {
                        flag = helper.AddProject(model, propertyIds);
                    }
                    else
                    {
                        flag = helper.UpdateProject(model, propertyIds);
                    }
                    return Json(new { flag }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message });
            }

        }
        public ActionResult EvaluationProjectList()
        {
            return View();
        }
        public JsonResult GetEvaluationProjectDetail(Guid projectId)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var result = helper.GetProjectDetail(projectId);
                    return Json(new JavaScriptSerializer().Serialize(new { flag = true, result = result }), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetAllProjects()
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var result =
                        helper.GetAllProjects().OrderBy(p => p.Order).Select(p => new { p.Id, p.CreateDate, p.Describe, p.Name }).ToList();
                    return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message });
            }
        }
        public JsonResult EvaluationProjectDelete(Guid ProjectId)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var result =
                        helper.DeleteProject(ProjectId);
                    return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ConfigEvaluationProjectRelWithObject()
        {
            return View();
        }
        public JsonResult GetProjectColomns(Guid projectId)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var result = helper.GetProjectAllPropery(projectId);
                    return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult EvaluationModel1t(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }
        public JsonResult GetClassEvaluationRecord(Guid classId, DateTime beginDate, DateTime endDate, Guid projectId)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    beginDate = beginDate.Date;
                    endDate = endDate.Date;
                    var result = helper.GetClassRecord(classId, beginDate, endDate, projectId);
                    return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetClassStudentsList(Guid classId)
        {
            try
            {
                var result = StudentAffarisHelper.GetClassStudentInfos(classId);
                return Json(new { flag = true, result = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
                throw;
            }
        }
        public JsonResult AddNewEvaluationRecord(List<EvaluationSystemRecord> postData)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    postData.ForEach(item => { item.LogId = userid; item.CreateDate = DateTime.Now; });
                    var result = helper.AddRecored(postData);
                    return Json(new { flag = true, result = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteEvaluationRecord(int[] ids)
        {
            try
            {

                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    if (user.usertype == "t")
                    {
                        var result = helper.DeleteRecords(ids);
                        return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { flag = false, message = user.usertype }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EvaluationModel1Admin(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }
        public ActionResult EvaluationModel2t(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }
        public ActionResult EvaluationModel3t(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }
        public JsonResult GetClassStudentRecords(Guid classId, DateTime HappenDate, Guid ProjectId)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var result = helper.GetClassRecordInDay(classId, HappenDate, ProjectId);
                    return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult EvalutionStatisticByStudentT(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }

        public JsonResult GetStatisticDataByStudent(DateTime beginDate, DateTime endDate, Guid projectId, Guid classId)
        {
            try
            {
                using (EvaluationSystemHelper helper = new EvaluationSystemHelper())
                {
                    var result = helper.GetStatisticDataByStudent(beginDate.Date, endDate.Date, projectId, classId);
                    return Json(new { flag = true, result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EvalutionStatisticByStudentAdmin(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }
        [HttpPost]
        public JsonResult GetXLSFileByTable(ExcelTable table)
        {
            try
            {
                NPOIExcelHelper helper = new NPOIExcelHelper();
                var HSSFWORK = helper.SaveTableToExcel(new List<ExcelTable>() { table });
                var fileName = SaveHsskworkStream(HSSFWORK);
                return Json(new { flag = true, result = fileName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { flag = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public FileResult GetXLSFileByName(string fileName, string RelName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(RelName))
                return null;

            try
            {
                var tempFilePath = System.Configuration.ConfigurationManager.AppSettings["fileTempPath"];
                var filePath = Path.Combine(tempFilePath, fileName);
                if (!System.IO.File.Exists(filePath))
                    return null;
                var fileStreambytes = System.IO.File.ReadAllBytes(filePath);
                System.IO.File.Delete(filePath);
                return File(fileStreambytes, "application/vnd.ms-excel", Url.Encode(RelName));
                //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

                //var bytes = System.IO.File.OpenRead(filePath);
                //HSSFWorkbook hssfWorkbook = new HSSFWorkbook(bytes);
                //MemoryStream file = new MemoryStream();
                //hssfWorkbook.Write(file);
                //bytes.Close();
                //response.Content = new ByteArrayContent(file.GetBuffer());
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment"){FileName = RelName};
                ////
                //return response;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public JsonResult GetStatisticFile(LevelModel levelModel, Guid projectId, Guid[] classIds, DateTime beginTime, DateTime endTime)
        {
            try
            {
                beginTime = beginTime.Date;
                endTime = endTime.Date;
                
                using (EvaluationSystemEntities evaluation = new EvaluationSystemEntities())
                using (StudentEntities studentEntities = new StudentEntities())
                {
                    var project = evaluation.EvaluationProject.Single(p => p.Id == projectId);
                    var evalutionSystem = evaluation.EvaluationSystem.Single(p => p.Id == project.SystemId).EvaluationSystem1.OrderBy(p=>p.Order);
                    HSSFWorkbook hssfWorkbook = new HSSFWorkbook();
                    //居中
                    ICellStyle cellStyle = hssfWorkbook.CreateCellStyle();
                    cellStyle.VerticalAlignment = VerticalAlignment.Center;
                    cellStyle.Alignment = HorizontalAlignment.Center;
                    //表头
                    var titleStyle = hssfWorkbook.CreateCellStyle();
                    titleStyle.CloneStyleFrom(cellStyle);
                    titleStyle.FillForegroundColor = HSSFColor.Grey40Percent.Index;
                    titleStyle.FillPattern = FillPattern.SolidForeground;
                    titleStyle.FillBackgroundColor = HSSFColor.Grey40Percent.Index;
                    var font = hssfWorkbook.CreateFont();
                    font.IsBold = true;
                    font.FontHeightInPoints = 14;
                    titleStyle.SetFont(font);
                    var classesInfo = studentEntities.View_Basic_SchClass.Where(p => classIds.Contains(p.BJID))
                        .Select(p => new {p.BJID, p.NJMC, p.XZBMC}).OrderBy(p=>p.XZBMC).ToList();
                    var classesStudents = studentEntities.Stu_ZZXS0101.Where(p => classIds.Contains(p.BJID.Value))
                        .Select(p => new
                        {
                            p.BJID,
                            p.XSID,
                            p.XJH,
                            p.XM
                        }).ToList();
                    var classesRecords = evaluation.EvaluationSystemRecord.Where(p =>
                        p.HappenDate >= beginTime && p.HappenDate <= endTime && p.ProjectId == projectId &&
                        classIds.Contains(p.ClassId))
                        .Select(p => new
                        {
                            p.StudentId,
                            p.PropertyId,
                            p.Score
                        });
                    foreach (var classInfo in classesInfo)
                    {
                        var sheet = hssfWorkbook.CreateSheet(classInfo.NJMC + classInfo.XZBMC);

                        var colIndex = 0;
                        var rowIndex = 0;
                        CreateMergedCell(sheet, rowIndex, colIndex++, 1, "学生姓名", titleStyle);
                        CreateMergedCell(sheet, rowIndex, colIndex++, 1, "学籍号", titleStyle);
                        CreateMergedCell(sheet, rowIndex, colIndex++, 1, "记分总计", titleStyle);
                        if (levelModel.on)
                        {
                            CreateMergedCell(sheet, rowIndex, colIndex++, 1, "得分", titleStyle);
                            CreateMergedCell(sheet, rowIndex, colIndex++, 1, "等级", titleStyle);
                        }
                        CreateMergedCell(sheet, rowIndex, colIndex++, 1, "记分类别", titleStyle);
                        CreateMergedCell(sheet, rowIndex, colIndex++, 1, "记分项", titleStyle);
                        CreateMergedCell(sheet, rowIndex, colIndex, 1, "记分值", titleStyle);
                        rowIndex++;
                        var classStudent =
                            classesStudents.Where(p => p.BJID == classInfo.BJID).OrderBy(p => p.XJH).ToList();
                        foreach (var student in classStudent)
                        {
                            var studentRecords = classesRecords.Where(p => p.StudentId == student.XSID).ToList();
                            var score = studentRecords.Sum(p => p.Score);
                            var propertyIds = studentRecords.Select(p => p.PropertyId).Distinct();
                            var rowSpan = propertyIds.Count();
                            colIndex = 0;
                            CreateMergedCell(sheet, rowIndex, colIndex++, rowSpan, student.XM, cellStyle);
                            CreateMergedCell(sheet, rowIndex, colIndex++, rowSpan, student.XJH, cellStyle);
                            CreateMergedCell(sheet, rowIndex, colIndex++, rowSpan, score.ToString("N0"), cellStyle);
                            if (levelModel.on)
                            {
                                var scoreAndLevel = getScoreAndLevel(levelModel, score);
                                CreateMergedCell(sheet, rowIndex, colIndex++, rowSpan, scoreAndLevel[0], cellStyle);
                                CreateMergedCell(sheet, rowIndex, colIndex++, rowSpan, scoreAndLevel[1], cellStyle);
                            }
                            
                            foreach (var system in evalutionSystem)
                            {
                                var thisColIndex = colIndex;
                                var systemRecords =
                                    system.EvaluationSystem1.Where(p => propertyIds.Contains(p.Id))
                                        .OrderBy(p => p.Order)
                                        .ToList();
                                if (systemRecords.Count > 0)
                                {
                                    CreateMergedCell(sheet, rowIndex, thisColIndex++, systemRecords.Count, system.Name, cellStyle);
                                    foreach (var record in systemRecords)
                                    {
                                        var scores = studentRecords.Where(p => p.PropertyId == record.Id).Sum(p => p.Score);
                                        CreateMergedCell(sheet, rowIndex, thisColIndex, 1, record.Name, cellStyle);
                                        CreateMergedCell(sheet, rowIndex, thisColIndex + 1, 1, scores.ToString("N0"), cellStyle);
                                        rowIndex++;
                                    }
                                }
                            }
                            if (rowSpan == 0)
                            {
                                rowIndex++;
                            }
                        }
                        for (int i = 0; i < colIndex+3; i++)
                        {
                            sheet.AutoSizeColumn(i);
                        }
                    }
                    var fileName = SaveHsskworkStream(hssfWorkbook);
                    return Json(new { flag = true, result = fileName }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new { flag = false, result = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string SaveHsskworkStream(HSSFWorkbook hssfWorkbook)
        {
            var tempFilePath = System.Configuration.ConfigurationManager.AppSettings["fileTempPath"];
            var fileName = DateTime.Now.Ticks + ".xls";
            var filePath = Path.Combine(tempFilePath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            FileStream fileStreamLocal = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            hssfWorkbook.Write(fileStreamLocal);
            fileStreamLocal.Close();
            return fileName;
        }

        private void CreateMergedCell(ISheet sheet,int rowIndex, int colIndex, int rowSpan, string value,ICellStyle style)
        {
            var row = sheet.GetRow(rowIndex)??sheet.CreateRow(rowIndex);
            var cell = row.CreateCell(colIndex);
            cell.SetCellValue(value);
            cell.CellStyle = style;
            if (rowSpan > 1)
            {
                CellRangeAddress address = new CellRangeAddress(rowIndex,rowIndex+rowSpan-1,colIndex,colIndex);
                sheet.AddMergedRegion(address);
            }
        }

        private string[] getScoreAndLevel(LevelModel levelModel, decimal score)
        {
            score = levelModel.TotalScore + score;
            var levelList = levelModel.LevelList.OrderByDescending(p => p.Score);
            foreach (var item in levelList)
            {
                if (score >= item.Score) return new[] { score.ToString("N0"), item.Name };
            }
            return new[] { score.ToString("N0"), levelModel.DefaultBottomLevel };
        }

        public ActionResult StatisticByClass(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }

        public JsonResult GetStatisticDataByClass(Guid projectId, string NJDM,DateTime beginTime,DateTime endTime)
        {
            try
            {
                beginTime = beginTime.Date;
                endTime = endTime.Date;
                using(EvaluationSystemEntities evaluationSystem = new EvaluationSystemEntities())
                using (StudentEntities studentEntities = new StudentEntities())
                {
                    var classesInfo =
                        studentEntities.Basic_ZZJX0202.Where(p => p.XXID == xxid && p.NJDM == NJDM)
                            .Select(p => p.BJID).ToList();

                    var classesInfo2 = studentEntities.Stu_ZZXS0101.Where(p => classesInfo.Contains(p.BJID.Value))
                        .GroupBy(p => p.BJID).Select(p => new {BJID = p.Key, StudentCount = p.Count()}).ToList();
                    var classesInfo3 = evaluationSystem.EvaluationSystemRecord.
                        Where(
                            p =>
                                classesInfo.Contains(p.ClassId) && p.ProjectId == projectId && p.HappenDate >= beginTime &&
                                p.HappenDate <= endTime).
                        GroupBy(p => p.ClassId).Select(p => new
                        {
                            BJID = p.Key,
                            Score = p.Sum(x => x.Score)
                        }).ToList();
                    var result = (from classInfo in classesInfo
                        join classStudent in classesInfo2 on classInfo equals classStudent.BJID into var1
                        join classScore in classesInfo3 on classInfo equals classScore.BJID into var2
                        from var3 in var1.DefaultIfEmpty()
                        from var4 in var2.DefaultIfEmpty()
                        select new
                        {
                            BJID = classInfo,
                            StudentCount = var3==null?0:var3.StudentCount,
                            Score = var4==null?0:var4.Score,
                            Average = var4 == null ? "0" : (var3 == null ? var4.Score.ToString("N0") : (var4.Score / var3.StudentCount).ToString("F"))
                        }).OrderByDescending(p=>p.Score).ToList();
                    return Json(new {flag = true, result}, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new {flag = false, message = e.Message}, JsonRequestBehavior.AllowGet);
            }
            
           
        }

        #endregion
    }
}