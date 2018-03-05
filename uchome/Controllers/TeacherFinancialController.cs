using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using OA.BLL.TeacherFinancial;
using UCHome.Model;

namespace UCHome.Controllers
{
    public class TeacherFinancialController : Controller
    {
        private readonly TeacherFinancialService _teacherFinancialService = new TeacherFinancialService();
        readonly UCHomeBasePage ucbase = new UCHomeBasePage();
        private static UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        private Guid loginId
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
        public ActionResult IndexPay()
        {
            var teacherPayList = _teacherFinancialService.GetTeacherPayList(loginId);

            return View(teacherPayList);
        }

        public ActionResult PayDetail()
        {
            var financialId = Request["financialId"];
            var financialModelId = Request["financialModelId"];
            var payModelType = Request["payModelType"];

            var teacherPayDetail = _teacherFinancialService.GetTeacherPayDetail(new Guid(financialModelId),
                new Guid(financialId), payModelType);

            return View(teacherPayDetail);
        }
    }
}