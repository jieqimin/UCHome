using JYTGBT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using UCHome.Model;
using UCHome.Models;
using SIM = StuInfo.Model;

namespace UCHome.Controllers {
	public class StuInfoController : Controller {
		private SIM.StuInfoEntities db = new SIM.StuInfoEntities();
		private SchInfo.BLL.DictionaryBLL dictbll = new SchInfo.BLL.DictionaryBLL();
		private SchInfo.BLL.GBTBLL gbtbll = new SchInfo.BLL.GBTBLL();
		private SchInfo.BLL.JYT1001BLL jybbll = new SchInfo.BLL.JYT1001BLL();
		private SchInfo.BLL.SIGradeBLL sigradebll = new SchInfo.BLL.SIGradeBLL();
		private SchInfo.BLL.SICalendarBLL sicalbll = new SchInfo.BLL.SICalendarBLL();
		private SchInfo.BLL.SIClassBLL sicbll = new SchInfo.BLL.SIClassBLL();
		#region 获取登录信息
		private UserInfo user {
			get {
				return UCHomeBasePage.RequestUser;
			}
		}
		private string curyearterm {
			get {
				UCHomeBasePage ubp = new UCHomeBasePage();
				return ubp.CurrentSchTerm;
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
		#region 学生基本管理
		private StuInfo.BLL.StuInfoBLL stubll = new StuInfo.BLL.StuInfoBLL();
		private StuInfo.BLL.vwStuInfoBLL vwstubll = new StuInfo.BLL.vwStuInfoBLL();
		//public JsonResult GetGroundTypes() {
		//	List<SchInfo.Model.Dictionary> groundtypelist = dictbll.GetDictionaryList("场地类别");
		//	return Json(groundtypelist, JsonRequestBehavior.AllowGet);
		//}
		public ActionResult Index() {
			List<SchInfo.Model.Grade> gradelist = sigradebll.GetSIGradeCodes(schid);
			ViewBag.Grades = new SelectList(gradelist, "GradeCode", "GradeName");
			return PartialView();
		}
		public void InitDrop(Guid? ClassID) {
			//班级
			List<SchInfo.Model.Grade> gradelist = sigradebll.GetSIGradeCodes(schid);
			if (ClassID != null) {
				SchInfo.Model.siClass siclass = sicbll.GetSIClass(ClassID.Value, false);
				if (siclass != null) {
					string createyear = siclass.ClassCreateYear;
					int grade = int.Parse(curyearterm.Substring(0, 4)) - int.Parse(createyear) + 1;
					int classstage = siclass.ClassStage;
					string gradecode = string.Format("{0}{1}", classstage, grade);
					ViewBag.Grade = gradecode;
				}
			}
			ViewBag.Grades = new SelectList(gradelist, "GradeCode", "GradeName");
			//身份类型
			List<JYT> idtypelist = JYTGBT.JYT1001.GetListByDMType("身份类型");
			ViewBag.StuIdentityType = new SelectList(idtypelist, "DMCode", "DMName");
			//性别
			List<JYT> genderlist = JYTGBT.JYT1001.GetListByDMType("性别");
			ViewBag.StuGender = new SelectList(genderlist, "DMCode", "DMName");
			//血型
			List<JYT> bloodtypelist = JYTGBT.JYT1001.GetListByDMType("血型");
			ViewBag.StuBloodType = new SelectList(bloodtypelist, "DMCode", "DMName");
			//省市区
			//民族
			List<JYT> nationlist = JYTGBT.JYT1001.GetListByDMType("民族");
			ViewBag.StuNation = new SelectList(nationlist, "DMCode", "DMName");
			//健康状况
			List<JYT> healthlist = JYTGBT.JYT1001.GetListByDMType("健康状况");
			ViewBag.StuHealth = new SelectList(healthlist, "DMCode", "DMName");
			//政治面貌
			List<JYT> stupoliticslist = JYTGBT.JYT1001.GetListByDMType("政治面貌");
			ViewBag.StuPolitics = new SelectList(stupoliticslist, "DMCode", "DMName");
			//国籍
			//List<JYT> nationalitylist = JYTGBT.JYT1001.GetListByDMType("国籍");
			//学生类别
			List<JYT> stutypelist = JYTGBT.JYT1001.GetListByDMType("学生类别");
			ViewBag.StuType = new SelectList(stutypelist, "DMCode", "DMName");
		}
		public ActionResult Create() {
			SIM.StuInfo stu = new SIM.StuInfo {
				StuID = Guid.NewGuid(),
				StuGender = "9",
				StuGAT = "0",
				IsHobo = "0",
				IsState = 1,
				StuStatus = "01",
				StuMarital = "10",
				StuNationality = "156",
				StuPolitics = "13"
			};
			InitDrop(null);
			return PartialView(stu);
		}
		public ActionResult Edit(Guid stuid) {
			SIM.StuInfo stu = stubll.GetStuInfo(stuid, false);
			InitDrop(stu.StuID_BJID);
			return PartialView(stu);
		}
		public ActionResult Details(Guid stuid) {
			SIM.StuInfo stu = stubll.GetStuInfo(stuid, false);
			InitDrop(stu.StuID_BJID);
			return PartialView(stu);
		}
		[HttpPost]
		public JsonResult CreateStu(SIM.StuInfo stu) {
			string stuhomedistrict = stu.StuHomeDistrict;
			if (stuhomedistrict != null && stuhomedistrict.Length == 6) {
				stu.StuHomeCity = stuhomedistrict.Substring(0, 4) + "00";
				stu.StuHomeProvince = stuhomedistrict.Substring(0, 2) + "0000";
			}
			stu.StuID = Guid.NewGuid();
			stu.StuID_SchID = schid;
			stu.IsState = 1;
			stu.LastIP = Request.UserHostAddress;
			stu.LastTime = DateTime.Now;
			stu.CreateTime = DateTime.Now;
			stu.Creator = loginId;
			if (stubll.AddStuInfo(stu)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "操作失败，系统存在相同学籍号或身份证号！" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpPost]
		public JsonResult EditStu(SIM.StuInfo stu) {
			stu.LastIP = Request.UserHostAddress;
			stu.LastTime = DateTime.Now;
			stu.CreateTime = DateTime.Now;
			stu.Creator = loginId;
			if (stubll.ModifyStuInfo(stu)) {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 200, ErrorMessage = "保存成功" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				JsonResult result = new JsonResult {
					Data = new { StatusCode = 400, ErrorMessage = "操作失败，系统存在相同学籍号或身份证号" }
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}

		public JsonResult GetStuList(string stu) {
			List<SIM.vwStuInfo> stulist = vwstubll.GetStuList(stu, stu, stu, stu, 100, false);
			int count = vwstubll.GetStuCount(stu, stu, stu, stu, false);
			var result = new {
				data = stulist,
				recordcount = count,
				curyt = curyearterm.Substring(0, 4)
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetStuList2(string grade, string classid, string stuname, string stucode, string stunumber, string stuidentity) {
			if (grade == "0") {
				List<SIM.vwStuInfo> stulist = vwstubll.GetStuList2(stunumber, stucode, stuidentity, stuname, 100, false);
				int count = vwstubll.GetStuCount2(stunumber, stucode, stuidentity, stuname, false);
				var result = new {
					data = stulist,
					recordcount = count,
					curyt = curyearterm.Substring(0, 4)
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else if (classid == "0") {
				List<SchInfo.Model.siClass> classlist = GetClassList(grade);
				List<Guid> bjids = classlist.Select(c => c.ClassID).ToList();
				List<SIM.vwStuInfo> stulist = vwstubll.GetStuList(schid, bjids, stunumber, stucode, stuidentity, stuname, 100, false);
				int count = vwstubll.GetStuCount(schid, bjids, stunumber, stucode, stuidentity, stuname, false);
				var result = new {
					data = stulist,
					recordcount = count,
					curyt = curyearterm.Substring(0, 4)
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			else {
				Guid bjid = Guid.Parse(classid);
				List<SIM.vwStuInfo> stulist = vwstubll.GetStuList(schid, bjid, stunumber, stucode, stuidentity, stuname, 100, false);
				int count = vwstubll.GetStuCount(schid, bjid, stunumber, stucode, stuidentity, stuname, false);
				var result = new {
					data = stulist,
					recordcount = count,
					curyt = curyearterm.Substring(0, 4)
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		public List<SchInfo.Model.siClass> GetClassList(string grade) {
			string curyear = curyearterm.Substring(0, 4);
			if (grade == "0") {
				List<SchInfo.Model.siClass> classlist2 = sicbll.GetSIClassList(schid, true, true);
				return classlist2;
			}
			int gradestage = int.Parse(grade.Substring(0, 1));
			int gradecode = int.Parse(grade.Substring(1, 1));
			//通过gradecode获取入学年份
			List<SchInfo.Model.siClass> classlist = sicbll.GetSIClassList(schid, curyear, gradestage, gradecode, true);
			return classlist;
		}
		public JsonResult GetSIClassList(string grade) {
			//通过gradecode获取入学年份
			List<SchInfo.Model.siClass> classlist = GetClassList(grade);
			var result = new {
				data = classlist
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		[HttpPost]
		public JsonResult DeleteStu(Guid stuid) {
			bool flag = stubll.DeleteStuInfo(stuid);
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
		public JsonResult RescoverStu(Guid stuid) {
			bool flag = stubll.RecoveryStuInfo(stuid);
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
		#region 学生导入
		public ActionResult StuImport() {
			return PartialView();
		}
		public ActionResult ImportFile() {
			try {
				HttpPostedFileBase upfile = Request.Files["file"];
				string fileExtenSion = Path.GetExtension(upfile.FileName);
				if (fileExtenSion != null && (fileExtenSion.ToLower() != ".xls" && fileExtenSion.ToLower() != ".xlsx")) {
					JsonResult jr = new JsonResult { Data = new { statuscode = 500, photourl = "请上传xls或xlsx格式的Excel文件" } };
					return Json(jr, JsonRequestBehavior.AllowGet);
				}
				else {
					//保存上传文件到指定路径
					UploadHelper uphelper = new UploadHelper();
					string savefilename = string.Format("{0}{1}", loginname, DateTime.Now.ToString("yyyyMMddHHmmss"));
					string webpath = uphelper.SaveAsPublicFile(upfile, "StuImportModel", "stuimport");//默认保存文件路径upload/stuimport/StuImportModel.xls

					JsonResult jr = new JsonResult {
						Data =
							new {
								statuscode = 200,
								fileid = Guid.NewGuid(),
								filepath = webpath,
								filename = upfile == null ? "" : upfile.FileName
							}
					};
					return Json(jr, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception) {
				JsonResult jr = new JsonResult { Data = new { statuscode = 500, photourl = "找不到文件路径" } };
				return Json(jr, JsonRequestBehavior.AllowGet);
			}
		}
		public JsonResult CheckData(string filepath) {
			List<StuInfo.Model.StuInfo> stus;
			DataTable dt = DataValid(filepath, out stus, schid, "true");
			return Json("", JsonRequestBehavior.AllowGet);
		}
		private DataTable DataValid(string filepath, out List<StuInfo.Model.StuInfo> students, Guid? XXID, string isCover) {
			DataTable dt;
			string errormsg = "";
			dt = ReadFile(filepath, out errormsg);
			InitModel(dt, out students, new Guid(XXID.ToString()), isCover);
			return dt;
		}
		private DataTable ReadFile(string filepath, out string errormsg) {
			DataTable dt = DataImportHelper.ImportExcelFile(filepath, Path.GetExtension(filepath), 0);
			bool error = false;
			errormsg = "";
			//移除空行
			int coulumnCount = dt.Columns.Count;
			List<DataRow> removeDataRows = new List<DataRow>();
			for (int i = 0; i < dt.Rows.Count; i++) {
				int count = 0;
				for (int j = 0; j < coulumnCount; j++) {
					if (dt.Rows[i].IsNull(j) || dt.Rows[i][j].ToString().Trim() == "") {
						count = count + 1;
					}
				}
				if (count == coulumnCount)//整行为空
				{
					removeDataRows.Add(dt.Rows[i]);
				}
			}
			foreach (DataRow item in removeDataRows) {
				dt.Rows.Remove(item);
			}
			//验证文件列符合导入标准
			string[] strArray = { "学段", "进班年份", "毕业年份", "班级", "学籍号", "地区学号", "姓名", "身份证号" };
			if (dt.Columns.Count == strArray.Length) {
				for (int j = 0; j < dt.Columns.Count; j++) {
					if (strArray[j] != dt.Rows[0][j].ToString().Trim()) {
						errormsg = "上传文件的列名或列序与模板的列名或列序不匹配";
						dt = new DataTable();
						error = true;
						break;
					}
				}
			}
			else {
				dt = new DataTable();
				errormsg = "上传文件的列数与模板的列数不匹配";
				error = true;
			}
			if (error == false) {
				#region dt detail
				//dt.Columns["登录帐号"].ColumnName = "LoginName";
				//dt.Columns["学校"].ColumnName = "XXID";
				dt.Columns["学段"].ColumnName = "ClassStage";
				dt.Columns["进班年份"].ColumnName = "ClassCreateYear";
				dt.Columns["毕业年份"].ColumnName = "ClassGraduationYear";
				dt.Columns["班级"].ColumnName = "ClassName";
				dt.Columns["学籍号"].ColumnName = "StuCode";
				dt.Columns["地区学号"].ColumnName = "StuNumber";
				dt.Columns["姓名"].ColumnName = "StuName";
				dt.Columns["身份证号"].ColumnName = "StuIdentity";
				#endregion
			}
			return dt;
		}

		private void InitModel(DataTable dt, out List<StuInfo.Model.StuInfo> stulist, Guid XXID, string isCover) {
            //增加错误警告列
            //dt.Columns.Add("ValidMsg");
            //dt.Columns.Add("BJID");
            //dt.Rows[0]["ValidMsg"] = "验证结果";
            //dt.Rows[0]["BJID"] = "班级ID";
            //stulist = new List<StuInfo.Model.StuInfo>();

            ////获取学校班级信息
            //var bjList = _publicHandler.GetClassList(XXID);

            //#region 循环
            //for (int i = 1; i < dt.Rows.Count; i++)//i=1,skip the first row
            //{
            //	StringBuilder errorstrb = new StringBuilder();
            //	try {
            //		if (string.IsNullOrEmpty(dt.Rows[i]["XJH"].ToString()) ||
            //			string.IsNullOrEmpty(dt.Rows[i]["NJDM"].ToString()) ||
            //			string.IsNullOrEmpty(dt.Rows[i]["XM"].ToString()) ||
            //			string.IsNullOrEmpty(dt.Rows[i]["ZYDM"].ToString()) ||
            //			string.IsNullOrEmpty(dt.Rows[i]["SFZJH"].ToString()) ||
            //			string.IsNullOrEmpty(dt.Rows[i]["BJMC"].ToString())) {
            //			//errorstrb.AppendFormat("本行中存在空数据；");
            //			dt.Rows[i]["ValidMsg"] = "本行中存在空数据";
            //			continue;
            //		}
            //		Model.Stu_ZZXS0101 studentInfo = new Model.Stu_ZZXS0101 { XSID = Guid.NewGuid() };
            //		// ll
            //		#region init attributes
            //		//登录帐号 
            //		//studentInfo.LoginName = dt.Rows[i]["LoginName"].ToString().Trim(); 
            //		studentInfo.XXID = XXID;

            //		#region 班级 进班年份:NJDM 学段：ZYDM
            //		string ZYDM = GetZYDM(dt.Rows[i]["ZYDM"].ToString().Trim()).ToString();
            //		if (ZYDM == "-1") {
            //			//errorstrb.AppendFormat("学段不正确(无、小学段、初中段、高中段)；");
            //			dt.Rows[i]["ValidMsg"] = "学段不正确(无、小学段、初中段、高中段)；";
            //			dt.Rows[i]["BJID"] = Guid.Empty.ToString();
            //			continue;
            //		}

            //		string name = dt.Rows[i]["BJMC"].ToString().Trim();
            //		string name1 = dt.Rows[i]["NJDM"].ToString().Trim();
            //		if (bjList.Count(s => s.JBNY == name1 && s.XZBMC.Equals(name) && s.ZYDM == ZYDM && s.XXID == xxid) == 1) {
            //			Guid bjid = bjList.First(e => e.JBNY == name1 && e.XZBMC.Equals(name) && e.ZYDM == ZYDM).BJID;
            //			studentInfo.BJID = bjid;
            //			dt.Rows[i]["BJID"] = bjid;
            //		}
            //		else {
            //			//errorstrb.AppendFormat("班级不存在或存在多个相同属性班级；");
            //			dt.Rows[i]["BJID"] = Guid.Empty.ToString();
            //			dt.Rows[i]["ValidMsg"] = "班级不存在或存在多个相同名称的班级；";
            //			continue;
            //		}

            //		#endregion
            //		#region 学籍号 地区学号

            //		studentInfo.XJH = dt.Rows[i]["XJH"].ToString().Trim();
            //		var xjhm = dt.Rows[i]["XJH"].ToString().Trim();
            //		//读取缓存学生数据
            //		if (sdb.Stu_ZZXS0101.Count(x => x.XJH == xjhm) > 0 && isCover.ToLower() == "false") {
            //			//errorstrb.AppendFormat("学籍号重复；");
            //			dt.Rows[i]["ValidMsg"] = "学籍号重复；";
            //			continue;
            //		}

            //		studentInfo.XH = dt.Rows[i]["XH"].ToString().Trim();

            //		#endregion
            //		#region 姓名
            //		studentInfo.XM = dt.Rows[i]["XM"].ToString().Trim();
            //		#endregion
            //		#region 身份证号
            //		studentInfo.SFZJH = dt.Rows[i]["SFZJH"].ToString().Trim();
            //		#endregion

            //		string sfzjh = dt.Rows[i]["SFZJH"].ToString().Trim();
            //		string xm = dt.Rows[i]["XM"].ToString().Trim();
            //		Model.Stu_ZZXS0101 FindStu = sdb.Stu_ZZXS0101.SingleOrDefault(x => x.SFZJH == sfzjh && x.XM == xm);
            //		if (FindStu != null) {
            //			Guid xsid = FindStu.XSID;
            //			studentInfo.XSID = xsid;
            //			if (isCover.ToLower() == "false") {
            //				//errorstrb.AppendFormat("该学生已存在！");
            //				dt.Rows[i]["ValidMsg"] = "该学生已存在；";
            //				continue;
            //			}
            //		}

            //		#region 出生日期--获取身份证号的7-13位

            //		var csrqStr = !string.IsNullOrEmpty(sfzjh) ? sfzjh.Substring(6, 8) : "";
            //		studentInfo.CSRQ = csrqStr;

            //		#endregion
            //		#region 性别
            //		string xb = !string.IsNullOrEmpty(sfzjh) && sfzjh.Length == 18 ? sfzjh.Substring(16, 1) : sfzjh.Substring(14, 1);
            //		studentInfo.XBM = int.Parse(xb) % 2 == 1 ? "1" : "2";
            //		#endregion
            //		#endregion
            stulist = new List<SIM.StuInfo>();
            stulist.Add(new SIM.StuInfo());
			//		dt.Rows[i]["ValidMsg"] = "验证通过";
			//	}
			//	catch (Exception e) {
			//		dt.Rows[i]["ValidMsg"] = "系统存在多个相同身份证的学生帐号！";
			//	}

			//}
			//Session["ImportStuList"] = dt;
			//#endregion
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing) {
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
