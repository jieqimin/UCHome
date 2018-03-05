using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCHome.Models;
using ParentsRegister.Model;

namespace UCHome.Controllers
{
    public class ParentsRegisterController : Controller
    {
        //
        // GET: /ParentsRegister/
        private ParentsRegister.BLL.ParentsRegister bll = new ParentsRegister.BLL.ParentsRegister();

        public ActionResult Index(string openId, string sceneId)
        {
            ViewBag.openId = openId;
            ViewBag.sceneId = sceneId;
            return View();
        }

        public JsonResult CheckStudentInfo(Guid childschool, string childName, string childIcard)
        {
            int count=bll.CheckStudentInfo(childschool, childName, childIcard);
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubmitRegInfo(string name, string icard, string nickname, string email, string password, string checkPassword, string tel, Guid childschool, string childName, string childIcard)
        {
            var result = "注册失败！";
            Guid jzid = Guid.NewGuid();
            var flag=bll.AddParentsInfo(name, icard, nickname, email, password, checkPassword, tel, childschool, childName, childIcard, jzid);//添加数据到家长表，家长学生关系表
            if (flag == "注册成功")
            {
                string commentHalfurl = APIHelper.GetApiUrl("ParentsRegister");//获取接口地址
                var sum = commentHalfurl + "?UserGuid=" + jzid + "&username=" + nickname + "&displayname=" + name + "&sfzjh=" + icard + "&email=" + email + "&mobile=" + tel + "&xxidGuid=" + childschool + "&password=" + password;
                //恶意注入
                var resultdata = HttpClientHelper.POSTMethod(sum);
                if (resultdata.Contains(jzid.ToString()))
                    result = "注册成功"+","+jzid;
            }
            else
            {
                result = flag;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchByAreaAType(string area, string school)
        {
            var result = bll.GetSchByAreaAndType(area, school);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StudentAttendance(Guid? jzid)
        {
            return View(jzid);
        }
    }
}
