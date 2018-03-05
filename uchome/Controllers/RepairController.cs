using OA.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using OA.Model;
using OA.Model.ViewModel;
using UCHome.Model;
using System.Configuration;

namespace UCHome.Controllers
{
    public class RepairController : Controller
    {
        Repair rp = new Repair();
        OABll oa = new OABll();
        ZZ_MIFVSEntities entity = new ZZ_MIFVSEntities();

        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        public Guid userid
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
            get { return user.xxid; }
        }
        private string loginName
        {
            get { return user.loginname; }
        }
        private string names
        {
            get { return user.username; }
        }
        //public JsonResult GetRepairTypeList()
        //{
        //    List<SelectListItem> _list = new List<SelectListItem>();
        //    _list.Add(new SelectListItem { Value = "", Text = "请选择" });
        //    _list.Add(new SelectListItem { Value = "电教报修", Text = "电教报修" });
        //    _list.Add(new SelectListItem { Value = "后勤报修", Text = "后勤报修" });
        //    return Json(_list, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetRepairTypeList()
        {
            IEnumerable<SelectListItem> list = rp.GetRepairTypeList().Select(p => new SelectListItem()
            {
                Text = p.CodeName,
                Value = p.CodeName
            });

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetReairProjectList(string dlid)
        //{
        //    List<SelectListItem> _listbj = new List<SelectListItem>();
        //    if (dlid == "电教报修")
        //    {
        //        _listbj.Add(new SelectListItem { Value = "教室>投影及相关设备", Text = "教室>投影及相关设备" });
        //        _listbj.Add(new SelectListItem { Value = "教室>操作系统", Text = "教室>操作系统" });
        //        _listbj.Add(new SelectListItem { Value = "教室>软件", Text = "教室>软件" });
        //        _listbj.Add(new SelectListItem { Value = "教室>计算机硬件", Text = "教室>计算机硬件" });
        //        _listbj.Add(new SelectListItem { Value = "教室>其他", Text = "教室>其他" });
        //        _listbj.Add(new SelectListItem { Value = "办公室>操作系统", Text = "办公室>操作系统" });
        //        _listbj.Add(new SelectListItem { Value = "办公室>软件", Text = "办公室>软件" });
        //        _listbj.Add(new SelectListItem { Value = "办公室>计算机硬件", Text = "办公室>计算机硬件" });
        //        _listbj.Add(new SelectListItem { Value = "办公室>其他", Text = "办公室>其他" });
        //    }
        //    else if (dlid == "后勤报修")
        //    {
        //        _listbj.Add(new SelectListItem { Value = "供水设备", Text = "供水设备" });
        //        _listbj.Add(new SelectListItem { Value = "供电设备", Text = "供电设备" });
        //        _listbj.Add(new SelectListItem { Value = "供暖设备", Text = "供暖设备" });
        //        _listbj.Add(new SelectListItem { Value = "门窗设施", Text = "门窗设施" });
        //        _listbj.Add(new SelectListItem { Value = "消防设备", Text = "消防设备" });
        //        _listbj.Add(new SelectListItem { Value = "空调设备", Text = "空调设备" });
        //        _listbj.Add(new SelectListItem { Value = "风扇设备", Text = "风扇设备" });
        //        _listbj.Add(new SelectListItem { Value = "照明设备", Text = "照明设备" });
        //        _listbj.Add(new SelectListItem { Value = "其他", Text = "其他" });
        //    }
        //    return Json(_listbj, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetReairProjectList(string dlid)
        {
            IEnumerable<SelectListItem> list = rp.GetRepair_DistictList(xxid, dlid).Select(p => new SelectListItem()
            {
                Text = p.RepairProjectName,
                Value = p.RepairProjectName
            });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var list = rp.GetMyRepairList(userid);
            return View(list);
        }
        public ActionResult IndexAudit()
        {
            var list = rp.GetMyAuditRepairList(userid);
            ViewBag.loginid = userid.ToString();
            return View(list);
        }
        public ActionResult Create()
        {
            View_Repair al = new View_Repair();
            al.REID = Guid.NewGuid();
            return View(al);
        }
        public ActionResult Edit()
        {
            Guid iD = Guid.Parse(Request["REID"]);
            var info = rp.GetRepairAduitPeople(iD);
            var model = rp.GetReapirEdit(iD);
            ViewBag.approver = info.QTBX;
            ViewBag.RepairType = info.DongLou;
            ViewBag.RepairProjectType = info.BaoXiuProject;
            //ViewBag.approver = info.BaoXiuProject;
            return View(model);
        }
        public ActionResult Details()
        {
            var id = Request["REID"];
            var rpeople = rp.GetRepairPeople(Guid.Parse(id));
            ViewBag.People = rpeople;
            var list = rp.GetRepairDetials(Guid.Parse(id));
            return View(list);
        }
        public ActionResult EditAudit()
        {
            var id = Request["REID"];
            var rpeople = rp.GetRepairPeople(Guid.Parse(id));
            ViewBag.id = id;
            ViewBag.People = rpeople;
            var list = rp.GetRepairDetials(Guid.Parse(id));

            //zy  转交人
            var info = rp.GetRepairAduitPeople(Guid.Parse(id));
            ViewBag.approver = info.QTBX;
         
            return View(list);
        }
        // 转交 zy
        public ActionResult EditTransfer(Guid REID, Guid ApplicantID)
        {
           // var model = entity.View_Repair.FirstOrDefault(x => x.REID == REID);
            var name = oa.GetTeacherGH(ApplicantID).DisName;
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {

              //  Repair_Basic model1 = new Repair_Basic();
                var model1 = entity.Repair_Basic.FirstOrDefault(x => x.REID == REID);
                model1.QTBX = ApplicantID.ToString();
                model1.WXFK = name;
                model1.Status = "9";
                //model1.RepairStatus = "审核中";
                //var model = entity.View_Repair.FirstOrDefault(x => x.REID == REID);
                //model.RepairStatus = "审核中";
                try
                {               
                    entity.SaveChanges();
                }
                catch (Exception ex)
                {
                    statuscode = "500";
                }
            }

            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditBackRepair()
        {
            var id = Request["REID"];
            ViewBag.id = id;
            var list = rp.GetRepairDetials(Guid.Parse(id));
            return View(list);
        }
        public ActionResult EditDealAudit()
        {
            var id = Request["REID"];
            var rpeople = rp.GetRepairPeople(Guid.Parse(id));
            ViewBag.id = id;
            ViewBag.People = rpeople;
            var list = rp.GetRepairDetials(Guid.Parse(id));
            ViewBag.Images = list.URL;
            return View(list);
        }
        [HttpPost]
        public JsonResult createSend(View_Repair model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    model.XXID = xxid;
                    var name = oa.GetTeacherGH(new Guid(model.QTBX)).DisName;
                    model.WXFK = name;
                    model.LouCeng = "1";
                    model.BXRID = userid;
                    model.Status = "1";
                    model.WXLBID = "1";
                    rp.CreateSend(model, names);
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
        public JsonResult EditRepairSend(View_Repair model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {

                try
                {
                    model.XXID = xxid;
                    var name = oa.GetTeacherGH(new Guid(model.QTBX)).DisName;
                    model.WXFK = name;
                    model.LouCeng = "1";
                    model.BXRID = userid;
                    model.Status = "1";
                    model.WXLBID = "1";
                    rp.EditRepairSend(model, names);
                }
                catch (Exception ex)
                {
                    statuscode = "500";
                }
            }

            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditAuditSend(string Id, string repairpeople)
        {
            var reid = new Guid(Id);
            var wxrid = repairpeople;
            string statuscode = "1";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    rp.EditRepairPeopleSend(reid, wxrid);
                }
                catch (Exception ex)
                {
                    statuscode = "0";
                    msg = ex.ToString();
                }
            }

            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditAuditDealSend(string Id, string repairtime, string content,string url)
        {
            var reid = new Guid(Id);
            string statuscode = "1";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var spurl = "";
                    var urlname = "";
                    if (!String.IsNullOrEmpty(url))
                    {
                        url = url.Substring(0, url.Length - 1);
                         urlname = url;
                        string HonorImageUrl = ConfigurationManager.AppSettings["HonorImageUrl"];
                        
                        var sp = url.Split(';');
                        for (int i = 0; i < sp.Length; i++)
                        {
                            spurl += (HonorImageUrl + sp[i]) + ";";
                        }
                        if (spurl.Length > 0)
                        {
                            spurl = spurl.Substring(0, spurl.Length - 1);
                        }
                     
                    }
                    rp.EditDealRepairPeopleSend(reid, repairtime, content, spurl, urlname);
                }
                catch (Exception ex)
                {
                    statuscode = "0";
                    msg = ex.ToString();
                }
            }

            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditBackRepairSend(string Id, string fk)
        {
            var reid = new Guid(Id);
            string statuscode = "1";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    rp.EditDealRepairFKSend(reid, fk);
                }
                catch (Exception ex)
                {
                    statuscode = "0";
                    msg = ex.ToString();
                }
            }

            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApprovePeopleList()
        {
            IEnumerable<SelectListItem> list = rp.GetDeptApproveList(xxid).Select(p => new SelectListItem()
            {
                Text = p.XM,
                Value = p.DriverID.ToString()
            });

            return Json(list, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetRepairPeopleList()
        {
            IEnumerable<SelectListItem> list = rp.GetDeptRepairList(xxid).Select(p => new SelectListItem()
            {
                Text = p.XM,
                Value = p.DriverID.ToString()
            });

            return Json(list, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DeleteRepair(Guid Id)
        {
            var code = "1";
            try
            {
                rp.DeleteRepair(Id);
            }
            catch (Exception)
            {
                code = "0";
            }

            return Json(code, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RepairStatistics()
        {
            return View();
        }

        public ActionResult RepairDetail()
        {
            ViewBag.SelectYear = Request["SelectYear"];
            ViewBag.RepairProject = Request["RepairProject"];

            return View();
        }

        #region 维修统计
        /// <summary>
        /// 维修统计
        /// </summary>
        /// <param name="repairType">维修类型</param>
        /// <param name="selectYear">年份</param>
        /// <returns></returns>
        public ActionResult GetRepairProject(string repairType, int selectYear)
        {
            var repairProjectViewModel = new RepairProjectViewModel();
            List<Series> serList = new List<Series>();

            try
            {
                var projectList = rp.GetRepair_DistictList(xxid, repairType);

                if (projectList.Any())
                {
                    string[] projectNames = new string[projectList.Count];

                    for (int j = 0; j < projectList.Count; j++)
                    {
                        projectNames[j] = projectList[j].RepairProjectName;
                    }

                    repairProjectViewModel.ProjectNames = projectNames;

                    var repairProjectList = rp.GetRepairListByYear(xxid, selectYear);

                    for (int i = 1; i < 13; i++)
                    {
                        Series s = new Series();
                        s.id = i;
                        s.type = "column";
                        s.name = i + "月";

                        float?[] d = new float?[projectList.Count];

                        for (int j = 0; j < projectList.Count; j++)
                        {
                            var proCount =
                                repairProjectList.Count(
                                    w =>
                                        w.BaoXiuProject == projectList[j].RepairProjectName &&
                                        w.RepairTime.Value.Month == i);

                            d[j] = proCount;
                        }

                        s.data = d;

                        serList.Add(s);
                    }

                    repairProjectViewModel.SeriesList = serList;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(NewtonsoftJson(repairProjectViewModel));
        }
        #endregion

        #region 维修统计明细
        /// <summary>
        /// 维修统计明细
        /// </summary>
        /// <param name="SelectYear">年份</param>
        /// <param name="RepairProject">维修项目</param>
        /// <returns></returns>
        public JsonResult GetRepairProjectDetail(int SelectYear, string RepairProject)
        {
            var repairProjectViewModel = new RepairProjectViewModel();

            try
            {
                var projectList = rp.GetRepairListByProjectAndYear(RepairProject, xxid, SelectYear);

                if (projectList.Any())
                {
                    foreach (var pro in projectList)
                    {
                        pro.Remark = pro.RepairTime.Value.ToString("yyyy-MM-dd");
                        pro.RepairFk = pro.BaoXiuTime.Value.ToString("yyyy-MM-dd");
                        DateTime repStart = new DateTime(Convert.ToInt32(pro.BaoXiuTime.Value.Year), Convert.ToInt32(pro.BaoXiuTime.Value.Month), Convert.ToInt32(pro.BaoXiuTime.Value.Day));
                        DateTime repEnd = new DateTime(Convert.ToInt32(pro.RepairTime.Value.Year), Convert.ToInt32(pro.RepairTime.Value.Month), Convert.ToInt32(pro.RepairTime.Value.Day));

                        int days = new TimeSpan(repEnd.Ticks - repStart.Ticks).Days;

                        pro.RepairFankui = days.ToString();
                    }
                    repairProjectViewModel.RepairProjectList = projectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(repairProjectViewModel, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region    维修统计(修改) zy
        public ActionResult GetRepairProjectTwo(int selectYear)
        {
            var repairProjectViewModel = new RepairProjectViewModel();

            try
            {
                var projectList = rp.GetRepairTypeList();
                var Sum = rp.GetRepair_DistictListTwo(xxid, selectYear);

                var totalcount1 = 0;
                var totalcount2 = 0;
                if (projectList.Any())
                {
                    string[] CodeName = new string[projectList.Count];
                    List<int> list1 = new List<int>();
                    List<int> list2 = new List<int>();
                    string[] type2 = new string[Sum.Count];

                    for (int j = 0; j < projectList.Count; j++)
                    {
                        CodeName[j] = projectList[j].CodeName;
                        totalcount1 = Sum.Count(r => r.DongLou == CodeName[j]);
                        totalcount2 = Sum.Count(r => r.DongLou == CodeName[j] && r.RepairStatus == "已处理");
                        list1.Add(totalcount1);
                        list2.Add(totalcount2);
                    }

                    //类型
                    repairProjectViewModel.ProjectNames = CodeName;
                    //保修总数
                    repairProjectViewModel.SumCount = list1;
                    //已维修总数
                    // var repairProjectList = rp.GetRepairListByYearTwo(xxid,selectYear);
                    repairProjectViewModel.HaveSumCount = list2;


                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(NewtonsoftJson(repairProjectViewModel));

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