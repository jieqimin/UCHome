using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ServiceStack.Text;
using Student.Information.Service;
using Student.Model;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class ParentsCenterController : Controller
    {
        //
        // GET: /ParentsCenter/
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private static UserInfo user
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

        private Guid xxid
        {
            get
            {
                return user.xxid;
            }
        }
        private string usertype
        {
            get
            {
                return user.usertype;
            }
        }
        private string username
        {
            get
            {
                try
                {
                    return user.username;
                }
                catch (Exception)
                {

                    return "";
                }

            }
        }
        public ActionResult Index(Guid? id)
        {
            //根据家长ID获取学生ID
            if (id != null)
                return View(id.Value);
            UCHomeEntities uc = new UCHomeEntities();
            View_Rel_StuFamily sf = uc.View_Rel_StuFamily.FirstOrDefault(s => s.JZID == loginId);
            if (sf != null)
                return View(sf.XSID);
            return View(Guid.Empty);
        }

        public ActionResult HomePage(Guid id)
        {
            UCHomeBasePage ucHomeBasePage = new UCHomeBasePage();
            Guid xxid = ucHomeBasePage.GetUserInfoByRequestId(id).xxid;
            StudentInfoService stuService = new StudentInfoService();
            //德育信息
            IList<StudentInformation.DYInformation> stuDyList = stuService.GetStudentDY(id, null, "", 0).Take(5).ToList();
            ViewBag.DYInfo = stuDyList;

            //奖惩信息
            IList<StudentInformation.JCinformation> stuJcList = stuService.GetStudentJC(id, xxid, null, "").Take(5).ToList();
            ViewBag.JCInfo = stuJcList;

            //体检信息
            IList<StudentInformation.StudentHealth> stuHealthList = stuService.GetStudentHealth(id, null).Take(5).ToList();
            ViewBag.StudentHealth = stuHealthList;


            //成绩信息
            IList<StudentInformation.StudentScores> stuScoreList = stuService.GetStudentScore(id, xxid, null, null, "", "").Take(5).ToList();
            ViewBag.ScoreInfo = stuScoreList;
            return PartialView("HomePage", id);
        }
        public IList<StudentInformation.StudentScores> GetStudentScore(Guid userId, Guid schoolId, string year, string term, string ExamType, string ExamName)
        {
            StudentEntities db = new StudentEntities();
            Func<View_Exam_ExamScore, bool> predicate;
            Func<View_Exam_ExamScore, bool> func2;
            Func<View_Exam_ExamScore, bool> func3;
            Func<View_Exam_ExamScore, bool> func4;
            string sql =
    "select * from View_Exam_ExamScore where studentid='" + userId + "' and SchoolId = '" + schoolId + "'";
            List<View_Exam_ExamScore> source = db.ExecuteStoreQuery<View_Exam_ExamScore>(sql).ToList();
            //List<View_Exam_ExamScore> source =
            //    db.View_Exam_ExamScore.Where(x => x.StudentId == userId && x.SchoolId == schoolId).ToList();

            if (!string.IsNullOrEmpty(year))
            {
                predicate = x => x.SchYear == year;
                source = source.Where(predicate).ToList();
            }
            if (!string.IsNullOrEmpty(term))
            {
                func2 = x => x.SchTerm == term;
                source = source.Where(func2).ToList();
            }
            if (!string.IsNullOrEmpty(ExamType))
            {
                func3 = delegate(View_Exam_ExamScore x)
                {
                    int? examType = x.ExamType;
                    int num = Convert.ToInt32(ExamType);
                    return (examType.GetValueOrDefault() == num) && examType.HasValue;
                };
                source = source.Where(func3).ToList();
            }
            if (!string.IsNullOrEmpty(ExamName))
            {
                func4 = x => x.ExamName == ExamName;
                source = source.Where(func4).ToList();
            }
            IList<StudentInformation.StudentScores> list2 = new List<StudentInformation.StudentScores>();
            foreach (View_Exam_ExamScore score in source)
            {
                StudentInformation.StudentScores item = new StudentInformation.StudentScores
                {
                    SCID = score.ScoreId.ToString(),
                    ClassId = score.ClassId.ToString(),
                    ClassName = score.ClassName,
                    ExamID = score.ExamId,
                    Score = score.Score,
                    SchYear = score.SchYear,
                    SchTerm = score.SchTerm,
                    ExamName = score.ExamName,
                    Examlevel = score.ExamLevel.ToString(),
                    ExamTypeCode = score.ExamType,
                    ExamStartTime = score.StartDate,
                    ExamEndTime = score.EndDate
                };
                if (item.SchTerm.Equals("1"))
                {
                    item.SchTermName = "第一学期";
                    item.SchYearTerm = string.Concat(new object[] { Convert.ToInt32(score.SchYear) - 1, "-", score.SchYear, "学年第一学期" });
                }
                else
                {
                    item.SchTermName = "第二学期";
                    item.SchYearTerm = string.Concat(new object[] { Convert.ToInt32(score.SchYear) - 1, "-", score.SchYear, "学年第二学期" });
                }
                string str = item.ExamTypeCode.ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    string str2 = str;
                    if (str2 != "0")
                    {
                        if (str2 == "1")
                        {
                            goto Label_0403;
                        }
                        if (str2 == "2")
                        {
                            goto Label_040C;
                        }
                        if (str2 == "9")
                        {
                            goto Label_0415;
                        }
                    }
                    else
                    {
                        str = "平时测试";
                    }
                }
                goto Label_041F;
            Label_0403:
                str = "期中考试";
                goto Label_041F;
            Label_040C:
                str = "期末考试";
                goto Label_041F;
            Label_0415:
                str = "其他考试";
            Label_041F:
                item.ExamType = str;
                item.KCMC = score.KCMC;
                list2.Add(item);
            }
            //if (list2.Count == 0)
            //{
            //    list2.Add(new StudentInformation.StudentScores());
            //}
            return list2;
        }
        public List<ChildrenScores> GetStuScoreses()
        {
            UCHomeBasePage ucHomeBasePage = new UCHomeBasePage();
            if (Session["stuscore"] == null)
            {
                StudentInfoService stuService = new StudentInfoService();
                List<ChildrenScores> childrenscores = new List<ChildrenScores>();
                if (usertype.ToLower() == "s")
                {
                    IList<StudentInformation.StudentScores> stuScoreList = GetStudentScore(loginId, xxid, null, null, "", "").OrderBy(s => s.SchYear).ThenBy(s => s.SchTerm).ThenBy(s => s.ExamTypeCode).ToList();
                    List<StuScores> stuscoress = new List<StuScores>();
                    foreach (StudentInformation.StudentScores items in stuScoreList)
                    {
                        StuScores stuscores = OrderByScores(items);
                        stuscoress.Add(stuscores);
                    }
                    childrenscores.Add(new ChildrenScores
                    {
                        xsid = loginId,
                        XM = username,
                        stuscores = stuscoress
                    });
                }
                else if (usertype.ToLower() == "p")
                {
                    StudentEntities stu = new StudentEntities();
                    List<Stu_FamilyStuRel> family = stu.Stu_FamilyStuRel.Where(f => f.JZID == loginId).ToList();
                    foreach (Stu_FamilyStuRel item in family)
                    {
                        Guid xsid = item.XSID;
                        UserInfo userInfo = ucHomeBasePage.GetUserInfoByRequestId(xsid);
                        Guid schid = userInfo.xxid;
                        IList<StudentInformation.StudentScores> stuScoreList = GetStudentScore(xsid, schid, null, null, "", "").OrderBy(s => s.SchYear).ThenBy(s => s.SchTerm).ThenBy(s => s.ExamTypeCode).ToList();
                        List<StuScores> stuscoress = new List<StuScores>();
                        foreach (StudentInformation.StudentScores items in stuScoreList)
                        {
                            StuScores stuscores = OrderByScores(items);
                            stuscoress.Add(stuscores);
                        }
                        childrenscores.Add(new ChildrenScores
                        {
                            xsid = xsid,
                            XM = userInfo.username,
                            stuscores = stuscoress
                        });
                    }
                }
                Session["stuscore"] = childrenscores;
                return childrenscores;
            }
            return Session["stuscore"] as List<ChildrenScores>;
        }
        public ActionResult ScoreInfo(Guid? id)
        {
            Session["stuscore"] = null;
            Session.Remove("stuscore");
            //成绩信息
            //获取亲子信息
            ViewBag.ScoreInfo = GetStuScoreses();

            return PartialView("ScoreInfo");
        }

        public StuScores OrderByScores(StudentInformation.StudentScores items)
        {
            StuScores stuscores = new StuScores();
            switch (items.KCMC)
            {
                case "语文":
                    stuscores.stuscore = items;
                    stuscores.KCIndex = 1;
                    break;
                case "数学":
                    stuscores.stuscore = items;
                    stuscores.KCIndex = 2;
                    break;
                case "英语":
                    stuscores.stuscore = items;
                    stuscores.KCIndex = 3;
                    break;
                case "物理":
                    stuscores.stuscore = items;
                    stuscores.KCIndex = 4;
                    break;
                case "化学":
                    stuscores.stuscore = items;
                    stuscores.KCIndex = 5;
                    break;
                case "生物":
                    stuscores.stuscore = items;
                    stuscores.KCIndex = 6;
                    break;
                case "历史":
                    stuscores.stuscore = items;
                    stuscores.KCIndex = 7;
                    break;
                case "政治":
                    stuscores.stuscore = items;
                    stuscores.KCIndex = 8;
                    break;
                case "地理":
                    stuscores.stuscore = items;
                    stuscores.KCIndex = 9;
                    break;
                default:
                    stuscores.stuscore = items;
                    stuscores.KCIndex = 10;
                    break;
            }
            return stuscores;
        }
        public ActionResult ScoreTable(Guid? id)
        {
            return PartialView("ScoreTable", GetStuScoreses());
        }
        public ActionResult ScoreView(Guid? id)
        {
            Guid userid = id != null ? id.Value : loginId;
            ViewBag.XM = username;
            ViewBag.UserType = usertype;
            List<ChildrenScores> stuScoreList = GetStuScoreses();
            var firstOrDefault = stuScoreList.FirstOrDefault();
            if (firstOrDefault != null)
            {
                List<StuScores> scorelist = firstOrDefault.stuscores.DistinctBy(e => e.stuscore.ExamID).OrderByDescending(s => s.stuscore.SchYear).ThenByDescending(s => s.stuscore.SchTerm).ThenByDescending(s => s.stuscore.ExamTypeCode).ToList();
                return PartialView("ScoreView", scorelist);
            }
            return PartialView("ScoreView", null);
        }
        public ActionResult ScoreDetails(Guid id)
        {
            Guid userid = loginId;
            IList<ChildrenScores> stuScoreList = GetStuScoreses();
            var firstOrDefault = stuScoreList.FirstOrDefault();
            if (firstOrDefault != null)
            {
                List<StuScores> stuscorelist =
                    firstOrDefault.stuscores.Where(e => e.stuscore.ExamID == id).ToList();
                return PartialView("scoredetails", stuscorelist);
            }
            return PartialView("scoredetails", null);
        }
        public ActionResult ScoreCharts(Guid? id)
        {
            Guid userid = id != null ? id.Value : loginId;
            string split = string.Empty;
            string jsondata = string.Empty;
            //成绩信息
            IList<ChildrenScores> stuScoreList = GetStuScoreses();
            foreach (ChildrenScores childrenScorese in stuScoreList)
            {
                var firstOrDefault = childrenScorese;
                if (firstOrDefault != null)
                {
                    List<StuScores> Exams = firstOrDefault.stuscores.DistinctBy(e => e.stuscore.ExamID).OrderByDescending(s => s.stuscore.SchYear).ThenByDescending(s => s.stuscore.SchTerm).ThenByDescending(s => s.stuscore.ExamTypeCode).ToList();
                    foreach (StuScores exam in Exams)
                    {
                        List<StuScores> examlist =
                            firstOrDefault.stuscores.Where(e => e.stuscore.ExamID == exam.stuscore.ExamID).ToList();
                        foreach (StuScores items in examlist)
                        {
                            jsondata += string.Format("{3}{{examname:'{2}', kcmc:'{0}',score:'{1}'}}", childrenScorese.XM + ":" + items.stuscore.KCMC, items.stuscore.Score, exam.stuscore.ExamName, split);
                            split = ",";
                        }
                    }
                }
            }

            jsondata = "[" + jsondata + "]";
            return PartialView("ScoreCharts", jsondata);
        }
        public ActionResult HealthInfo(Guid? id)
        {
            Guid userid = id != null ? id.Value : loginId;
            StudentInfoService stuService = new StudentInfoService();
            //体检信息
            IList<StudentInformation.StudentHealth> stuHealthList = stuService.GetStudentHealth(userid, null).ToList();
            ViewBag.StudentHealth = stuHealthList;
            return PartialView("HealthInfo", userid);
        }
        public ActionResult JCInfo(Guid? id)
        {
            Guid userid = id != null ? id.Value : loginId;
            UCHomeBasePage ucHomeBasePage = new UCHomeBasePage();
            Guid xxid = ucHomeBasePage.GetUserInfoByRequestId(userid).xxid;
            StudentInfoService stuService = new StudentInfoService();

            //奖惩信息
            IList<StudentInformation.JCinformation> stuJcList = stuService.GetStudentJC(userid, xxid, null, "").ToList();
            ViewBag.JCInfo = stuJcList;

            return PartialView("JCInfo", userid);
        }
        public ActionResult DYInfo(Guid? id)
        {
            Guid userid = id != null ? id.Value : loginId;
            StudentInfoService stuService = new StudentInfoService();
            //德育信息
            IList<StudentInformation.DYInformation> stuDyList = stuService.GetStudentDY(userid, null, "", 0).ToList();
            ViewBag.DYInfo = stuDyList;

            return PartialView("DYInfo", userid);
        }
        public ActionResult PersonInfo(Guid? id)
        {
            Guid userid = id != null ? id.Value : loginId;
            UCHomeEntities uc = new UCHomeEntities();
            View_Simple_Parents fam = uc.View_Simple_Parents.FirstOrDefault(f => f.XSID == userid);
            return PartialView(fam);
        }
        public ActionResult TeacherList()
        {
            return PartialView();
        }
        public ActionResult OtherParents()
        {
            return PartialView();
        }
        public ActionResult Chat()
        {
            return View();
        }
        public ActionResult PChat()
        {
            return View();
        }

        public ActionResult GetPayInfo(string id)
        {
            return PartialView("PayInfo", loginId);
        }

    }

    public partial class StuScores
    {
        public int KCIndex { get; set; }
        public StudentInformation.StudentScores stuscore { get; set; }
    }
    public partial class ChildrenScores
    {
        public Guid xsid { get; set; }
        public string XM { get; set; }
        public List<StuScores> stuscores { get; set; }
    }
}
