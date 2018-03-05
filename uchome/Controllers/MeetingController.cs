using MeetingBLL;
using MeetingModel;
using OA.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class MeetingController : Controller
    {
        MeetingService msl = new MeetingService();
        private readonly UCHomeBasePage page = new UCHomeBasePage();
        ZZ_WSMeetingEntities db = new ZZ_WSMeetingEntities();
        ZZ_MIFVSEntities entity = new ZZ_MIFVSEntities();
        #region 属性
        private static UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        private Guid userid
        {
            get { return user.userid; }
        }
        private string userName
        {
            get { return user.loginname; }
        }
        private Guid XXID
        {
            get { return user.xxid; }
        }
        private string DisPlayName
        {
            get { return user.username; }
        }
        #endregion
        //获取审核人
        public JsonResult GetApprovePeopleList()
        {
            IEnumerable<SelectListItem> list = msl.GetDeptApproveList(XXID).Select(p => new SelectListItem()
            {
                Text = p.XM,
                Value = p.DriverID.ToString()
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //获取会议室名称
        public JsonResult GetMeetingRoomList()
        {
            IEnumerable<SelectListItem> list = msl.GetMeetingRoomList(XXID).Select(p => new SelectListItem()
            {
                Text = p.MeetingRoomName,
                Value = p.ID.ToString()
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //申请初始化页面
        public ActionResult MeetingApply()
        {
            ApplyMeeting app = new ApplyMeeting();
            app.ID = Guid.NewGuid();
            return View(app);
        }
        //发送会议申请
        [HttpPost]
        public JsonResult MeetingApply(ApplyMeeting model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var roomid = Request["MeetingRoom"];
                    Guid _Roomid = Guid.Parse(roomid);
                    var apid = Request["ApproveId"];
                    Guid approveid = Guid.Parse(apid);
                    model.ID = Guid.NewGuid();
                    model.ApplierName = DisPlayName;
                    model.CreatorName = userid.ToString();
                    model.MeetingRoomID = _Roomid;
                    model.MeetingRoom = db.MeetingRoom.FirstOrDefault(x => x.ID == _Roomid).MeetingRoomName;
                    model.Department = entity.View_TeacherContactInfo.FirstOrDefault(x => x.JSID == userid).JGMC;
                    model.ApplyDate = DateTime.Now;
                    model.Status = "申请中";
                    model.XXID = XXID;
                    model.CreateTime = DateTime.Now;
                    var receiveIds = Request["selectUserID"];
                    var receiveNames = Request["selectUserName"];
                    model.AttenderID = receiveIds;
                    model.AttenderDisplayName = receiveNames;
                    model.ApproverId = approveid;
                    db.ApplyMeeting.AddObject(model);
                    db.SaveChanges();
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
        //会议申请编辑
        public ActionResult EditMeeting()
        {
            Guid iD = Guid.Parse(Request["ID"]);
            var model = msl.GetMeetingEdit(iD);
            ViewBag.approver = model.ApproverId;
            ViewBag.meetingroom = model.MeetingRoom;
            ViewBag.ids = model.AttenderID;
            ViewBag.names = model.AttenderDisplayName;
            return View(model);
        }
        [HttpPost]
        public JsonResult EditMeeting(ApplyMeeting model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var roomid = Request["MeetingRoom"];
                    Guid _Roomid = Guid.Parse(roomid);
                    var apid = Request["ApproveId"];
                    Guid approveid = Guid.Parse(apid);
                    model.ID = Guid.NewGuid();
                    model.ApplierName = DisPlayName;
                    model.CreatorName = userid.ToString();
                    model.MeetingRoomID = _Roomid;
                    model.MeetingRoom = db.MeetingRoom.FirstOrDefault(x => x.ID == _Roomid).MeetingRoomName;
                    model.Department = entity.View_TeacherContactInfo.FirstOrDefault(x => x.JSID == userid).JGMC;
                    model.ApplyDate = DateTime.Now;
                    model.Status = "申请中";
                    model.XXID = XXID;
                    model.CreateTime = DateTime.Now;
                    var receiveIds = Request["selectUserID"];
                    var receiveNames = Request["selectUserName"];
                    model.AttenderID = receiveIds;
                    model.AttenderDisplayName = receiveNames;
                    model.ApproverId = approveid;
                    db.ApplyMeeting.Attach(model);
                    db.ObjectStateManager.ChangeObjectState(model, EntityState.Modified);
                    db.SaveChanges();
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
        //我的会议申请
        public ActionResult MyMeeting()
        {
            var jsid = userid.ToString();
            var list = msl.GetMyMeetingList(jsid);
            return View(list);
        }
        //我的审核
        public ActionResult MeetingAudit()
        {
            var list = msl.GetMyAuditMeetingList(userid);
            return View(list);
        }

        //会议申请详情
        public ActionResult Detials()
        {
            var id = Request["ID"];

            var list = msl.GetDetials(Guid.Parse(id));
            Guid jsid = Guid.Parse(list.ApproverId.ToString());
            ViewBag.Approver = msl.GetTeacherGH(jsid).DisName;
            return View(list);
        }
        //会议审核页面
        public ActionResult AuditMeeting()
        {
            var id = Request["ID"];
            ViewBag.id = id;
            var list = msl.GetAuditDetials(Guid.Parse(id));
            Guid jsid = Guid.Parse(list.ApproverId.ToString());
            ViewBag.Approver = msl.GetTeacherGH(jsid).DisName;
            return View(list);
        }
        //检查会议时间
        public JsonResult CheckTime(DateTime startdate, DateTime enddate, Guid meetroom)
        {
            string code = "1";
            string msg = "";
            var meetinfo = db.ApplyMeeting.Where(x => x.Status != "审核不通过"&&x.MeetingRoomID==meetroom).ToList().OrderByDescending(x => x.CreateTime);          
            foreach (var item in meetinfo)
            {
                if (enddate <= item.StartTime)
                {
                    code = "1";
                }
                else if (startdate >= item.EndTime)
                {
                    code = "1";
                }
                else
                {
                    code = "0";
                    msg = item.StartTime + "至" + item.EndTime;
                    break;
                }
            }
            JsonResult rlt = new JsonResult { Data = new { code, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
        //检查会议时间
        public JsonResult CheckEditTime(Guid ID, DateTime startdate, DateTime enddate, Guid meetroom)
        {
            string code = "1";
            string msg = "";
            var meetinfo = db.ApplyMeeting.Where(x => x.Status != "审核不通过" && x.MeetingRoomID == meetroom&&x.ID!=ID).ToList().OrderByDescending(x => x.CreateTime);
            foreach (var item in meetinfo)
            {
                if (enddate <= item.StartTime)
                {
                    code = "1";
                }
                else if (startdate >= item.EndTime)
                {
                    code = "1";
                }
                else
                {
                    code = "0";
                    msg = item.StartTime + "至" + item.EndTime;
                    break;
                }
            }
            JsonResult rlt = new JsonResult { Data = new { code, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
        //审核通过不通过
        public JsonResult NoPass(string Id, string approveComment)
        {
            Guid id = new Guid(Id);
            string code = "1";
            string msg = "";
            try
            {
                msl.NoPass(id, approveComment);
            }
            catch (Exception)
            {
                code = "0";
            }

            JsonResult rlt = new JsonResult { Data = new { code, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Pass(string Id, string approveComment)
        {
            Guid id = new Guid(Id);
            string code = "1";
            string msg = "";
            try
            {
                msl.Pass(id, approveComment);
            }
            catch (Exception)
            {
                code = "0";
            }

            JsonResult rlt = new JsonResult { Data = new { code, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
        //删除我的会议申请
        public JsonResult DeleteMeeting(Guid Id)
        {
            var code = "1";

            code = msl.DeleteMeeting(Id);

            return Json(code, JsonRequestBehavior.AllowGet);
        }
    }
}