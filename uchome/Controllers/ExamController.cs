using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Exam.BLL;
using Exam.Model;
using Exam.Model.ViewModel;
using OA.BLL;
using OA.Model.ViewModel;
using TeacherAwards.BLL.Public;
using TeacherAwards.BLL.TeacherAward;
using TeacherAwards.Model.Entity;
using TeacherAwards.Model.QueryModel;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class ExamController : Controller
    {
        private readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private readonly SysDictHelper _sysDictHelper = new SysDictHelper();
        private readonly EntityHelper _entityHelper = new EntityHelper();
        private readonly ExamService _examService = new ExamService();

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
        #region 班级学生成绩页面HTML
        /// <summary>
        /// 班级学生成绩页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult ClassStudentExamScore()
        {
            BindDropByDownLoad();
            return View(new OriginalScoreResponseViewModel());
        }

        #endregion

        #region 班级学生成绩提交页面Html
        /// <summary>
        /// 班级学生成绩提交页面Html
        /// </summary>
        /// <returns></returns>
        public ActionResult OriginalScore()
        {
            var studentScoreViewModel = new StudentScoreViewModel();
            var originalScoreResponseViewModel = new OriginalScoreResponseViewModel();

            var yearTerm = Request["SchYearTerm"];
            var examId = Request["SchExam"];
            var gradeCode = Request["SchGrade"];
            var classCode = Request["SchClass"];
            var subjectName = Request["SchSubject"];
            var kldm = Request["SchKLDM"];

            SqlParameter[] para = new SqlParameter[5];
            para[0] = new SqlParameter("@ExamId", new Guid(examId));
            para[1] = new SqlParameter("@SchId", schoolId);
            para[2] = new SqlParameter("@ExamNJ", gradeCode);
            para[3] = new SqlParameter("@ExamKL", kldm);
            para[4] = new SqlParameter("@ClassCode", classCode);

            var studentScoreByPrimary =
                _examService.GetStudentScoreByPrimary(para).Where(w => w.SubjectName == subjectName).ToList();

            if (studentScoreByPrimary.Any())
            {
                #region 数据处理

                originalScoreResponseViewModel.OriginalScoreList = studentScoreByPrimary;

                var studentList = studentScoreByPrimary.Select(s => new StudentScoreViewModel
                {
                    StudentId = s.XSID,
                    SchoolName = s.XXMC,
                    GradeCode = s.NJDM,
                    GradeName = s.NJMC,
                    ClassName = s.BJMC,
                    StudentName = s.XSXM,
                    StuClassCode = s.KLDM,
                    StudentCardNumber = s.SFZHM,
                    StuSchoolNumber = s.KSH
                }).GroupBy(g => g.StudentCardNumber).Select(s => s.First()).ToList();

                originalScoreResponseViewModel.StudentList = studentList;

                var subjectList =
                    _examService.GetSubjectByExamIdList(new Guid(examId)).Where(w => w.SubjectName == subjectName)
                        .Select(
                            s => new View_Exam_ExamSubject { SubjectCode = s.SubjectCode, SubjectName = s.SubjectName })
                        .GroupBy(g => g.SubjectCode).Select(s => s.First()).OrderBy(o => o.SubjectCode).ToList();

                originalScoreResponseViewModel.SubjectList = subjectList;

                #endregion

                #region 生成图表数据

                try
                {
                    List<Series> serList = new List<Series>();

                    string[] studentNames = new string[studentList.Count];

                    #region 按学科生成图表

                    Series s = new Series();
                    s.id = 1;
                    s.type = "column";
                    s.name = subjectName;

                    decimal[] d = new decimal[studentList.Count];

                    List<StudentRankViewModel> rankList = new List<StudentRankViewModel>();

                    for (int j = 0; j < studentList.Count; j++)
                    {
                        var stuScore = studentScoreByPrimary.Where(
                            w =>
                                w.SFZHM == studentList[j].StudentCardNumber && w.SubjectName == subjectName).ToList();

                        var rank = new StudentRankViewModel();

                        var studentName = studentList[j].StudentName;

                        var ai = 1;
                        while (studentNames.Contains(studentList[j].StudentName))
                        {
                            studentName = studentList[j].StudentName + "_" + ai++;
                        }

                        rank.StudentName = studentName;
                        studentNames[j] = studentName;

                        var examScore = stuScore.FirstOrDefault(w => w.RankType == "班名次");

                        if (examScore != null)
                        {
                            rank.ClassRank = examScore.ScoreRank;
                            d[j] = examScore.Score;
                        }
                        else
                        {
                            rank.ClassRank = 0;
                            d[j] = 0;
                        }

                        var schoolRank = stuScore.FirstOrDefault(w => w.RankType == "校名次");

                        if (schoolRank != null)
                            rank.SchoolRank = schoolRank.ScoreRank;
                        else
                            rank.SchoolRank = 0;

                        var areaRank = stuScore.FirstOrDefault(w => w.RankType == "区名次");

                        if (areaRank != null)
                            rank.AreaRank = areaRank.ScoreRank;
                        else
                            rank.AreaRank = 0;

                        rankList.Add(rank);
                    }

                    s.Score = d;

                    serList.Add(s);

                    studentScoreViewModel.StudentNames = studentNames;
                    studentScoreViewModel.SeriesList = serList;
                    studentScoreViewModel.StuRankList = rankList;

                    #endregion

                }
                catch (Exception ex)
                {

                }

                #endregion
            }

            //页面图形数据
            ViewBag.StudentScoreViewModel = NewtonsoftJson(studentScoreViewModel);

            return View("OriginalScore", originalScoreResponseViewModel);
        }

        #endregion

        #region 获取学生年度成绩详情Html
        /// <summary>
        /// 获取学生年度成绩详情Html
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentYearScoreDetails()
        {
            var studentId = Request["StudentId"];
            var gradeCode = Request["GradeCode"];
            var classTypeCode = Request["ClassTypeCode"];
            var yearTerm = Request["YearTerm"];
            var subjectName = Request["SubjectName"];

            var originalScoreResponseViewModel = new OriginalScoreResponseViewModel();
            var studentScoreViewModel = new StudentScoreViewModel();

            var year = yearTerm.Substring(0, 4) + "-" + (Convert.ToInt32(yearTerm.Substring(0, 4)) + 1);
            var term = yearTerm.Substring(4, 1);

            switch (classTypeCode)
            {
                case "不分科":
                    classTypeCode = "0";
                    break;
                case "文科":
                    classTypeCode = "1";
                    break;
                case "理科":
                    classTypeCode = "2";
                    break;
            }

            SqlParameter[] para = new SqlParameter[6];
            para[0] = new SqlParameter("@SchId", schoolId);
            para[1] = new SqlParameter("@StudentId", studentId);
            para[2] = new SqlParameter("@GradeCode", gradeCode);
            para[3] = new SqlParameter("@ClassTypeCode", classTypeCode);
            para[4] = new SqlParameter("@Year", year);
            para[5] = new SqlParameter("@Term", term);

            var studentScoreByPrimary =
                _examService.GetStudentYearScoreByPrimary(para).Where(w => w.SubjectName == subjectName).ToList();

            if (studentScoreByPrimary.Any())
            {
                #region 数据处理

                originalScoreResponseViewModel.OriginalScoreList = studentScoreByPrimary;

                var examList =
                    studentScoreByPrimary.Select(s => new ExamViewModel { ExamId = s.ExamId, ExamName = s.ExamName })
                        .GroupBy(g => g.ExamId)
                        .Select(s => s.First())
                        .ToList();

                originalScoreResponseViewModel.ExamList = examList;

                #endregion

                #region 生成图表数据

                List<Series> serList = new List<Series>();

                string[] examNames = new string[examList.Count];

                Series s1 = new Series();
                s1.id = 1;
                s1.type = "column";
                s1.name = subjectName;

                decimal[] d = new decimal[examList.Count];

                List<StudentRankViewModel> rankList = new List<StudentRankViewModel>();

                for (int j = 0; j < examList.Count; j++)
                {
                    var stuScore = studentScoreByPrimary.Where(
                        w =>
                            w.ExamName == examList[j].ExamName).ToList();

                    var rank = new StudentRankViewModel();

                    var examName = examList[j].ExamName;

                    var ai = 1;
                    while (examNames.Contains(examList[j].ExamName))
                    {
                        examName = examList[j].ExamName + "_" + ai++;
                    }

                    rank.StudentName = examName;
                    examNames[j] = examName;

                    var examScore = stuScore.FirstOrDefault(w => w.RankType == "班名次");

                    if (examScore != null)
                    {
                        rank.ClassRank = examScore.ScoreRank;
                        d[j] = examScore.Score;
                    }
                    else
                    {
                        rank.ClassRank = 0;
                        d[j] = 0;
                    }

                    var schoolRank = stuScore.FirstOrDefault(w => w.RankType == "校名次");

                    if (schoolRank != null)
                        rank.SchoolRank = schoolRank.ScoreRank;
                    else
                        rank.SchoolRank = 0;

                    var areaRank = stuScore.FirstOrDefault(w => w.RankType == "区名次");

                    if (areaRank != null)
                        rank.AreaRank = areaRank.ScoreRank;
                    else
                        rank.AreaRank = 0;

                    rankList.Add(rank);
                }

                s1.Score = d;

                serList.Add(s1);

                studentScoreViewModel.StudentNames = examNames;
                studentScoreViewModel.SeriesList = serList;
                studentScoreViewModel.StuRankList = rankList;

                #endregion
            }

            ViewBag.StudentScoreViewModel = NewtonsoftJson(studentScoreViewModel);

            return View(originalScoreResponseViewModel);
        }

        #endregion

        #region 班级学生成绩页面(班主任)HTML
        /// <summary>
        /// 班级学生成绩页面(班主任)HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult ClassStudentExamAllScore()
        {
            BindDropByDownLoadByBZR();
            return View();
        }

        #endregion

        #region 班级学生成绩提交页面(班主任)Html
        /// <summary>
        /// 班级学生成绩提交页面Html
        /// </summary>
        /// <returns></returns>
        public ActionResult OriginalAllScore()
        {
            var studentScoreViewModel = new StudentScoreViewModel();
            var originalScoreResponseViewModel = new OriginalScoreResponseViewModel();

            var yearTerm = Request["SchYearTerm"];
            var examId = Request["SchExam"];
            var gradeCode = Request["SchGrade"];
            var classCode = Request["SchClass"];
            var kldm = Request["SchKLDM"];

            SqlParameter[] para = new SqlParameter[5];
            para[0] = new SqlParameter("@ExamId", new Guid(examId));
            para[1] = new SqlParameter("@SchId", schoolId);
            para[2] = new SqlParameter("@ExamNJ", gradeCode);
            para[3] = new SqlParameter("@ExamKL", kldm);
            para[4] = new SqlParameter("@ClassCode", classCode);

            var studentScoreByPrimary =
                _examService.GetStudentScoreByPrimary(para);

            if (studentScoreByPrimary.Any())
            {
                #region 数据处理

                originalScoreResponseViewModel.OriginalScoreList = studentScoreByPrimary;

                var studentList = studentScoreByPrimary.Select(s => new StudentScoreViewModel
                {
                    StudentId = s.XSID,
                    SchoolName = s.XXMC,
                    GradeCode = s.NJDM,
                    GradeName = s.NJMC,
                    ClassName = s.BJMC,
                    StudentName = s.XSXM,
                    StuClassCode = s.KLDM,
                    StudentCardNumber = s.SFZHM,
                    StuSchoolNumber = s.KSH
                }).GroupBy(g => g.StudentCardNumber).Select(s => s.First()).ToList();

                originalScoreResponseViewModel.StudentList = studentList;

                var subjectList =
                    _examService.GetSubjectByExamIdList(new Guid(examId))
                        .Select(
                            s => new View_Exam_ExamSubject { SubjectCode = s.SubjectCode, SubjectName = s.SubjectName })
                        .GroupBy(g => g.SubjectCode).Select(s => s.First()).OrderBy(o => o.SubjectCode).ToList();

                subjectList.Add(new View_Exam_ExamSubject { SubjectName = "总分" });

                #endregion

                #region 生成图表数据
                try
                {
                    List<Series> serList = new List<Series>();

                    string[] studentNames = new string[studentList.Count];

                    for (int j = 0; j < studentList.Count; j++)
                    {
                        var studentName = studentList[j].StudentName;

                        var ai = 1;
                        while (studentNames.Contains(studentList[j].StudentName))
                        {
                            studentName = studentList[j].StudentName + "_" + ai++;
                        }

                        studentNames[j] = studentName;
                    }

                    #region 按学科生成图表

                    List<StudentRankViewModel> rankList = new List<StudentRankViewModel>();

                    for (int i = 0; i < subjectList.Count; i++)
                    {
                        Series s = new Series();
                        s.id = i + 1;
                        s.type = "column";
                        s.name = subjectList[i].SubjectName;

                        decimal[] d = new decimal[studentList.Count];

                        var names = new string[studentList.Count];
                        for (int j = 0; j < studentList.Count; j++)
                        {
                            var stuScore = studentScoreByPrimary.Where(
                                w =>
                                    w.SFZHM == studentList[j].StudentCardNumber &&
                                    w.SubjectName == subjectList[i].SubjectName).ToList();

                            if (stuScore.Any())
                            {
                                var rank = new StudentRankViewModel();

                                var studentName = studentList[j].StudentName;

                                var ai = 1;
                                while (names.Contains(studentList[j].StudentName))
                                {
                                    studentName = studentList[j].StudentName + "_" + ai++;
                                }

                                rank.SubjectName = subjectList[i].SubjectName;
                                rank.StudentName = studentName;
                                names[j] = studentName;

                                var examScore = stuScore.FirstOrDefault(w => w.RankType == "班名次");

                                if (examScore != null)
                                {
                                    rank.ClassRank = examScore.ScoreRank;
                                    d[j] = examScore.Score;
                                }
                                else
                                {
                                    rank.ClassRank = 0;
                                    d[j] = 0;
                                }

                                var schoolRank = stuScore.FirstOrDefault(w => w.RankType == "校名次");

                                if (schoolRank != null)
                                    rank.SchoolRank = schoolRank.ScoreRank;
                                else
                                    rank.SchoolRank = 0;

                                var areaRank = stuScore.FirstOrDefault(w => w.RankType == "区名次");

                                if (areaRank != null)
                                    rank.AreaRank = areaRank.ScoreRank;
                                else
                                    rank.AreaRank = 0;

                                rankList.Add(rank);
                            }
                        }
                        s.Score = d;
                        serList.Add(s);
                    }

                    studentScoreViewModel.StudentNames = studentNames;
                    studentScoreViewModel.SeriesList = serList;
                    studentScoreViewModel.StuRankList = rankList;

                    #endregion
                }
                catch (Exception ex)
                {

                }

                #endregion

                subjectList.Add(new View_Exam_ExamSubject { SubjectName = "参考总分" });

                originalScoreResponseViewModel.SubjectList = subjectList;
            }

            ViewBag.StudentScoreViewModel = NewtonsoftJson(studentScoreViewModel);

            return View(originalScoreResponseViewModel);
        }

        #endregion

        #region 获取学生年度成绩详情(班主任)Html
        /// <summary>
        /// 获取学生年度成绩详情Html
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentYearAllScoreDetails()
        {
            var examId = Request["ExamId"];
            var studentId = Request["StudentId"];
            var gradeCode = Request["GradeCode"];
            var classTypeCode = Request["ClassTypeCode"];
            var yearTerm = Request["YearTerm"];

            var originalScoreResponseViewModel = new OriginalScoreResponseViewModel();
            var studentScoreViewModel = new StudentScoreViewModel();
            List<StudentRankViewModel> rankList = new List<StudentRankViewModel>();

            var year = yearTerm.Substring(0, 4) + "-" + (Convert.ToInt32(yearTerm.Substring(0, 4)) + 1);
            var term = yearTerm.Substring(4, 1);

            switch (classTypeCode)
            {
                case "不分科":
                    classTypeCode = "0";
                    break;
                case "文科":
                    classTypeCode = "1";
                    break;
                case "理科":
                    classTypeCode = "2";
                    break;
            }

            SqlParameter[] para = new SqlParameter[6];
            para[0] = new SqlParameter("@SchId", schoolId);
            para[1] = new SqlParameter("@StudentId", studentId);
            para[2] = new SqlParameter("@GradeCode", gradeCode);
            para[3] = new SqlParameter("@ClassTypeCode", classTypeCode);
            para[4] = new SqlParameter("@Year", year);
            para[5] = new SqlParameter("@Term", term);

            var studentScoreByPrimary =
                _examService.GetStudentYearScoreByPrimary(para).ToList();

            if (studentScoreByPrimary.Any())
            {
                #region 数据处理

                originalScoreResponseViewModel.OriginalScoreList = studentScoreByPrimary;

                var examList =
                    studentScoreByPrimary.Select(s => new ExamViewModel { ExamId = s.ExamId, ExamName = s.ExamName })
                        .GroupBy(g => g.ExamId)
                        .Select(s => s.First())
                        .ToList();

                originalScoreResponseViewModel.ExamList = examList;

                var examIdList = examList.Select(s => s.ExamId).ToList();

                var subjectList =
                    _examService.GetSubjectByExamIdsList(examIdList)
                        .Select(
                            s => new View_Exam_ExamSubject { SubjectCode = s.SubjectCode, SubjectName = s.SubjectName })
                        .GroupBy(g => g.SubjectCode).Select(s => s.First()).OrderBy(o => o.SubjectCode).ToList();

                subjectList.Add(new View_Exam_ExamSubject { SubjectName = "总分" });

                #endregion

                #region 生成图表数据

                List<Series> serList = new List<Series>();

                string[] examNames = new string[examList.Count];

                for (int i = 0; i < examList.Count; i++)
                {
                    var name = examList[i].ExamName;

                    var ai = 1;
                    while (examNames.Contains(examList[i].ExamName))
                    {
                        name = examList[i].ExamName + "_" + ai++;
                    }
                    examNames[i] = name;
                }

                for (int i = 0; i < subjectList.Count; i++)
                {
                    Series s1 = new Series();
                    s1.id = 1;
                    s1.type = "column";
                    s1.name = subjectList[i].SubjectName;

                    decimal[] d = new decimal[examList.Count];

                    var names = new string[examList.Count];
                    for (int j = 0; j < examList.Count; j++)
                    {
                        var stuScore = studentScoreByPrimary.Where(
                            w =>
                                w.ExamName == examList[j].ExamName && w.SubjectName == subjectList[i].SubjectName).ToList();

                        if (stuScore.Any())
                        {
                            var rank = new StudentRankViewModel();

                            var examName = examList[j].ExamName;

                            var ai = 1;
                            while (names.Contains(examList[j].ExamName))
                            {
                                examName = examList[j].ExamName + "_" + ai++;
                            }
                            rank.SubjectName = subjectList[i].SubjectName;
                            rank.StudentName = examName;
                            names[j] = examName;

                            var examScore = stuScore.FirstOrDefault(w => w.RankType == "班名次");

                            if (examScore != null)
                            {
                                rank.ClassRank = examScore.ScoreRank;
                                d[j] = examScore.Score;
                            }
                            else
                            {
                                rank.ClassRank = 0;
                                d[j] = 0;
                            }

                            var schoolRank = stuScore.FirstOrDefault(w => w.RankType == "校名次");

                            if (schoolRank != null)
                                rank.SchoolRank = schoolRank.ScoreRank;
                            else
                                rank.SchoolRank = 0;

                            var areaRank = stuScore.FirstOrDefault(w => w.RankType == "区名次");

                            if (areaRank != null)
                                rank.AreaRank = areaRank.ScoreRank;
                            else
                                rank.AreaRank = 0;

                            rankList.Add(rank);
                        }
                    }

                    s1.Score = d;

                    serList.Add(s1);
                }

                studentScoreViewModel.StudentNames = examNames;
                studentScoreViewModel.SeriesList = serList;
                studentScoreViewModel.StuRankList = rankList;

                #endregion

                subjectList.Add(new View_Exam_ExamSubject { SubjectName = "参考总分" });

                originalScoreResponseViewModel.SubjectList = subjectList;
            }

            ViewBag.StudentScoreViewModel = NewtonsoftJson(studentScoreViewModel);

            return View(originalScoreResponseViewModel);
        }

        #endregion

        #region 根据学年学期获取本校和区考试信息Json
        /// <summary>
        /// 根据学年学期获取本校和区考试信息Json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllExamListJson(string yearTerm, string examId = "")
        {
            var year = yearTerm.Substring(0, 4) + "-" + (Convert.ToInt32(yearTerm.Substring(0, 4)) + 1);
            var term = yearTerm.Substring(4, 1);

            return Json(_examService.GetAllExamList(schoolId, year, term, examId), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 根据学年学期年级获取教师任教班级信息Json

        /// <summary>
        /// 根据学年学期年级获取教师任教班级信息Json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetClassListJson(string yearTerm, string gradeCode)
        {
            var teachList =
                _examService.GetTeacherTeachScheduleList(yearTerm, loginId, gradeCode)
                    .GroupBy(g => g.XZBDM).Select(s => s.First()).Select(d => new SelectListItem()
                    {
                        Text = d.XZBMC,
                        Value = d.XZBDM
                    }).ToList();

            return Json(teachList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 根据学年学期获取教师任教年级信息Json
        /// <summary>
        /// 根据学年学期获取教师任教年级信息Json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetGradeListJson(string yearTerm)
        {
            var teachGradeList =
                _examService.GetTeacherTeachScheduleList(yearTerm, loginId)
                    .GroupBy(g => g.NJDM)
                    .Select(s => s.First())
                    .Select(d => new SelectListItem()
                    {
                        Text = d.NJMC,
                        Value = d.NJDM
                    }).ToList();

            return Json(teachGradeList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 根据学年学期获取教师任教学科信息Json
        /// <summary>
        /// 根据学年学期获取教师任教学科信息Json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSubjectListJson(string yearTerm)
        {
            var teachSubjectList =
                _examService.GetTeacherTeachScheduleList(yearTerm, loginId)
                    .GroupBy(g => g.KCMC)
                    .Select(s => s.First())
                    .Select(d => new SelectListItem()
                    {
                        Text = d.KCMC,
                        Value = d.KCMC
                    }).ToList();

            return Json(teachSubjectList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 根据年级获取班主任班级信息Json

        /// <summary>
        /// 根据年级获取班主任班级信息Json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetClassHeadMasterList(string gradeCode)
        {
            var teachList =
                _examService.GetClassHeadMasterList(loginId, gradeCode)
                    .GroupBy(g => g.XZBDM).Select(s => s.First()).Select(d => new SelectListItem()
                    {
                        Text = d.XZBMC,
                        Value = d.XZBDM
                    }).ToList();

            return Json(teachList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 根据考试获取参加考试的学科信息Json
        /// <summary>
        /// 根据考试获取参加考试的学科信息Json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSubjectListByExamIdJson(Guid examId)
        {
            var teachSubjectList =
                _examService.GetSubjectByExamIdList(examId)
                    .GroupBy(g => g.SubjectCode)
                    .Select(s => s.First())
                    .Select(d => new SelectListItem()
                    {
                        Text = d.SubjectName,
                        Value = d.SubjectCode
                    }).ToList();

            return Json(teachSubjectList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 下拉框绑定
        /// <summary>
        /// 下拉框绑定
        /// </summary>
        /// <param name="yearTerm">学年学期</param>
        /// <param name="examId">考试ID</param>
        /// <param name="gradeCode">年级编号</param>
        /// <param name="classCode">班级编码</param>
        /// <param name="subjectName">科目名称</param>
        /// <param name="kldm">科类代码</param>
        public void BindDropByDownLoad(string yearTerm = "", string examId = "", string gradeCode = "", string classCode = "", string subjectName = "", string kldm = "")
        {
            if (string.IsNullOrEmpty(yearTerm))
            {
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
            }

            var xnxqList = _sysDictHelper.GetXNXQList(yearTerm);

            ViewBag.SchYearTerm = xnxqList;

            if (yearTerm != "" && yearTerm.Length == 5)
            {
                var year = yearTerm.Substring(0, 4) + "-" + (Convert.ToInt32(yearTerm.Substring(0, 4)) + 1);
                var term = yearTerm.Substring(4, 1);
                ViewBag.SchExam = _examService.GetAllExamList(schoolId, year, term, examId);

                //根据当前教师加载教师所任教的年级班级学科
                var teacherTechList = _examService.GetTeacherTeachScheduleList(yearTerm, loginId);

                if (teacherTechList.Any())
                {
                    //任教年级
                    ViewBag.SchGrade =
                        teacherTechList.GroupBy(g => g.NJDM).Select(s => s.First()).Select(d => new SelectListItem()
                        {
                            Text = d.NJMC,
                            Value = d.NJDM,
                            Selected = d.NJDM == gradeCode
                        }).ToList();

                    if (string.IsNullOrEmpty(gradeCode))
                    {
                        gradeCode = teacherTechList.First().NJDM;
                    }

                    //任教班级
                    ViewBag.SchClass = teacherTechList.Where(w => w.NJDM == gradeCode).GroupBy(g => g.XZBDM).Select(s => s.First()).Select(d => new SelectListItem()
                    {
                        Text = d.XZBMC,
                        Value = d.XZBDM,
                        Selected = d.XZBDM == classCode
                    }).ToList();

                    //任教学科
                    ViewBag.SchSubject = teacherTechList.GroupBy(g => g.KCMC).Select(s => s.First()).Select(d => new SelectListItem()
                    {
                        Text = d.KCMC,
                        Value = d.KCMC,
                        Selected = d.KCMC == subjectName
                    }).ToList();
                }
                else
                {
                    ViewBag.SchGrade = new List<SelectListItem>();
                    ViewBag.SchClass = new List<SelectListItem>();
                    ViewBag.SchSubject = new List<SelectListItem>();
                }
            }
            else
            {
                ViewBag.SchExam = new List<SelectListItem>();
                ViewBag.SchGrade = new List<SelectListItem>();
                ViewBag.SchClass = new List<SelectListItem>();
                ViewBag.SchSubject = new List<SelectListItem>();
            }

            ViewBag.SchKLDM = _sysDictHelper.GetKLDMList(kldm);
        }
        #endregion

        #region 下拉框绑定（班主任）
        /// <summary>
        /// 下拉框绑定（班主任）
        /// </summary>
        /// <param name="yearTerm">学年学期</param>
        /// <param name="examId">考试ID</param>
        /// <param name="gradeCode">年级编号</param>
        /// <param name="classCode">班级编码</param>
        /// <param name="kldm">科类代码</param>
        public void BindDropByDownLoadByBZR(string yearTerm = "", string examId = "", string gradeCode = "", string classCode = "", string kldm = "")
        {
            if (string.IsNullOrEmpty(yearTerm))
            {
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
            }

            var xnxqList = _sysDictHelper.GetXNXQList(yearTerm);

            ViewBag.SchYearTerm = xnxqList;

            if (yearTerm != "" && yearTerm.Length == 5)
            {
                var year = yearTerm.Substring(0, 4) + "-" + (Convert.ToInt32(yearTerm.Substring(0, 4)) + 1);
                var term = yearTerm.Substring(4, 1);
                ViewBag.SchExam = _examService.GetAllExamList(schoolId, year, term, examId);
            }
            else
            {
                ViewBag.SchExam = new List<SelectListItem>();
                ViewBag.SchGrade = new List<SelectListItem>();
                ViewBag.SchClass = new List<SelectListItem>();
            }

            //根据当前教师加载教师所任教的年级班级学科
            var headMasterList = _examService.GetClassHeadMasterList(loginId);

            if (headMasterList.Any())
            {
                //任教年级
                ViewBag.SchGrade =
                    headMasterList.GroupBy(g => g.NJDM).Select(s => s.First()).Select(d => new SelectListItem()
                    {
                        Text = d.NJMC,
                        Value = d.NJDM,
                        Selected = d.NJDM == gradeCode
                    }).ToList();

                if (string.IsNullOrEmpty(gradeCode))
                {
                    gradeCode = headMasterList.First().NJDM;
                }

                //任教班级
                ViewBag.SchClass = headMasterList.Where(w => w.NJDM == gradeCode).GroupBy(g => g.XZBDM).Select(s => s.First()).Select(d => new SelectListItem()
                {
                    Text = d.XZBMC,
                    Value = d.XZBDM,
                    Selected = d.XZBDM == classCode
                }).ToList();
            }
            else
            {
                ViewBag.SchGrade = new List<SelectListItem>();
                ViewBag.SchClass = new List<SelectListItem>();
            }

            ViewBag.SchKLDM = _sysDictHelper.GetKLDMList(kldm);
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