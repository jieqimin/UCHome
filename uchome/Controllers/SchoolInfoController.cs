using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIM = SchInfo.Model;
using UCHome.Model;
using Newtonsoft.Json.Linq;
using JYTGBT;
using System.Text;

namespace UCHome.Controllers {
	public class SchoolInfoController : Controller {
		private SIM.SchInfoEntities db = new SIM.SchInfoEntities();
		private SchInfo.BLL.SchInfoBLL sibll = new SchInfo.BLL.SchInfoBLL();
		private SchInfo.BLL.DictionaryBLL dictbll = new SchInfo.BLL.DictionaryBLL();
		private SchInfo.BLL.GBTBLL gbtbll = new SchInfo.BLL.GBTBLL();
		private static List<SIM.GBT> gbt2260;
		#region 获取登录信息
		private static UserInfo user {
			get {
				return UCHomeBasePage.RequestUser;
			}
		}
		private Guid loginId {
			get {
				try {
					return user.userid;
				}
				catch (Exception) {
					return Guid.Empty;
				}
			}
		}
		private string loginname {
			get {
				return user.username;
			}
		}
		private string userType {
			get {
				return user.usertype;
			}
		}
		private string schname {
			get {
				return user.xxmc;
			}
		}
		private string areacode {
			get {
				try {
					return user.areacode;
				}
				catch (Exception) {
					return string.Empty;
				}
			}
		}

		private Guid schid {
			get {
				try {
					return user.xxid;
				}
				catch (Exception) {
					return Guid.Empty;
				}
			}
		}
		#endregion
		// GET: SchoolInfo
		#region 学校信息管理
		public ActionResult Index() {
			return View(db.SchoolInfo.ToList());
		}

		// GET: SchoolInfo/Details/5
		public ActionResult Details(Guid? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SIM.SchoolInfo schoolInfo = db.SchoolInfo.Find(id);
			if (schoolInfo == null) {
				return HttpNotFound();
			}
			return View(schoolInfo);
		}

		// GET: SchoolInfo/Create
		public ActionResult Create() {
			List<SchInfo.Model.Dictionary> schtypelist = dictbll.GetDictionaryList("学校类型");
			List<SchInfo.Model.Dictionary> schnaturelist = dictbll.GetDictionaryList("学校性质");
			var schdistrictlist = gbtbll.GetGBTList("gbt2260");
			ViewBag.SchType_DictCode = new SelectList(schtypelist, "DictCode", "DictName");
			ViewBag.SchNature_DictCode = new SelectList(schnaturelist, "DictCode", "DictName");
			ViewBag.SchDistrict_GBT2260 = new SelectList(schdistrictlist, "Code_ID", "Code_Name");
			SIM.SchoolInfo sch = sibll.GetSchInfo(schid);
			return PartialView(sch);
		}

		// POST: SchoolInfo/Create
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
		[HttpPost]
		public ActionResult Create(SIM.SchoolInfo schoolInfo) {
			if (ModelState.IsValid) {
				schoolInfo.SchID = Guid.NewGuid();
				db.SchoolInfo.Add(schoolInfo);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return PartialView(schoolInfo);
		}

		// GET: SchoolInfo/Edit/5
		public ActionResult Edit() {
			List<SchInfo.Model.Dictionary> schnaturelist = dictbll.GetDictionaryList("学校性质");
			ViewBag.SchNature_DictCode = new SelectList(schnaturelist, "DictCode", "DictName");
			SIM.SchoolInfo sch = sibll.GetSchInfo(schid);
			if (sch == null) {
				return HttpNotFound();
			}
			return PartialView(sch);
		}

		// POST: SchoolInfo/Edit/5
		// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
		// 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
		[HttpPost]
		public JsonResult Edit(SIM.SchoolInfo schoolInfo) {
			schoolInfo.IsState = 1;
			schoolInfo.LastIP = Request.UserHostAddress;
			schoolInfo.LastTime = DateTime.Now;
			schoolInfo.CreateTime = DateTime.Now;
			schoolInfo.Creator = loginId;
			if (sibll.ModifySchInfo(schoolInfo)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "保存失败", UserID = "" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}

		// GET: SchoolInfo/Delete/5
		public ActionResult Delete(Guid? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SIM.SchoolInfo schoolInfo = db.SchoolInfo.Find(id);
			if (schoolInfo == null) {
				return HttpNotFound();
			}
			return View(schoolInfo);
		}

		// POST: SchoolInfo/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(Guid id) {
			SIM.SchoolInfo schoolInfo = db.SchoolInfo.Find(id);
			db.SchoolInfo.Remove(schoolInfo);
			db.SaveChanges();
			return RedirectToAction("Index");
		}
		#endregion
		#region 学校校历管理
		private SchInfo.BLL.SICalendarBLL sicalbll = new SchInfo.BLL.SICalendarBLL();
		public ActionResult siCalendarIndex() {
			return PartialView();
		}
		public ActionResult siCalendarCreate() {
			SIM.siCalendar calendar = new SIM.siCalendar {
				CalID = Guid.NewGuid()
			};
			return PartialView();
		}
		public ActionResult siCalendarEdit(Guid calid) {
			SIM.siCalendar calendar = sicalbll.GetSICalendar(calid, false);
			return PartialView(calendar);
		}
		[HttpPost]
		public JsonResult CreateSICalendar(SIM.siCalendar calendar) {
			calendar.CalID = Guid.NewGuid();
			calendar.CalID_SchID = schid;
			calendar.IsState = 1;
			calendar.LastIP = Request.UserHostAddress;
			calendar.LastTime = DateTime.Now;
			calendar.CreateTime = DateTime.Now;
			calendar.Creator = loginId;
			if (sicalbll.AddSICalendar(calendar)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "已存在相同学期代码" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult EditSICalendar(SIM.siCalendar calendar) {
			calendar.LastIP = Request.UserHostAddress;
			calendar.LastTime = DateTime.Now;
			calendar.CreateTime = DateTime.Now;
			calendar.Creator = loginId;
			if (sicalbll.ModifySICalendar(calendar)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "已存在相同学期代码" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		public JsonResult GetCalendarList() {
			List<SIM.siCalendar> callist = sicalbll.GetSICalendarList(schid, false);
			var result = new {
				data = callist,
				recordcount = callist.Count()
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		[HttpPost]
		public JsonResult SetCurrentCal(Guid CalID) {
			bool flag = sicalbll.SetCurrentSICalendar(CalID);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult DeleteCalendar(Guid CalID) {
			bool flag = sicalbll.DeleteSICalendar(CalID);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult RescoverCalendar(Guid CalID) {
			bool flag = sicalbll.ResCoverSICalendar(CalID);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion
		#region 字典管理

		public ActionResult DictionaryIndex() {
			return PartialView();
		}
		public ActionResult DictionaryCreate() {
			SIM.Dictionary dict = new SIM.Dictionary {
				DictID = Guid.NewGuid()
			};
			return PartialView(dict);
		}
		public ActionResult DictionaryEdit(Guid dictid) {
			SIM.Dictionary dict = dictbll.GetDictionary(dictid, false);
			return PartialView(dict);
		}
		[HttpPost]
		public JsonResult CreateDictionary(SIM.Dictionary dict) {
			dict.DictID = Guid.NewGuid();
			dict.IsSwitch = 1;
			dict.IsState = 1;
			dict.LastIP = Request.UserHostAddress;
			dict.LastTime = DateTime.Now;
			dict.CreateTime = DateTime.Now;
			dict.Creator = loginId;
			if (dictbll.AddDictionary(dict)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "已存在相同字典代码" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult EditDictionary(SIM.Dictionary dict) {
			dict.LastIP = Request.UserHostAddress;
			dict.LastTime = DateTime.Now;
			dict.CreateTime = DateTime.Now;
			dict.Creator = loginId;
			if (dictbll.ModifyDictionary(dict)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "已存在相同字典代码" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		public JsonResult GetDictTypes() {
			string[] dictlist = dictbll.GetDictTypes().ToArray();
			return Json(dictlist, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetDictionaryList(string dicttype, string dictname) {
			List<SIM.Dictionary> dictlist = dictbll.GetDictionaryList(dicttype, "", dictname, false);
			var result = new {
				data = dictlist,
				recordcount = dictlist.Count()
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult DeleteDictionary(Guid dictid) {
			bool flag = dictbll.DeleteDictionary(dictid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult RescoverDictionary(Guid dictid) {
			bool flag = dictbll.RecoveryDictionary(dictid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion
		#region 课程管理
		private SchInfo.BLL.SICourseBLL sicoursebll = new SchInfo.BLL.SICourseBLL();
		private SchInfo.BLL.JYT1001BLL jytbll = new SchInfo.BLL.JYT1001BLL();
		public JsonResult GetCourseTypes() {
			List<SchInfo.Model.Dictionary> coursetypelist = dictbll.GetDictionaryList("课程分类");
			return Json(coursetypelist, JsonRequestBehavior.AllowGet);
		}
		public ActionResult SICourseIndex() {

			List<SchInfo.Model.Dictionary> coursetypelist = dictbll.GetDictionaryList("课程分类");
			ViewBag.CourseType_DictCode = new SelectList(coursetypelist, "DictCode", "DictName");
			return PartialView();
		}
		public ActionResult SICourseCreate() {
			List<SchInfo.Model.Dictionary> coursetypelist = dictbll.GetDictionaryList("课程分类");
			ViewBag.CourseType_DictCode = new SelectList(coursetypelist, "DictCode", "DictName");
			//List<JYTGBT.JYT> coursenaturelist = JYTGBT.CourseNature.coursenatures;
			List<JYT> coursenaturelist = JYTGBT.JYT1001.GetListByDMType("课程等级");
			ViewBag.CourseNature_DMCode = new SelectList(coursenaturelist, "DMCode", "DMName");
			List<JYTGBT.JYT> courselist = JYTGBT.JYT1001.GetListByDMType("学科");
			ViewBag.CourseID_DMCode = new SelectList(courselist, "DMCode", "DMName");
			//List<JYTGBT.JYT> coursemethodlist = JYTGBT.TechMethod.techmethods;
			List<JYT> coursemethodlist = JYTGBT.JYT1001.GetListByDMType("授课方式");
			ViewBag.CourseTechMethod_DMCode = new SelectList(coursemethodlist, "DMCode", "DMName");
			SIM.siCourse course = new SIM.siCourse {
				CourseID = Guid.NewGuid()
			};
			return PartialView(course);
		}
		public ActionResult SICoursePatchCreate() {
			List<SIM.siCourse> courselist = sicoursebll.GetSICourseList(schid);

			List<JYTGBT.JYT> subjects = JYTGBT.JYT1001.GetListByDMType("学科");
			List<SelectListItem> subjectlist = new List<SelectListItem>();
			foreach (var item in subjects) {
				if (courselist.Any(c => c.CourseID_DMCode == item.DMCode))
					subjectlist.Add(new SelectListItem { Text = item.DMName, Value = item.DMCode, Selected = true });
				else
					subjectlist.Add(new SelectListItem { Text = item.DMName, Value = item.DMCode, Selected = false });
			}
			return PartialView(subjectlist);
		}
		public ActionResult SICourseEdit(Guid courseid) {
			List<SchInfo.Model.Dictionary> coursetypelist = dictbll.GetDictionaryList("课程分类");
			ViewBag.CourseType_DictCode = new SelectList(coursetypelist, "DictCode", "DictName");
			List<JYTGBT.JYT> coursenaturelist = JYTGBT.JYT1001.GetListByDMType("课程等级");
			ViewBag.CourseNature_DMCode = new SelectList(coursenaturelist, "DMCode", "DMName");
			List<JYTGBT.JYT> courselist = JYTGBT.JYT1001.GetListByDMType("学科");
			ViewBag.CourseID_DMCode = new SelectList(courselist, "DMCode", "DMName");
			List<JYTGBT.JYT> coursemethodlist = JYTGBT.JYT1001.GetListByDMType("授课方式");
			ViewBag.CourseTechMethod_DMCode = new SelectList(coursemethodlist, "DMCode", "DMName");
			SIM.siCourse course = sicoursebll.GetSICourse(courseid, false);
			return PartialView(course);
		}
		[HttpPost]
		public JsonResult CreateSICourse(SIM.siCourse course) {
			course.CourseID = Guid.NewGuid();
			course.CourseID_SchID = schid;
			course.CourseCode = course.CourseID_DMCode;
			course.IsState = 1;
			course.LastIP = Request.UserHostAddress;
			course.LastTime = DateTime.Now;
			course.CreateTime = DateTime.Now;
			course.Creator = loginId;
			if (sicoursebll.AddSICourse(course)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "该学科已关联" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult PatchCreateSICourse(List<SelectListItem> subjects) {
			int i = 0;
			sicoursebll.PatchDeleteSICourse(schid);
			foreach (var item in subjects) {
				SIM.siCourse course = new SIM.siCourse();
				course.CourseID = Guid.NewGuid();
				course.CourseID_SchID = schid;
				course.CourseCode = item.Value;
				course.CourseID_DMCode = item.Value;
				course.CourseNature_DMCode = "1";
				course.CourseType_DictCode = "1";
				course.CourseTechMethod_DMCode = "1";
				course.CourseName = item.Text;
				course.IsState = 1;
				course.LastIP = Request.UserHostAddress;
				course.LastTime = DateTime.Now;
				course.CreateTime = DateTime.Now;
				course.Creator = loginId;
				if (!sicoursebll.AddSICourse(course)) {
					sicoursebll.RecoverySICourse(schid, item.Value);

				}
				i++;
			}

			if (i > 0) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "保存失败，存在异常！" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult EditSICourse(SIM.siCourse course) {
			course.CourseCode = course.CourseID_DMCode;
			course.LastIP = Request.UserHostAddress;
			course.LastTime = DateTime.Now;
			course.CreateTime = DateTime.Now;
			course.Creator = loginId;
			if (sicoursebll.ModifySICourse(course)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "已存在相同课程代码" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}

		public JsonResult GetSICourseList(string coursetype) {
			List<SIM.siCourse> courselist = sicoursebll.GetSICourseList(schid, coursetype, false);
			var result = new {
				data = courselist,
				recordcount = courselist.Count()
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult DeleteSICourse(Guid courseid) {
			bool flag = sicoursebll.DeleteSICourse(courseid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult RescoverSICourse(Guid courseid) {
			bool flag = sicoursebll.RecoverySICourse(courseid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion
		#region 部门管理
		private SchInfo.BLL.SIDeptBLL sideptbll = new SchInfo.BLL.SIDeptBLL();
		public ActionResult SIDeptIndex() {
			return PartialView();
		}
		public ActionResult SIDeptCreate() {

			SIM.siDept dept = new SIM.siDept {
				DeptID = Guid.NewGuid()
			};
			return PartialView(dept);
		}
		public ActionResult SIDeptEdit(Guid deptid) {
			SIM.siDept dept = sideptbll.GetSIDept(deptid, false);
			return PartialView(dept);
		}
		[HttpPost]
		public JsonResult CreateSIDept(SIM.siDept dept) {
			dept.DeptID = Guid.NewGuid();
			dept.DeptID_SchID = schid;
			dept.IsState = 1;
			dept.LastIP = Request.UserHostAddress;
			dept.LastTime = DateTime.Now;
			dept.CreateTime = DateTime.Now;
			dept.Creator = loginId;
			if (sideptbll.AddSIDept(dept)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "该学科已关联" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult EditSIDept(SIM.siDept dept) {
			dept.LastIP = Request.UserHostAddress;
			dept.LastTime = DateTime.Now;
			dept.CreateTime = DateTime.Now;
			dept.Creator = loginId;
			if (sideptbll.ModifySIDept(dept)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "已存在相同课程代码" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}

		public JsonResult GetSIDeptList() {
			List<SIM.siDept> deptlist = sideptbll.GetSIDeptList(schid, false);
			var result = new {
				data = deptlist,
				recordcount = deptlist.Count()
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult DeleteSIDept(Guid deptid) {
			bool flag = sideptbll.DeleteSIDept(deptid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult RescoverSIDept(Guid deptid) {
			bool flag = sideptbll.RecoverySIDept(deptid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion
		#region 学校年级设置
		private SchInfo.BLL.SIGradeBLL sigradebll = new SchInfo.BLL.SIGradeBLL();
		public ActionResult SIGradePatchCreate() {
			List<SIM.SchStage> schstage = sigradebll.GetSIStages(schid);

			//List<JYTGBT.JYT> subjects = JYTGBT.Subject.subjects;
			//List<SelectListItem> subjectlist = new List<SelectListItem>();
			//foreach (var item in subjects) {
			//	if (courselist.Any(c => c.CourseID_DMCode == item.DMCode))
			//		subjectlist.Add(new SelectListItem { Text = item.DMName, Value = item.DMCode, Selected = true });
			//	else
			//		subjectlist.Add(new SelectListItem { Text = item.DMName, Value = item.DMCode, Selected = false });
			//}
			return PartialView(schstage);
		}
		public string IntToString(int stage, int i) {
			switch (i) {
				case 1:
					return stage == 1 ? "小班" : "一年级";
				case 2:
					return stage == 1 ? "中班" : "二年级";
				case 3:
					return stage == 1 ? "大班" : "三年级";
				case 4:
					return "四年级";
				case 5:
					return "五年级";
				case 6:
					return "六年级";
				default:
					return "其他年级";
			}
		}
		[HttpPost]
		public JsonResult PatchCreateSIGrade(List<SelectListItem> grades) {
			int i = 0;
			sigradebll.PatchDeleteSIGrade(schid);
			foreach (var item in grades) {
				var gradevalue = item.Value;
				int gradestage = int.Parse(gradevalue.Substring(0, 1));
				int gradecode = int.Parse(gradevalue.Substring(1, 1));
				for (int m = 1; m <= gradecode; m++) {
					SIM.siGrade grade = new SIM.siGrade();
					grade.GradeID = Guid.NewGuid();
					grade.GradeID_SchID = schid;
					grade.GradeCode = m;
					grade.GradeName = item.Text + IntToString(gradestage, m);
					grade.GradeStage = gradestage;
					grade.IsState = 1;
					grade.LastIP = Request.UserHostAddress;
					grade.LastTime = DateTime.Now;
					grade.CreateTime = DateTime.Now;
					grade.Creator = loginId;
					if (!sigradebll.AddSIGrade(grade)) {
						i++;
					}
				}
			}
			if (i == 0) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "保存失败，存在异常！" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion
		#region 场地管理
		private SchInfo.BLL.SIGroundBLL sigroundbll = new SchInfo.BLL.SIGroundBLL();
		public JsonResult GetGroundTypes() {
			List<SchInfo.Model.Dictionary> groundtypelist = dictbll.GetDictionaryList("场地类别");
			return Json(groundtypelist, JsonRequestBehavior.AllowGet);
		}
		public ActionResult SIGroundIndex() {
			return PartialView();
		}
		public ActionResult SIGroundCreate() {
			List<SchInfo.Model.Dictionary> groundtypelist = dictbll.GetDictionaryList("场地类别");
			ViewBag.GroundType_DictCode = new SelectList(groundtypelist, "DictCode", "DictName");
			SIM.siGround ground = new SIM.siGround {
				GroundID = Guid.NewGuid()
			};
			return PartialView(ground);
		}
		public ActionResult SIGroundEdit(Guid groundid) {
			List<SchInfo.Model.Dictionary> groundtypelist = dictbll.GetDictionaryList("场地类别");
			ViewBag.GroundType_DictCode = new SelectList(groundtypelist, "DictCode", "DictName");
			SIM.siGround ground = sigroundbll.GetSIGround(groundid, false);
			return PartialView(ground);
		}
		[HttpPost]
		public JsonResult CreateSIGround(SIM.siGround ground) {
			ground.GroundID = Guid.NewGuid();
			ground.GroundID_SchID = schid;
			ground.IsState = 1;
			ground.LastIP = Request.UserHostAddress;
			ground.LastTime = DateTime.Now;
			ground.CreateTime = DateTime.Now;
			ground.Creator = loginId;
			if (sigroundbll.AddSIGround(ground)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "网络异常，请稍候再试！" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult EditSIGround(SIM.siGround ground) {
			ground.LastIP = Request.UserHostAddress;
			ground.LastTime = DateTime.Now;
			ground.CreateTime = DateTime.Now;
			ground.Creator = loginId;
			if (sigroundbll.ModifySIGround(ground)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "网络异常，请稍候重试" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}

		public JsonResult GetSIGroundList() {
			List<SIM.siGround> groundlist = sigroundbll.GetSIGroundList(schid, false);
			var result = new {
				data = groundlist,
				recordcount = groundlist.Count()
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult DeleteSIGround(Guid groundid) {
			bool flag = sigroundbll.DeleteSIGround(groundid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult RescoverSIGround(Guid groundid) {
			bool flag = sigroundbll.RecoverySIGround(groundid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion
		#region 场馆管理
		private SchInfo.BLL.SIVenueBLL sivenuebll = new SchInfo.BLL.SIVenueBLL();
		public JsonResult GetGrounds() {
			List<SIM.siGround> grounds = sigroundbll.GetSIGroundList(schid);
			return Json(grounds, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetVenueTypes() {
			List<SchInfo.Model.Dictionary> venuetypelist = dictbll.GetDictionaryList("场馆分类");
			return Json(venuetypelist, JsonRequestBehavior.AllowGet);
		}
		public ActionResult SIVenueIndex() {
			List<SIM.siGround> grounds = sigroundbll.GetSIGroundList(schid);
			ViewBag.Grounds = grounds;
			return PartialView();
		}
		public ActionResult SIVenueCreate() {
			List<SchInfo.Model.Dictionary> venuetypelist = dictbll.GetDictionaryList("场馆分类");
			ViewBag.VenueType_DictCode = new SelectList(venuetypelist, "DictCode", "DictName");
			List<SIM.siGround> grounds = sigroundbll.GetSIGroundList(schid);
			ViewBag.VenueID_GroundID = new SelectList(grounds, "GroundID", "GroundName");
			SIM.siVenue venue = new SIM.siVenue {
				VenueID = Guid.NewGuid()
			};
			return PartialView(venue);
		}
		public ActionResult SIVenueEdit(Guid venueid) {
			List<SchInfo.Model.Dictionary> venuetypelist = dictbll.GetDictionaryList("场馆分类");
			ViewBag.VenueType_DictCode = new SelectList(venuetypelist, "DictCode", "DictName");
			List<SIM.siGround> grounds = sigroundbll.GetSIGroundList(schid);
			ViewBag.VenueID_GroundID = new SelectList(grounds, "GroundID", "GroundName");
			SIM.siVenue venue = sivenuebll.GetSIVenue(venueid, false);
			return PartialView(venue);
		}
		[HttpPost]
		public JsonResult CreateSIVenue(SIM.siVenue venue) {
			venue.VenueID = Guid.NewGuid();
			venue.VenueID_SchID = schid;
			venue.IsState = 1;
			venue.LastIP = Request.UserHostAddress;
			venue.LastTime = DateTime.Now;
			venue.CreateTime = DateTime.Now;
			venue.Creator = loginId;
			if (sivenuebll.AddSIVenue(venue)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "网络异常，请稍候再试！" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult EditSIVenue(SIM.siVenue venue) {
			venue.LastIP = Request.UserHostAddress;
			venue.LastTime = DateTime.Now;
			venue.CreateTime = DateTime.Now;
			venue.Creator = loginId;
			if (sivenuebll.ModifySIVenue(venue)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "网络异常，请稍候重试" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}

		public JsonResult GetSIVenueList(Guid groundid) {
			if (groundid == Guid.Empty) {
				List<SIM.siVenue> venuelist = sivenuebll.GetSIVenueList(schid, false);
				var result = new {
					data = venuelist,
					recordcount = venuelist.Count()
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				List<SIM.siVenue> venuelist = sivenuebll.GetSIVenueList(schid, groundid, false);
				var result = new {
					data = venuelist,
					recordcount = venuelist.Count()
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}

		}

		[HttpPost]
		public JsonResult DeleteSIVenue(Guid venueid) {
			bool flag = sivenuebll.DeleteSIVenue(venueid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult RescoverSIVenue(Guid venueid) {
			bool flag = sivenuebll.RecoverySIVenue(venueid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion
		#region 班级管理
		private SchInfo.BLL.SIClassBLL siclassbll = new SchInfo.BLL.SIClassBLL();
		public ActionResult SIClassIndex() {
			List<SchInfo.Model.Grade> gradelist = sigradebll.GetSIGradeCodes(schid);
			ViewBag.Grades = new SelectList(gradelist, "GradeCode", "GradeName");
			return PartialView();
		}
		public ActionResult SIClassCreate() {
			List<SchInfo.Model.siVenue> venuelist = sivenuebll.GetSIVenueList(schid);
			ViewBag.ClassRoom_VenueID = new SelectList(venuelist, "VenueID", "VenueName");
			List<int> stages = sigradebll.GetSIGradeStages(schid);
			ViewBag.Stages = stages;
			SIM.siClass siclass = new SIM.siClass {
				ClassID = Guid.NewGuid()
			};
			return PartialView(siclass);
		}
		public ActionResult SIClassPatchCreate() {
			List<SchInfo.Model.Grade> gradelist = sigradebll.GetSIGradeCodes(schid);
			return PartialView(gradelist);
		}
		public ActionResult SIClassEdit(Guid classid) {
			List<SchInfo.Model.siVenue> venuelist = sivenuebll.GetSIVenueList(schid);
			ViewBag.ClassRoom_VenueID = new SelectList(venuelist, "VenueID", "VenueName");
			List<int> stages = sigradebll.GetSIGradeStages(schid);
			ViewBag.Stages = stages;
			SIM.siClass siclass = siclassbll.GetSIClass(classid, false);
			return PartialView(siclass);
		}
		[HttpPost]
		public JsonResult CreateSIClass(SIM.siClass siclass) {
			siclass.ClassID = Guid.NewGuid();
			siclass.ClassID_SchID = schid;
			siclass.IsGraduation = 1;
			siclass.IsState = 1;
			siclass.LastIP = Request.UserHostAddress;
			siclass.LastTime = DateTime.Now;
			siclass.CreateTime = DateTime.Now;
			siclass.Creator = loginId;
			if (siclassbll.AddSIClass(siclass)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "该班级已存在" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		public int year {
			get {
				return sigradebll.GetSIGradeCodes(schid, 2).Count();
			}
		}
		[HttpPost]
		public JsonResult PatchCreateSIClass(List<SelectListItem> classes) {
			SIM.siCalendar calendar = sicalbll.GetCurrentSICalendar(schid);
			string curyear;
			if (calendar != null) {
				string cur = calendar.CalCode;
				curyear = cur.Substring(0, 4);
			}
			else {
				curyear = DateTime.Now.Year.ToString();
			}
			int i = 0;
			List<SIM.SchStage> schstages = sigradebll.GetSIStages(schid);
			foreach (var item in classes) {

				int gradestage = int.Parse(item.Text.Substring(0, 1));
				int gradecode = int.Parse(item.Text.Substring(1, 1));
				int createyear = siclassbll.GetCreateYear(gradecode, curyear);
				SIM.SchStage schstage = schstages.SingleOrDefault(s => s.StageCode == gradestage);
				int year = schstage != null ? schstage.years : 0;
				int graduationyear = siclassbll.GetGraduationYear(year, gradecode, curyear);
				for (int m = 1; m <= int.Parse(item.Value); m++) {
					SIM.siClass siclass = new SIM.siClass();
					siclass.ClassID = Guid.NewGuid();
					siclass.ClassID_SchID = schid;
					siclass.ClassName = m < 10 ? string.Format("0{0}", m) + "班" : m + "班";
					siclass.ClassCreateYear = createyear.ToString();
					siclass.ClassGraduationYear = graduationyear.ToString();
					siclass.ClassType_DictCode = "行政班";
					siclass.ClassStage = gradestage;
					siclass.IsGraduation = 1;
					siclass.IsState = 1;
					siclass.LastIP = Request.UserHostAddress;
					siclass.LastTime = DateTime.Now;
					siclass.CreateTime = DateTime.Now;
					siclass.Creator = loginId;
					if (siclassbll.AddSIClass(siclass)) {
						i++;
					}
				}

			}

			if (i > 0) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "保存失败，存在异常！" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult EditSIClass(SIM.siClass siclass) {
			siclass.LastIP = Request.UserHostAddress;
			siclass.LastTime = DateTime.Now;
			siclass.CreateTime = DateTime.Now;
			siclass.Creator = loginId;
			if (siclassbll.ModifySIClass(siclass)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "该班级已存在" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}

		public JsonResult GetSIClassList(string grade) {
			SIM.siCalendar calendar = sicalbll.GetCurrentSICalendar(schid);
			string curyear;
			if (calendar != null) {
				string cur = calendar.CalCode;
				curyear = cur.Substring(0, 4);
			}
			else {
				curyear = DateTime.Now.Year.ToString();
			}
			if (grade == "0") {
				List<SIM.siClass> classlist2 = siclassbll.GetSIClassList(schid, true, false);
				var result2 = new {
					data = classlist2,
					recordcount = classlist2.Count(),
					curyear = curyear
				};
				return Json(result2, JsonRequestBehavior.AllowGet);
			}
			int gradestage = int.Parse(grade.Substring(0, 1));
			int gradecode = int.Parse(grade.Substring(1, 1));
			//通过gradecode获取入学年份
			List<SIM.siClass> classlist = siclassbll.GetSIClassList(schid, curyear, gradestage, gradecode, false);
			var result = new {
				data = classlist,
				recordcount = classlist.Count(),
				curyear = curyear
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult DeleteSIClass(Guid classid) {
			bool flag = siclassbll.DeleteSIClass(classid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult RescoverSIClass(Guid classid) {
			bool flag = siclassbll.RecoverySIClass(classid);
			if (flag) {
				var result = new {
					statuscode = 200
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				var result = new {
					statuscode = 500
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion
		#region 生成JYT001 JSON格式字符串
		public ActionResult GeneralJYTJSON() {
			return View();
		}
		public ActionResult GeneralJSON() {
			List<SIM.JYT1001> jytlist = jytbll.GetJYT1001List();
			List<string> dmtypes = jytlist.Select(dm => dm.DMType).Distinct().ToList();
			StringBuilder jsonstr = new StringBuilder();
			string split = string.Empty;
			foreach (string dmtype in dmtypes) {
				List<SIM.JYT1001> childjytlist = jytlist.Where(j => j.DMType == dmtype).ToList();
				string childsplit = string.Empty;
				StringBuilder childjsonstr = new StringBuilder();
				foreach (SIM.JYT1001 jyt in childjytlist) {
					childjsonstr.AppendFormat("{7}{{\"DMID\":\"{0}\",\"DMCode\":\"{1}\",\"DMName\":\"{2}\",\"DMOrder\":{3},\"IsSwitch\":{4},\"IsState\":{5},\"DMType\":\"{6}\"}}",
						jyt.DMID, jyt.DMCode, jyt.DMName, jyt.DMOrder, jyt.IsSwitch, jyt.IsState, jyt.DMType, childsplit);
					childsplit = ",";
				}
				jsonstr.AppendFormat("{0}\"{1}\":[{2}]", split, dmtype, childjsonstr);
				split = ",";
			}
			StringBuilder results = new StringBuilder();
			results.AppendFormat("{{{0}}}", jsonstr);
			return View("GeneralJYTJSON", results);
		}
		#endregion
		#region 公共部分
		[HttpGet]
		public JsonResult GetProvince() {
			gbt2260 = gbtbll.GetGBTList("gbt2260");
			List<SIM.GBT> provinces = gbt2260.Where(p => p.Code_ID.EndsWith("0000")).ToList();

			return Json(provinces, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult GetCity(string provincecode) {
			var selprovince = provincecode.Substring(0, 2);
			List<SIM.GBT> citys = gbt2260.Where(p => p.Code_ID != provincecode && p.Code_ID.EndsWith("00") && p.Code_ID.StartsWith(selprovince)).ToList();
			return Json(citys, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult GetDistrict(string citycode) {
			var selprovince = citycode.Substring(0, 4);
			List<SIM.GBT> districts = gbt2260.Where(p => p.Code_ID != citycode && p.Code_ID.StartsWith(selprovince)).ToList();
			return Json(districts, JsonRequestBehavior.AllowGet);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				db.Dispose();
			}
			base.Dispose(disposing);
		}
		#endregion

	}
}
