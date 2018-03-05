using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Mvc;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class RolePermissionsController : Controller
    {
        UCHomeEntities db = new UCHomeEntities();
        //
        // GET: /RolePermissions/
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private static UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        private Guid xxid
        {
            get { return user.xxid; }
        }

        public PartialViewResult RolePermissions()
        {
            return PartialView();
        }

        public PartialViewResult PersonPermissionsSet()
        {
            ViewBag.teacherList = db.View_Simple_TeaInfo.Where(x => x.xxid == xxid).ToList();
            ViewBag.studentList = db.View_Simple_StuInfo.Where(x => x.xxid == xxid).ToList();
            return PartialView();
        }

        public JsonResult getRolebyId(Guid id, string type)
        {
            var roleList = db.UCHome_AppRole.Where(x => x.AppFuncRole == type).ToList();
            var roleItem = new List<SelectListItem>();
            roleItem = roleList.Select(x => new SelectListItem()
            {

                Text = x.RoleName,
                Value = x.RoleId.ToString(),
                Selected = db.UCHome_UserRole_Relation.Where(p => p.RoleId == x.RoleId && p.UserId == id).Count() > 0
            }).ToList();

            return Json(roleItem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetpersonPro(Guid userid, Guid roleId, string username)
        {
            var serperPro = db.UCHome_UserRole_Relation.FirstOrDefault(x => x.UserId == userid && x.RoleId == roleId);
            if (serperPro != null)
            {
                db.UCHome_UserRole_Relation.DeleteObject(serperPro);
            }
            else
            {
                UCHome_UserRole_Relation userRole = new UCHome_UserRole_Relation();
                userRole.RoleId = roleId;
                userRole.UserId = userid;
                userRole.UserName = username;
                db.UCHome_UserRole_Relation.AddObject(userRole);
            }
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetOurSchRListbyRId(Guid roleId, string roleType)
        {
            var result = new List<SelectListItem>();
            switch (roleType)
            {
                case "TP":
                       result = (from a in db.UCHome_UserRole_Relation
                        join b in db.View_Simple_TeaInfo.Where(x => x.xxid == xxid) on a.UserId equals b.jsid
                        where (a.RoleId == roleId)
                        select a).ToList().Select(x=>new SelectListItem()
                        {
                            Value = x.UserId.ToString(),
                            Text = x.UserName
                        }).ToList();
                    break;
                case "SP":
                    result = (from a in db.UCHome_UserRole_Relation
                        join b in db.View_Simple_StuInfo.Where(x => x.xxid == xxid) on a.UserId equals b.xsid
                        where (a.RoleId == roleId)
                        select a).ToList().Select(x => new SelectListItem()
                        {
                            Value = x.UserId.ToString(),
                            Text = x.UserName.ToString()
                        }).ToList();
                    break;
                case "PP":
                    result = (from a in db.UCHome_UserRole_Relation
                        join b in db.View_Simple_Parents.Where(x => x.xxid == xxid) on a.UserId equals b.jzid
                        where (a.RoleId == roleId)
                        select a).ToList().Select(x => new SelectListItem()
                        {
                            Value = x.UserId.ToString(),
                            Text = x.UserName.ToString()
                        }).ToList();
                    break;
                case "XP":
                    result = (from a in db.UCHome_UserRole_Relation
                        join b in db.View_Simple_TeaInfo.Where(x => x.xxid == xxid) on a.UserId equals b.jsid
                        where (a.RoleId == roleId)
                        select a).ToList().Select(x => new SelectListItem()
                        {
                            Value = x.UserId.ToString(),
                            Text = x.UserName.ToString()
                        }).ToList();
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
