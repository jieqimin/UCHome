using MessageCenter.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using MessageCenter.Model;
using System.Web.UI.WebControls;
using UCHome.Model;
using OA.Model;
namespace UCHome.Controllers
{
    public class MessageCenterController : Controller
    {
        MessageBll msl = new MessageBll();

        ZZ_MessageCenterEntities db = new ZZ_MessageCenterEntities();
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        private Guid userid
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
        #region   个人通知  zy
        public ActionResult EditNotice(Guid EditID)
        {
            var temp = db.Msg_Notice.FirstOrDefault(x => x.ID == EditID);
            if (temp.MsgTypeName == "学校通知")
            {
                var notice = msl.Msg_Notice(EditID);
                if (!string.IsNullOrEmpty(notice.FileID))
                {
                    var files = notice.FileID.Split(',');
                    var filepaths = notice.FileUrl.Split(',');
                    string split = string.Empty;
                    string filestr = string.Empty;
                    for (int i = 0; i < files.Length; i++)
                    {
                        filestr += string.Format("{3}{{ \"fileid\":\"{0}\",\"filename\":\"{1}\",\"filepath\":\"{2}\"}}", notice.ID, files[i], filepaths[i], split);
                        split = ",";
                    }
                    filestr = "[" + filestr + "]";
                    //if (ids != "")
                    //{
                    //    ids = ids.Remove(ids.Length - 1);
                    //    names = names.Remove(names.Length - 1);
                    //}

                    //ViewBag.ids = ids;
                    //ViewBag.names = names;
                    ViewBag.files = filestr;
                    ViewBag.type = notice.MsgTypeCode;
                }
                var result = db.msg_NoticeType.Select(p => new { p.typeName, p.typeId }).ToList();
                return View(notice);
            }
            else {
            var notice = msl.Msg_Notice(EditID);
            var people = msl.Msg_ReciveNotice(EditID);
            var ids = "";
            var names = "";
            foreach (var item in people)
            {
                ids += item.ReceiveUserID + ",";
                names += item.ReceiveUserName + ",";

            }
            if (!string.IsNullOrEmpty(notice.FileID))
            {
                var files = notice.FileID.Split(',');
                var filepaths = notice.FileUrl.Split(',');
                string split = string.Empty;
                string filestr = string.Empty;
                for (int i = 0; i < files.Length; i++)
                {
                    filestr += string.Format("{3}{{ \"fileid\":\"{0}\",\"filename\":\"{1}\",\"filepath\":\"{2}\"}}",
                        notice.ID, files[i], filepaths[i], split);
                    split = ",";
                }
                filestr = "[" + filestr + "]";

                if (ids != "")
                {
                    ids = ids.Remove(ids.Length - 1);
                    names = names.Remove(names.Length - 1);
                }

                ViewBag.ids = ids;
                ViewBag.names = names;
                ViewBag.files = filestr;
                ViewBag.type = notice.MsgTypeCode;
            }
            var result = db.msg_NoticeType.Select(p => new { p.typeName, p.typeId }).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in result)
            {
                SelectListItem listItem = new SelectListItem();
                listItem.Value = item.typeId.ToString();
                listItem.Text = item.typeName;
                listItem.Selected = item.typeId == int.Parse(notice.MsgTypeCode);
                list.Add(listItem);
            }
            ViewBag.MsgTypeCode = list;

            return View(notice);
            
        }

           
        }
        public ActionResult NoticeSend()
        {
            Msg_Notice notice = new Msg_Notice();

            var result = db.msg_NoticeType.Select(p => new { p.typeName, p.typeId }).Distinct().ToList();

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in result)
            {
                SelectListItem listItem = new SelectListItem();
                listItem.Value = item.typeId.ToString();
                listItem.Text = item.typeName;
                list.Add(listItem);
            }
            ViewBag.MsgTypeCode = list;

            notice.ID = Guid.NewGuid();
            return View(notice);
        }
        public ActionResult NoticeQuery()
        {
            var list = msl.GetMsgList(userid);

            return View(list);
        }
        public ActionResult ReciveQuery()
        {

            var list = msl.GetMsgReciveList1(userid);
            return View(list);
        }

      
        public PartialViewResult Message()
        {
            var count = msl.MessageCount(userid);
            ViewBag.NoticeCount = count;
            return PartialView("Message");
        }
        public ActionResult NoticeDetail(Guid ID)
        {
            var noticeID = Request["ID"];
            var list = msl.GetMsg(Guid.Parse(noticeID));

            ViewBag.MsgSendUserName = db.Msg_Notice.FirstOrDefault(x => x.ID == ID).MsgSendUserName;
            //ViewBag.MsgSendUserName = db.Msg_Notice.FirstOrDefault(x => x.ID ==Guid.Parse(noticeID)).MsgSendUserName;

            return View(list);
        }
        public ActionResult ReciveDetail(Guid ReciveID)
        {
            //// var noticeID = Request["ID"];
            var model = db.Msg_Notice.FirstOrDefault(x => x.ID == ReciveID);
            if (model.MsgTypeName == "学校通知")
            {
                var list = msl.GetReciveMsg1(ReciveID, userid);
                return View(list);

            }
            else {
            var list = msl.GetReciveMsg(ReciveID, userid);
            return View(list);
        }
           
        }
       


        public JsonResult DeleteNotice(Guid noticeId)
        {
            var model = db.Msg_Notice.FirstOrDefault(x => x.ID == noticeId);
            if (model.MsgTypeName == "学校通知")
            {
                model.Status = "已删除";
                db.SaveChanges();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else {
            var code = "1";
            try
            {
                msl.DeleteNotice(noticeId);
            }
            catch (Exception)
            {
                code = "0";
            }

            return Json(code, JsonRequestBehavior.AllowGet);
        }
           
        }
        public JsonResult DeleteReciveNotice(Guid noticeId)
        {
            

            var model = db.Msg_Notice.FirstOrDefault(x => x.ID == noticeId);
            if (model.MsgTypeName == "学校通知")
            {
                model.Status = "已删除";
                db.SaveChanges();

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else {
            var code = "1";
            try
            {
                msl.DeleteReciveNotice(noticeId);
            }
            catch (Exception)
            {
                code = "0";
            }

            return Json(code, JsonRequestBehavior.AllowGet);
        }

            
        }

        [HttpPost]
        public JsonResult NoticeSend(Msg_Notice model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                int type = Convert.ToInt32(Request["MsgTypeCode"]);
                model.MsgDate = DateTime.Now;
                model.XXID = schoolId;
                model.MsgSendID = userid;
                model.MsgSendLoginName = loginName;
                model.MsgSendUserName = userName;
                model.MsgTypeCode = Request["MsgTypeCode"];
                model.MsgTitle = Request["MsgTitle"];
                model.MsgContent = Request["MsgContent"];
                model.MsgTypeName = db.msg_NoticeType.FirstOrDefault(x => x.typeId == type).typeName;
                //model.MsgTypeName = db.msg_NoticeType.FirstOrDefault(x => x.XXID == model.XXID && x.typeId == type).typeName;
                model.Status = "已发送";
                model.MsgType = "通知公告";
                model.FileUrl = Request["FileUrl"];
                model.FileID = Request["FileID"];
                var receiveIds = Request["selectUserID"];
                var receiveNames = Request["selectUserName"];

                try
                {
                    msl.NoticeSend(model, receiveIds, receiveNames);
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
        [HttpPost]
        public JsonResult EditNotice(Msg_Notice model)
        {
            
            var temp=db.Msg_Notice.FirstOrDefault(x=>x.ID==model.ID);
            if (temp.MsgTypeName == "学校通知")
            {
                string statuscode = "200";
                string msg = "";
                if (ModelState.IsValid)
                {
                    int type = Convert.ToInt32(Request["MsgTypeCode"]);
                    temp.MsgDate = DateTime.Now;
                    temp.XXID = schoolId;
                    temp.MsgSendID = userid;
                    temp.MsgSendLoginName = loginName;
                    temp.MsgSendUserName = userName;
                    temp.MsgTypeCode = Request["MsgTypeCode"];
                    temp.MsgTitle = Request["MsgTitle"];
                    temp.MsgContent = Request["MsgContent"];
                    temp.MsgTypeName = "学校通知";
                    //model.MsgTypeName = db.msg_NoticeType.FirstOrDefault(x => x.XXID == model.XXID && x.typeId == type).typeName;
                    temp.Status = "已发送";
                    temp.MsgType = "通知公告";
                    temp.FileUrl = Request["FileUrl"];
                    temp.FileID = Request["FileID"];
                    var receiveIds = Request["selectUserID"];
                    var receiveNames = Request["selectUserName"];

                    if (receiveIds != null && receiveNames != null)
                    {
                        msl.EditNotice(model, receiveIds, receiveNames);
                    }
                    else
                    {
                        db.SaveChanges();
                    }
                    
                }
                JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
                return Json(rlt, JsonRequestBehavior.AllowGet);
            }
            else {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                int type = Convert.ToInt32(Request["MsgTypeCode"]);
                model.MsgDate = DateTime.Now;
                model.XXID = schoolId;
                model.MsgSendID = userid;
                model.MsgSendLoginName = loginName;
                model.MsgSendUserName = userName;
                model.MsgTypeCode = Request["MsgTypeCode"];
                model.MsgTitle = Request["MsgTitle"];
                model.MsgContent = Request["MsgContent"];
                model.MsgTypeName = db.msg_NoticeType.FirstOrDefault(x => x.typeId == type).typeName;
                //model.MsgTypeName = db.msg_NoticeType.FirstOrDefault(x => x.XXID == model.XXID && x.typeId == type).typeName;
                model.Status = "已发送";
                model.MsgType = "通知公告";
                model.FileUrl = Request["FileUrl"];
                model.FileID = Request["FileID"];
                var receiveIds = Request["selectUserID"];
                var receiveNames = Request["selectUserName"];
                //try
                //{

                msl.EditNotice(model, receiveIds, receiveNames);
                //}
                //catch (Exception ex)
                //{
                //    statuscode = "500";
                //    msg = ex.ToString();
                //}
            }

            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }

         
        }
        public JsonResult GetNoticeTypeList()
        {
            var xxid = schoolId.ToString();
            IEnumerable<SelectListItem> list = msl.GetNoticeTypeList(xxid).Select(p => new SelectListItem()
            {
                Text = p.typeName,
                Value = p.typeId.ToString()
            });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  学校通知  zy
        public ActionResult NoticeSchoolSend()
        {
            Msg_Notice notice = new Msg_Notice();
         
            notice.ID = Guid.NewGuid();
            return View(notice);
        }
        [HttpPost]
        //新建
        public JsonResult NoticeSchoolSend(Msg_Notice model)
        {
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                int type = Convert.ToInt32(Request["MsgTypeCode"]);
                model.MsgDate = DateTime.Now;
                model.XXID = schoolId;
                model.MsgSendID = userid;
                model.MsgSendLoginName = loginName;
                model.MsgSendUserName = userName;
                model.MsgTypeCode = Request["MsgTypeCode"];
                model.MsgTitle = Request["MsgTitle"];
                model.MsgContent = Request["MsgContent"];
               // model.MsgTypeName = db.msg_NoticeType.FirstOrDefault(x => x.typeId == type).typeName;
                //model.MsgTypeName = db.msg_NoticeType.FirstOrDefault(x => x.XXID == model.XXID && x.typeId == type).typeName;
                model.Status = "已发送";
                model.MsgType = "通知公告";
                model.FileUrl = Request["FileUrl"];
                model.FileID = Request["FileID"];
                model.MsgTypeName = "学校通知";
                var receiveIds = Request["selectUserID"];
                var receiveNames = Request["selectUserName"];
                if (receiveIds != null && receiveNames != null)
                {
                    msl.EditNotice(model, receiveIds, receiveNames);
                }
                else {
                    db.Msg_Notice.AddObject(model);
                    db.SaveChanges();


                  // ZZ_MIFVSEntities fifvs=new ZZ_MIFVSEntities();

                  //var Teacher = fifvs.Teacher_ZZJG0101.Where(x => x.XXID == model.XXID).Select(x=>x.JSID);  //所有老师
                  // List<string> Teachername = fifvs.Teacher_ZZJG0101.Where(x => x.XXID == model.XXID).Select(x => x.XM);  //所有老师名字
                  //List<Guid> student = fifvs.Stu_ZZXS0101.Where(x => x.XXID == model.XXID).Select(x => x.XSID);  //所有学生
                  //List<string> studentname = fifvs.Stu_ZZXS0101.Where(x => x.XXID == model.XXID).Select(x => x.XM);  //所有学生姓名



                 
                  //  string[] list = Teacher.Concat(student);


                //   List<string> list = Teacher.ToList().AddRange(student);

                  // string[] array1 = uids.Split(',');
                //   string[] array2 = unames.Split(',');

                //    //获取所有可以收到消息的人的Id 和NAME
                //    foreach (var item in collection)
                //    {
                        
                //    }

                //        Msg_Receive receive = new Msg_Receive();
                //        receive.ID = Guid.NewGuid();
                //        receive.MsgID = model.ID;
                //        receive.ReceiveUserID = Guid.Parse(receiveIds);  //id
                //        receive.ReceiveUserName = receiveNames.ToString();   //name
                //        receive.XXID = model.XXID; 
                //        receive.ReadStatus = "未读";
                //        receive.IsRead = false;
                //        db.Msg_Receive.AddObject(receive);
                  
                  

                //    db.Msg_Notice.AddObject(model);
                //    db.SaveChanges();
                }

            }

            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }

        //已发通知列表
        public ActionResult ReciveSchoolQuery()
        {
            var list = msl.GetSchoolList(userid);
            return View(list);
        }
        //查看
        public ActionResult NoticeSchoolDetail()
        {
            var noticeID = Request["ID"];

            ViewBag.MsgSendUserName = db.Msg_Notice.FirstOrDefault(x => x.ID == Guid.Parse(noticeID)).MsgSendUserName;
            var list = msl.GetMsg(Guid.Parse(noticeID));
            return View(list);
        }
        //编辑
        public ActionResult EditSchoolNotice(Guid EditID)
        {
            var notice = msl.Msg_Notice(EditID);
            var people = msl.Msg_ReciveNotice(EditID);
            var ids = "";
            var names = "";
            foreach (var item in people)
            {
                ids += item.ReceiveUserID + ",";
                names += item.ReceiveUserName + ",";

            }
            if (!string.IsNullOrEmpty(notice.FileID))
            {
                var files = notice.FileID.Split(',');
                var filepaths = notice.FileUrl.Split(',');
                string split = string.Empty;
                string filestr = string.Empty;
                for (int i = 0; i < files.Length; i++)
                {
                    filestr += string.Format("{3}{{ \"fileid\":\"{0}\",\"filename\":\"{1}\",\"filepath\":\"{2}\"}}", notice.ID, files[i], filepaths[i], split);
                    split = ",";
                }
                filestr = "[" + filestr + "]";
               
                ViewBag.files = filestr;
             
            }

            
            return View(notice);
        }
        [HttpPost]
        public JsonResult EditSchoolNotice(Msg_Notice model)
        {
            var temp = db.Msg_Notice.FirstOrDefault(x => x.ID == model.ID);

            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                int type = Convert.ToInt32(Request["MsgTypeCode"]);
                temp.MsgDate = DateTime.Now;
                temp.XXID = schoolId;
                temp.MsgSendID = userid;
                temp.MsgSendLoginName = loginName;
                temp.MsgSendUserName = userName;
                temp.MsgTypeCode = Request["MsgTypeCode"];
                temp.MsgTitle = Request["MsgTitle"];
                temp.MsgContent = Request["MsgContent"];
                temp.MsgTypeName = "学校通知";

                temp.Status = "已发送";
                temp.MsgType = "通知公告";
                temp.FileUrl = Request["FileUrl"];
                temp.FileID = Request["FileID"];
                var receiveIds = Request["selectUserID"];
                var receiveNames = Request["selectUserName"];

                if (receiveIds != null && receiveNames != null)
                {
                    msl.EditNotice(model, receiveIds, receiveNames);
                }
                else
                {
                    //db.Msg_Notice.Attach(model);
                  
                    db.SaveChanges();
                }
               
            }

            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }

        //删除
        public JsonResult DeleteSchoolNotice(Guid noticeId)
        {
            var code =db.Msg_Notice.FirstOrDefault(x=>x.ID==noticeId);

            code.Status = "已删除";
            db.SaveChanges();
           // db.Msg_Notice.DeleteObject(code);
           
                
            return Json(code, JsonRequestBehavior.AllowGet);
        }
        

        #endregion

    }

}