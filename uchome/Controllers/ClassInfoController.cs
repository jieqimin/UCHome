using ClassCenterBLL;
using ClassCenterModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeacherInfo.Model;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class ClassInfoController : Controller
    {
        ClassCenterInfo msl = new ClassCenterInfo();
        private readonly UCHomeBasePage page = new UCHomeBasePage();
        ClassEntities db = new ClassEntities();
        ZZ_MIFVSEntities entity = new ZZ_MIFVSEntities();
        private readonly string HonorImageUrl = ConfigurationManager.AppSettings["HonorImageUrl"];
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

        public ActionResult ActiveIndex()
        {

            var list = msl.GetClassActiveList(XXID);
            return View(list);
        }
        #region 班级活动新建页面HTML
        /// <summary>
        /// 班级活动页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult ActiveCreate()
        {
            ClassActive award = new ClassActive();
            award.ID = Guid.NewGuid();
            ViewBag.Images = "";
            return View("ActiveEdit", award);
        }

        #endregion

        #region 班级活动编辑页面HTML
        /// <summary>
        ///班级活动页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult ActiveEdit(Guid ID)
        {
            var teaAward = msl.GetClassActiveInfo(ID);
            ViewBag.Images = teaAward.ActivePhotoUrl;
            return View(teaAward);
        }

        [HttpPost]
        public JsonResult EditSend(ClassActive model)
        {
            var usid = userid.ToString();
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    model.XXID = XXID;
                    model.Creator = userid.ToString();

                    if (!string.IsNullOrEmpty(model.ActiveContent))
                    {
                        model.ActiveContent = HttpUtility.UrlDecode(model.ActiveContent);
                    }
                    var hurl = Request.Form["hidImages"];


                    if (!String.IsNullOrEmpty(hurl))
                    {
                        hurl = hurl.Substring(0, hurl.Length - 1);
                        model.ActivePhotoName = hurl;
                        var spurl = "";
                        var sp = hurl.Split(';');
                        for (int i = 0; i < sp.Length; i++)
                        {
                            spurl += (HonorImageUrl + sp[i]) + ";";
                        }
                        if (spurl.Length > 0)
                        {
                            spurl = spurl.Substring(0, spurl.Length - 1);
                        }
                        model.ActivePhotoUrl = spurl;
                    }

                    model.ActivePhotoUrl = Request.Form["hidImages"];
                    model.CreateTime = DateTime.Now.ToString();
                    var bjname = msl.GetClassName(usid);

                    var bjid = msl.GetClassID(usid);
                    if (!String.IsNullOrEmpty(bjid) && !String.IsNullOrEmpty(bjname))
                    {
                        model.BJMC = bjname;
                        model.BJID = new Guid(bjid);
                        msl.EditSend(model);
                    }
                    else
                    {
                        statuscode = "400";
                    }


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

        #endregion
        public ActionResult ActiveDetails(Guid ID)
        {
            var teaAward = msl.GetClassActiveInfo(ID);
            ViewBag.Images = teaAward.ActivePhotoUrl;
            return View(teaAward);
        }
        public JsonResult DeleteActive(Guid Id)
        {
            var code = "1";
            try
            {
                msl.DeleteActive(Id);
            }
            catch (Exception)
            {
                code = "0";
            }

            return Json(code, JsonRequestBehavior.AllowGet);
        }


        public ActionResult HonorIndex()
        {
            var list = msl.GetClassHonorList(XXID);
            return View(list);
        }
        public ActionResult HonorDetails(Guid ID)
        {
            var teaAward = msl.GetClassHonorInfo(ID);
            ViewBag.Images = teaAward.HonorPhotoName;
            return View(teaAward);
        }
        public JsonResult DeleteHonor(Guid Id)
        {
            var code = "1";
            try
            {
                msl.DeleteHonor(Id);
            }
            catch (Exception)
            {
                code = "0";
            }

            return Json(code, JsonRequestBehavior.AllowGet);
        }
        #region 班级活动新建页面HTML
        /// <summary>
        /// 班级活动页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult HonorCreate()
        {
            ClassHonor award = new ClassHonor();
            award.ID = Guid.NewGuid();
            ViewBag.Images = "";
            return View("HonorEdit", award);
        }

        #endregion

        #region 班级活动编辑页面HTML
        /// <summary>
        ///班级活动页面HTML
        /// </summary>
        /// <returns></returns>
        public ActionResult HonorEdit(Guid ID)
        {
            var teaAward = msl.GetClassHonorInfo(ID);
            ViewBag.Images = teaAward.HonorPhotoName;
            return View(teaAward);
        }

        [HttpPost]
        public JsonResult EditHonorSend(ClassHonor model)
        {
            var usid = userid.ToString();
            string statuscode = "200";
            string msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    model.XXID = XXID;
                    model.Creator = userid.ToString();
                    var hurl = Request.Form["hidImages"];


                    if (!String.IsNullOrEmpty(hurl))
                    {
                        hurl = hurl.Substring(0, hurl.Length - 1);
                        model.HonorPhotoName = hurl;
                        var spurl = "";
                        var sp = hurl.Split(';');
                        for (int i = 0; i < sp.Length; i++)
                        {
                            spurl += (HonorImageUrl + sp[i]) + ";";
                        }
                        if (spurl.Length > 0)
                        {
                            spurl = spurl.Substring(0, spurl.Length - 1);
                        }
                        model.HonorPhotoUrl = spurl;
                    }
                    model.CreateTime = DateTime.Now.ToString();
                    var bjname = msl.GetClassName(usid);
                    var bjid = msl.GetClassID(usid);
                    if (!String.IsNullOrEmpty(bjid) && !String.IsNullOrEmpty(bjname))
                    {
                        model.BJMC = bjname;
                        model.BJID = new Guid(bjid);
                        msl.EditHonorSend(model);
                    }
                    else
                    {
                        statuscode = "400";
                    }


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

        #endregion

        public ActionResult DutyEdit()
        {
            var usid = userid.ToString();
            var bjid = msl.GetClassID(usid);
            ClassDuty teaAward = new ClassDuty();
            if (!string.IsNullOrEmpty(bjid))
            {
                teaAward = msl.GetClassDutyInfo(XXID, new Guid(bjid));
                return View(teaAward);
            }
            else
            {
                ClassDuty award = new ClassDuty();
                award.ID = Guid.NewGuid();
                return View(award);
            }

        }

        public JsonResult GetDuty(Guid xxid, Guid bjid)
        {
            var duty = msl.GetClassDutyInfo(xxid, bjid);



            return Json(duty, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditDutySend(ClassDuty model)
        {
            var usid = userid.ToString();


            string statuscode = "200";
            string msg = "";
            try
            {

                model.XXID = XXID;
                model.Creator = userid.ToString();

                model.CreateTime = DateTime.Now.ToString();
                var bjname = msl.GetClassName(usid);
                var bjid = msl.GetClassID(usid);
                if (!String.IsNullOrEmpty(bjid) && !String.IsNullOrEmpty(bjname))
                {
                    model.BJMC = bjname;
                    model.BJID = new Guid(bjid);
                    msl.EditDutySend(model);
                }
                else
                {
                    statuscode = "400";
                }


            }
            catch (Exception ex)
            {
                statuscode = "500";
                msg = ex.ToString();
            }
            JsonResult rlt = new JsonResult { Data = new { statuscode, msg } };
            return Json(rlt, JsonRequestBehavior.AllowGet);
        }
    }
}
