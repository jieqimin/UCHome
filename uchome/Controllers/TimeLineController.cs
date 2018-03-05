using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.Views
{
    /// <summary>
    /// 时光轴控制器类
    /// </summary>
    public class TimeLineController : Controller
    {

        #region 时光轴首页Html
        readonly UCHomeBasePage ucbase=new UCHomeBasePage();
        /// <summary>
        /// 时光轴首页Html
        /// </summary>
        /// <returns></returns>
        private static UserInfo user
        {
            get { return UCHomeBasePage.RequestUser; }
        }
        public ActionResult Index() 
        {
            var result = new TimeLineViewModel() { 
                ID = user.userid,
                ImageUrl = ConfigurationManager.AppSettings["ImageUrl"]
            };
            return View(result);
        }
        #endregion

    }
}
