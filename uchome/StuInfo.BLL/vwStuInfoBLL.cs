using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using sim = StuInfo.Model;
namespace StuInfo.BLL {
	/// <summary>
	/// 学校信息管理
	/// </summary>
	public partial class vwStuInfoBLL {
		public sim.vwStuInfo GetStuInfo(Guid stuid, bool bstate = true) {
			using (var db = new sim.StuInfoEntities()) {
				if (bstate)
					return db.vwStuInfo.SingleOrDefault(si => si.StuID == stuid && si.IsState == 1);
				else
					return db.vwStuInfo.SingleOrDefault(si => si.StuID == stuid);
			}
		}
		/// <summary>
		/// 获取年级学生
		/// </summary>
		/// <param name="schid"></param>
		/// <param name="bjids"></param>
		/// <param name="stunumber"></param>
		/// <param name="stucode"></param>
		/// <param name="stuidentity"></param>
		/// <param name="stuname"></param>
		/// <param name="maxrecord"></param>
		/// <returns></returns>
		public List<sim.vwStuInfo> GetStuList(Guid schid, List<Guid> bjids, string stunumber, string stucode, string stuidentity, string stuname, int maxrecord = 1000, bool bstate = true) {
			using (var db = new sim.StuInfoEntities()) {
				if (bstate)
					return db.vwStuInfo.Where(si => si.StuID_SchID == schid && bjids.Contains(si.StuID_BJID.Value) && si.IsState == 1 && si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber)).OrderByDescending(si => si.StuID_BJID).ThenBy(si => si.StuNumber).Take(maxrecord).ToList();
				else
					return db.vwStuInfo.Where(si => si.StuID_SchID == schid && bjids.Contains(si.StuID_BJID.Value) && si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber)).OrderByDescending(si => si.StuID_BJID).ThenBy(si => si.StuNumber).Take(maxrecord).ToList();
			}
		}
		public int GetStuCount(Guid schid, List<Guid> bjids, string stunumber, string stucode, string stuidentity, string stuname, bool bstate = true) {
			using (var db = new sim.StuInfoEntities()) {
				if (bstate)
					return db.vwStuInfo.Count(si => si.StuID_SchID == schid && bjids.Contains(si.StuID_BJID.Value) && si.IsState == 1 && si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber));
				else
					return db.vwStuInfo.Count(si => si.StuID_SchID == schid && bjids.Contains(si.StuID_BJID.Value) && si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber));
			}
		}
		/// <summary>
		/// 获取班级学生
		/// </summary>
		/// <param name="schid"></param>
		/// <param name="bjid"></param>
		/// <param name="stunumber"></param>
		/// <param name="stucode"></param>
		/// <param name="stuidentity"></param>
		/// <param name="stuname"></param>
		/// <param name="maxrecord"></param>
		/// <returns></returns>
		public List<sim.vwStuInfo> GetStuList(Guid schid, Guid bjid, string stunumber, string stucode, string stuidentity, string stuname, int maxrecord = 1000, bool bstate = true) {
			using (var db = new sim.StuInfoEntities()) {
				if (bstate)
					return db.vwStuInfo.Where(si => si.StuID_SchID == schid && si.StuID_BJID == bjid && si.IsState == 1 && si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber)).OrderBy(si => si.StuNumber).Take(maxrecord).ToList();
				else
					return db.vwStuInfo.Where(si => si.StuID_SchID == schid && si.StuID_BJID == bjid && si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber)).OrderBy(si => si.StuNumber).Take(maxrecord).ToList();
			}
		}
		public int GetStuCount(Guid schid, Guid bjid, string stunumber, string stucode, string stuidentity, string stuname, bool bstate = true) {
			using (var db = new sim.StuInfoEntities()) {
				if (bstate)
					return db.vwStuInfo.Count(si => si.StuID_SchID == schid && si.StuID_BJID == bjid && si.IsState == 1 && si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber));
				else
					return db.vwStuInfo.Count(si => si.StuID_SchID == schid && si.StuID_BJID == bjid && si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber));
			}
		}
		/// <summary>
		/// 模糊并列查询
		/// </summary>
		/// <param name="stunumber">学号</param>
		/// <param name="stucode">学籍号</param>
		/// <param name="stuidentity">身份证号</param>
		/// <param name="stuname">姓名</param>
		/// <returns></returns>
		public List<sim.vwStuInfo> GetStuList2(string stunumber, string stucode, string stuidentity, string stuname, int maxrecord = 1000, bool bstate = true) {
			using (var db = new sim.StuInfoEntities()) {
				if (bstate)
					return db.vwStuInfo.Where(si => si.IsState == 1 && si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber)).OrderByDescending(si => si.StuID_BJID).ThenBy(si => si.StuNumber).Take(maxrecord).ToList();
				else
					return db.vwStuInfo.Where(si => si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber)).OrderByDescending(si => si.StuID_BJID).ThenBy(si => si.StuNumber).Take(maxrecord).ToList();
			}
		}
		public int GetStuCount2(string stunumber, string stucode, string stuidentity, string stuname, bool bstate = true) {
			using (var db = new sim.StuInfoEntities()) {
				if (bstate)
					return db.vwStuInfo.Count(si => si.IsState == 1 && si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber));
				else
					return db.vwStuInfo.Count(si => si.StuName.Contains(stuname) && si.StuCode.Contains(stucode) && si.StuIdentity.Contains(stuidentity) && si.StuNumber.Contains(stunumber));
			}
		}
		/// <summary>
		/// 模糊查询
		/// </summary>
		/// <param name="stunumber">学号</param>
		/// <param name="stucode">学籍号</param>
		/// <param name="stuidentity">身份证号</param>
		/// <param name="stuname">姓名</param>
		/// <returns></returns>
		public List<sim.vwStuInfo> GetStuList(string stunumber, string stucode, string stuidentity, string stuname, int maxrecord = 1000, bool bstate = true) {
			using (var db = new sim.StuInfoEntities()) {
				if (bstate)
					return db.vwStuInfo.Where(si => si.IsState == 1 && si.StuName.Contains(stuname) || si.StuCode.Contains(stucode) || si.StuIdentity.Contains(stuidentity) || si.StuNumber.Contains(stunumber)).OrderByDescending(si => si.StuID_BJID).ThenBy(si => si.StuNumber).Take(maxrecord).ToList();
				else
					return db.vwStuInfo.Where(si => si.StuName.Contains(stuname) || si.StuCode.Contains(stucode) || si.StuIdentity.Contains(stuidentity) || si.StuNumber.Contains(stunumber)).OrderByDescending(si => si.StuID_BJID).ThenBy(si => si.StuNumber).Take(maxrecord).ToList();
			}
		}
		public int GetStuCount(string stunumber, string stucode, string stuidentity, string stuname, bool bstate = true) {
			using (var db = new sim.StuInfoEntities()) {
				if (bstate)
					return db.vwStuInfo.Count(si => si.IsState == 1 && si.StuName.Contains(stuname) || si.StuCode.Contains(stucode) || si.StuIdentity.Contains(stuidentity) || si.StuNumber.Contains(stunumber));
				else
					return db.vwStuInfo.Count(si => si.StuName.Contains(stuname) || si.StuCode.Contains(stucode) || si.StuIdentity.Contains(stuidentity) || si.StuNumber.Contains(stunumber));
			}
		}
		/// <summary>
		/// 开放式条件查询学生信息
		/// </summary>
		/// <param name="filterexpression"></param>
		/// <returns></returns>
		public List<sim.vwStuInfo> GetStuList(Expression<Func<sim.vwStuInfo, bool>> filterexpression, int top = 1000) {
			using (var db = new sim.StuInfoEntities()) {
				return db.vwStuInfo.Where(filterexpression).OrderByDescending(si => si.StuID_BJID).ThenBy(si => si.StuNumber).Take(top).ToList();
			}
		}
		public int GetStuCount(Expression<Func<sim.vwStuInfo, bool>> filterexpression) {
			using (var db = new sim.StuInfoEntities()) {
				return db.vwStuInfo.Count(filterexpression);
			}
		}
	}
}
