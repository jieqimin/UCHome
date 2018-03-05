using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCHome.BLL;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class FamilySchInteractionController : Controller
    {
        private readonly UCHomeBasePage ucpage = new UCHomeBasePage();
        private readonly FamilySchServices services = new FamilySchServices();
        private static UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        private Guid userId
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
        private string userName
        {
            get
            {
                return user.username;
            }
        }

        #region message
        public ActionResult MsgSend()
        {
            UCHome_Msg msg = new UCHome_Msg();
            msg.MsgID = Guid.NewGuid();
            return PartialView(msg);
        }
        public ActionResult FamMsgSend()
        {
            //获取当前亲子的教师名单
            Guid bjid = user.childinfo.childbjid;
            SelectUserServices userservice = new SelectUserServices();
            string xnxq = ucpage.CurrentSchTerm;
            var list = userservice.GetMyTeacherList(bjid, xnxq);
            return PartialView(list);
        }
        [HttpPost]
        public JsonResult FamMsgSend(Guid receiveUserId, string fammsgcontent, string receiveUsername)
        {
            string statuscode = "200";
            string msg = "";

            UCHome_Msg model = new UCHome_Msg();
            model.MsgID = Guid.NewGuid();
            model.MsgDate = DateTime.Now;
            model.MsgUserID = userId;
            model.MsgUserName = userName;
            model.MsgContent = HttpUtility.UrlDecode(fammsgcontent);
            var receiveIds = receiveUserId.ToString();
            var receiveNames = HttpUtility.UrlDecode(receiveUsername);
            try
            {
                services.SendMsg(model, receiveIds, receiveNames);
            }
            catch (Exception ex)
            {
                statuscode = "500";
                msg = ex.ToString();
            }


            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult MsgSend(UCHome_Msg model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                model.MsgDate = DateTime.Now;
                model.MsgUserID = userId;
                model.MsgUserName = userName;
                var receiveIds = Request["selectUserID"];
                var receiveNames = Request["selectUserName"];

                try
                {
                    services.SendMsg(model, receiveIds, receiveNames);
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

        public ActionResult MsgQuery()
        {
            var list = services.GetMsgList(userId);
            return PartialView(list);
        }

        public ActionResult MsgList()
        {
            var receiveUserId = Request["receiveUserId"];

            var list = services.GetMsg_ReceiveList(userId, Guid.Parse(receiveUserId));
            return PartialView(list);
        }

        public ActionResult MsgDetail()
        {
            var receiveUserId = Request["receiveUserId"];

            var list = services.GetMsg_ReceiveList(userId, Guid.Parse(receiveUserId));
            return PartialView(list);
        }

        public JsonResult DeleteMsgReceiver(Guid pkId)
        {
            var code = "1";
            try
            {
                services.DeleteMsgReceiver(pkId);
            }
            catch (Exception)
            {
                code = "0";
            }

            return Json(code, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region notice

        public JsonResult GetNoticeTypeList()
        {
            IEnumerable<SelectListItem> list = services.GetNoticeTypeList().Select(p => new SelectListItem
            {
                Text = p.NoticeTypeName,
                Value = p.NoticeTypeCode
            });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NoticeSend()
        {
            UCHome_Notice notice = new UCHome_Notice();
            notice.NoticeID = Guid.NewGuid();
            return PartialView(notice);
        }

        [HttpPost]
        public JsonResult NoticeSend(UCHome_Notice model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                model.NoticeDate = DateTime.Now;
                model.NoticeUserID = userId;
                model.NoticeUserName = userName;
                var receiveIds = Request["selectUserID"];
                var receiveNames = Request["selectUserName"];
                if (model.NoticeTypeCode == "1")
                {
                    receiveIds = user.xxid.ToString();
                    receiveNames = user.xxmc;
                }

                try
                {
                    services.NoticeSend(model, receiveIds, receiveNames);
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

        public ActionResult NoticeQuery()
        {
            var list = services.GetNoticeList(userId);
            return PartialView(list);
        }
        public ActionResult ReceiveNotice()
        {
            var list = services.GetReceiveNoticeList(userId, user.childinfo.childxxid);
            return PartialView(list);
        }
        public JsonResult DeleteNotice(Guid noticeId)
        {
            var code = "1";
            try
            {
                services.DeleteNotice(noticeId);
            }
            catch (Exception)
            {
                code = "0";
            }

            return Json(code, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NoticeDetail(Guid noticeID)
        {
            var list = services.GetNotice(noticeID);
            return PartialView(list);
        }
        public ActionResult NoticeDetailByFam(Guid noticeID)
        {
            var list = services.GetNotice(noticeID);
            return PartialView(list);
        }

        public JsonResult updatereadstatus(Guid noticeID)
        {
            int result = services.ChangeReadStatus(noticeID, userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NoticeDel()
        {
            var noticeID = Request["noticeID"];

            var list = services.GetNotice(Guid.Parse(noticeID));
            return PartialView(list);
        }

        public JsonResult DeleteNoticeReceiver(Guid receiveUserId, Guid noticeId)
        {
            var code = "1";
            try
            {
                services.DeleteNoticeReceiver(receiveUserId, noticeId);
            }
            catch (Exception)
            {
                code = "0";
            }

            return Json(code, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
