using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RedisCache.Model;
using ServiceStack.ServiceHost;
using Student.Model;
using TaskIntegral.Model;
using UCHome.BLL;
using UCHome.Model;
using UCHome.Models;
using UserInfo = UCHome.Model.UserInfo;

namespace UCHome.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/   
        static readonly UCHomeBasePage ucbase = new UCHomeBasePage();

        private string apiconfig
        {
            get
            {
                return Server.MapPath("~\\API.config");
            }
        }
        private static UserInfo user
        {
            get
            {
                return UCHomeBasePage.RequestUser;
            }
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
        private string loginname
        {
            get
            {
                return user.username;
            }
        }
        private string userType
        {
            get
            {
                return user.usertype;
            }
        }
        private string xxmc
        {
            get
            {
                return user.xxmc;
            }
        }
        private string areacode
        {
            get
            {
                try
                {
                    return user.areacode;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        private Guid XXID
        {
            get
            {
                try
                {
                    return user.xxid;
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }
        }
        private string headpic
        {
            get
            {
                return user.headpic;
            }
        }

        private ChildInfo child
        {
            get
            {
                return user.childinfo;
            }
        }
        private string subject
        {
            get
            {
                return user.subject;
            }
        }
        public string ThemeSkin
        {
            get
            {
                return user.skintheme;
            }
        }
        protected HttpContextBase PageContext
        {
            get
            {
                HttpContextWrapper context =
                new HttpContextWrapper(System.Web.HttpContext.Current);
                return context;
            }
        }
        public ActionResult Index(bool? flag)
        {
            if (flag == null)
                return RedirectToAction("HomePage");
            return View("Index", flag);
        }

        public ActionResult Integral()
        {
            return View();
        }
        public ActionResult HomePage()
        {
            if (user != null)
            {
                UCHomeEntities uc = new UCHomeEntities();
                UCHome_BaseInfo space = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == user.userid);
                if (space == null)
                {
                    ViewBag.usertype = user.usertype;
                    return View("Index", true);
                }
                if (space.UserType == null)
                {
                    space.UserType = user.usertype;
                    uc.SaveChanges();
                }
                ViewBag.XXID = user.xxid;
                ViewBag.UserID = loginId;
                ViewBag.OrgID = user.orgid;
                ViewBag.OrgName = user.orgname;
                //获取用户皮肤主题
                //if (userType.ToLower() == "s")
                //{
                //    return RedirectToAction("Index", "StudentCenter");
                //}
                if (userType.ToLower() == "p" && Request.Cookies["ChildInfo"] == null)
                {
                    StudentEntities stu = new StudentEntities();
                    List<Guid> rel =
                        stu.Stu_FamilyStuRel.Where(f => f.JZID == user.userid).Select(f => f.XSID).ToList();
                    if (rel.Count == 1)
                    {
                        //设置ChildGuid
                        Guid xsid = rel.First();
                        View_Simple_StuInfo childinfo = uc.View_Simple_StuInfo.SingleOrDefault(s => s.xsid == xsid);
                        UCHomeBasePage.SetChildCookies(childinfo);
                    }
                    else
                    {
                        List<View_Simple_StuInfo> stulist =
                            uc.View_Simple_StuInfo.Where(s => rel.Contains(s.xsid)).ToList();
                        ViewBag.childs = stulist;
                        return View("ChildRedirect", stulist);
                    }

                }
                return View("HomePage", user);
            }

            return View("Index", false);
        }

        public ViewResult ChildRedirect(Guid childid)
        {
            //设置ChildGuid
            UCHomeEntities uc = new UCHomeEntities();
            View_Simple_StuInfo childinfo = uc.View_Simple_StuInfo.SingleOrDefault(s => s.xsid == childid);
            UCHomeBasePage.SetChildCookies(childinfo);
            return View("HomePage", user);

        }
        public PartialViewResult HomeLeft()
        {
            if (userType.ToLower() == "t")
                ViewBag.Subject = subject;
            ViewBag.HeadPic = headpic;
            ViewBag.UserType = userType.ToLower() == "s" ? "学生" : userType.ToLower() == "p" ? "家长" : "老师";
            ViewBag.UserID = loginId;
            ViewBag.XM = loginname;
            ViewBag.XXMC = xxmc;
            ViewBag.XXID = XXID;
            return PartialView("HomeLeft", user);
        }
        public PartialViewResult HomeLeft2(UserInfo model)
        {
            if (model.usertype.ToLower() == "t")
                ViewBag.Subject = model.subject;
            ViewBag.HeadPic = model.headpic;
            ViewBag.UserType = model.usertype.ToLower() == "t" ? "教师" : model.usertype.ToLower() == "s" ? "学生" : "家长";
            ViewBag.UserID = model.userid;
            ViewBag.XM = model.username;
            ViewBag.XXMC = model.xxmc;
            ViewBag.XXID = model.xxid;
            return PartialView("HomeLeft2", model);
        }
        public PartialViewResult UserSkinSet()
        {
            return PartialView("UserSkinSet");
        }
        public PartialViewResult Skins()
        {
            UCHome_SkinBLL skinbll = new UCHome_SkinBLL();
            IEnumerable<UCHome_Skin> skinlist = skinbll.getsysskinlist();
            return PartialView("Skins", skinlist);
        }
        public ActionResult SkinSet(string skintheme, string skincolor)
        {
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_BaseInfo space = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == loginId);
            JsonResult jsonResult;
            if (space != null)
            {
                space.SkinTheme = skintheme;
                try
                {
                    uc.SaveChanges();
                    HttpCookie cookie = Request.Cookies["SpaceInfo"];
                    if (cookie != null)
                    {
                        cookie.Values["Theme"] = EncryptUtils.Base64Encrypt(skintheme);
                        cookie.Values["Skin"] = EncryptUtils.Base64Encrypt(skincolor);
                        Response.AppendCookie(cookie);
                    }
                    else
                    {
                        cookie = new HttpCookie("SpaceInfo");
                        cookie.Values.Add("Theme", EncryptUtils.Base64Encrypt(skintheme));
                        cookie.Values.Add("Skin", EncryptUtils.Base64Encrypt(skincolor));
                        Response.AppendCookie(cookie);
                    }
                    jsonResult = new JsonResult
                    {
                        Data = new
                        {
                            statuscode = "200"
                        }
                    };
                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    jsonResult = new JsonResult
                    {
                        Data = new
                        {
                            statuscode = "500"
                        }
                    };
                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }
            }
            jsonResult = new JsonResult
            {
                Data = new
                {
                    statuscode = "404"
                }
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult Portal()
        {
            return PartialView("Portal", user);
        }
        public PartialViewResult Portal2(UserInfo model)
        {
            return PartialView("Portal2", model);
        }
        public ActionResult Person()
        {
            if (user != null)
                if (user.usertype.ToLower() == "t")
                    return Redirect(Url.Action("PersonInfo2", "TeacherCenter", new { id = loginId }));
            return Redirect(Url.Action("PersonInfo2", "TeacherCenter", new { id = loginId }));
        }
        public ActionResult Chat()
        {
            ViewBag.ClientName = "用户-" + Guid.NewGuid().ToString();
            return View();
        }
        public ActionResult AliveSpace(FormCollection form)
        {
            JsonResult jsonResult;
            if (user != null)
            {
                UCHomeEntities uc = new UCHomeEntities();
                UCHome_BaseInfo space = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == user.userid);
                if (space == null)
                {
                    UCHome_BaseInfo ucbBaseInfo = new UCHome_BaseInfo
                    {
                        PKID = Guid.NewGuid(),
                        NickName = form["NickName"],
                        Section = form["Section"],
                        Subject = form["Subject"],
                        UserID = user.userid,
                        UserType = user.usertype,
                        Visits = 0,
                        ExcellentSpace = "other",
                        IsStatus = "1",
                        CreateTime = DateTime.Now
                    };
                    uc.UCHome_BaseInfo.AddObject(ucbBaseInfo);
                    try
                    {
                        uc.SaveChanges();
                        jsonResult = new JsonResult
                        {
                            Data = new
                            {
                                result = "success",
                                uctype = user.usertype,
                                subject = form["Subject"]
                            }
                        };
                        UCHomeBasePage.SetSpaceCookies(ucbBaseInfo);
                        return Json(jsonResult, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception)
                    {
                        jsonResult = new JsonResult
                        {
                            Data = new
                            {
                                result = "fail",
                                uctype = "X"
                            }
                        };
                        return Json(jsonResult, JsonRequestBehavior.AllowGet);
                    }
                }
                jsonResult = new JsonResult
                {
                    Data = new
                    {
                        result = "exist",
                        uctype = space.UserType
                    }
                };
            }
            else
            {
                jsonResult = new JsonResult
                {
                    Data = new
                    {
                        result = "fail",
                        uctype = "X"
                    }
                };
            }
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateInfo(FormCollection form)
        {
            UCHomeEntities uc = new UCHomeEntities();
            UCHome_BaseInfo space = uc.UCHome_BaseInfo.SingleOrDefault(u => u.UserID == loginId);
            JsonResult jsonResult;
            if (space != null)
            {
                space.NickName = form["NickName"];
                space.Section = form["Section"];
                space.Subject = form["Subject"];
                try
                {
                    uc.SaveChanges();
                    HttpCookie cookie = Request.Cookies["SpaceInfo"];
                    if (cookie != null)
                    {
                        cookie.Values["Subject"] = EncryptUtils.Base64Encrypt(form["Subject"]);
                        Response.AppendCookie(cookie);
                    }
                    else
                    {
                        cookie = new HttpCookie("SpaceInfo");
                        cookie.Values.Add("Subject", EncryptUtils.Base64Encrypt(form["Subject"]));
                        Response.AppendCookie(cookie);
                    }
                    jsonResult = new JsonResult
                    {
                        Data = new
                        {
                            result = "success",
                            uctype = user.usertype,
                            subject = form["Subject"]
                        }
                    };
                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    jsonResult = new JsonResult
                    {
                        Data = new
                        {
                            result = "fail",
                            uctype = "X"
                        }
                    };
                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }
            }
            jsonResult = new JsonResult
            {
                Data = new
                {
                    result = "noexist",
                    uctype = "X"
                }
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChildList(Guid jzid)
        {
            UCHomeEntities uc = new UCHomeEntities();
            StudentEntities stu = new StudentEntities();
            List<Guid> rel =
                stu.Stu_FamilyStuRel.Where(f => f.JZID == user.userid).Select(f => f.XSID).ToList();
            List<View_Simple_StuInfo> stulist =
                uc.View_Simple_StuInfo.Where(s => rel.Contains(s.xsid)).ToList();
            return Json(stulist, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetDataChart()
        //{
        //    string apiurl = APIHelper.GetApiUrl("getpersondata", apiconfig);
        //    apiurl = string.Format(apiurl.IndexOf('?') > -1 ? "{0}&userid={1}" : "{0}?userid={1}", apiurl, loginId);
        //    var resultdata = HttpClientHelper.GETMethod(apiurl);
        //    return Json(resultdata, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetDataChart(Guid? userid)
        {
            Guid id = loginId;
            if (userid != null)
            {
                id = userid.Value;
            }
            string apiurl = APIHelper.GetApiUrl("getpersondata", apiconfig);
            apiurl = string.Format(apiurl.IndexOf('?') > -1 ? "{0}&userid={1}" : "{0}?userid={1}", apiurl, id);
            var resultdata = HttpClientHelper.GETMethod(apiurl);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetArticleCount(Guid? userid)
        {
            Guid id = loginId;
            if (userid != null)
            {
                id = userid.Value;
            }
            string apiurl = APIHelper.GetApiUrl("getarticlecount", apiconfig);
            apiurl = string.Format(apiurl.IndexOf('?') > -1 ? "{0}&userid={1}" : "{0}?userid={1}", apiurl, id);
            var resultdata = HttpClientHelper.GETMethod(apiurl);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetErrorCount(Guid? userid)
        {
            Guid id = loginId;
            if (userid != null)
            {
                id = userid.Value;
            }
            string apiurl = APIHelper.GetApiUrl("studentErrorCount", apiconfig);
            apiurl = string.Format("{0}?userGuid={1}", apiurl, id);
            var resultdata = HttpClientHelper.GETMethod(apiurl);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetResourceCount(Guid? userid)
        {
            Guid id = loginId;
            if (userid != null)
            {
                id = userid.Value;
            }
            string apiurl = APIHelper.GetApiUrl("getresourcecount", apiconfig);
            apiurl = string.Format(apiurl.IndexOf('?') > -1 ? "{0}&userid={1}" : "{0}/{1}", apiurl, id);
            var resultdata = HttpClientHelper.GETMethod(apiurl);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEbookCount(Guid? userid)
        {
            Guid id = loginId;
            if (userid != null)
            {
                id = userid.Value;
            }
            string apiurl = APIHelper.GetApiUrl("getebookdata", apiconfig);
            EbookPara para = new EbookPara
            {
                PageIndex = 1,
                PageSize = 10,
                UserId = id
            };
            var resultdata = HttpClientHelper.POSTMethod(apiurl, para);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDayTask()
        {
            string apiurl = APIHelper.GetApiUrl("getdaytask", apiconfig);
            apiurl = string.Format(apiurl.IndexOf('?') > -1 ? "{0}&userid={1}" : "{0}?userid={1}", apiurl, loginId);
            var resultdata = HttpClientHelper.GETMethod(apiurl);

            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetArticleCountById(Guid userid)
        {
            UCHomeEntities uc = new UCHomeEntities();
            int count = uc.UCHome_PersonNew.Count(a => a.AddUser == userid && (a.UCType == "article" || a.UCType == "log"));
            JsonResult jsonResult = new JsonResult
            {
                Data = new
                {
                    Count = count
                }
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult StatData()
        {

            //计算文章数及其排名
            UCHomeEntities uc = new UCHomeEntities();

            //if (HttpContext.Cache["ArticleStat"] == null)
            //{
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@UserID", loginId);
            long citynum =
                 uc.ExecuteStoreQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by count(1) desc) Number,  AddUser UserID,count(1) ACount,getdate() LastTime from UCHome_PersonNew a inner join Teacher_ZZJG0101 b on a.AddUser=b.JSID where (uctype='article' or uctype='log')  group by AddUser) a where UserID=@UserID",
                     paras).FirstOrDefault();
            //区级排名
            object[] paras2 = new SqlParameter[2];
            paras2[0] = new SqlParameter("@UserID", loginId);
            paras2[1] = new SqlParameter("@AreaCode", areacode);
            long areanum =
                 uc.ExecuteStoreQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by count(1) desc) Number,  AddUser UserID,count(1) ACount,getdate() LastTime from UCHome_PersonNew a inner join Teacher_ZZJG0101 b on a.AddUser=b.JSID inner join basic_zzxx0101 c on b.XXID=c.XXID where (uctype='article' or uctype='log') and c.XZQHM=@AreaCode  group by AddUser) a where UserID=@UserID",
                     paras2).FirstOrDefault();
            //areanum = anos.Count(a => a.AreaCode == areacode && a.Number < citynum);
            //校级排名
            object[] paras3 = new SqlParameter[2];
            paras3[0] = new SqlParameter("@UserID", loginId);
            paras3[1] = new SqlParameter("@XXID", XXID);
            long schoolnum =
                 uc.ExecuteStoreQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by count(1) desc) Number,  AddUser UserID,count(1) ACount,getdate() LastTime from UCHome_PersonNew a inner join Teacher_ZZJG0101 b on a.AddUser=b.JSID where (uctype='article' or uctype='log') and b.XXID=@XXID  group by AddUser) a where UserID=@UserID",
                     paras3).FirstOrDefault();
            //资源排名
            string apiurl = APIHelper.GetApiUrl("getresourceorder", apiconfig);
            apiurl = string.Format(apiurl.IndexOf('?') > -1 ? "{0}&userid={1}" : "{0}/{1}", apiurl, loginId);
            var resultdata = HttpClientHelper.GETMethod(apiurl);
            long cityresnum = 0, arearesnum = 0, schoolresnum = 0;
            var resorder = JsonConvert.DeserializeObject<ResourceNo>(resultdata);
            if (resorder != null)
            {
                cityresnum = resorder.CityOrder;
                arearesnum = resorder.AreaOrder;
                schoolresnum = resorder.SchoolOrder;
            }
            JsonResult jsonResult = new JsonResult
            {
                Data = new
                {
                    UserID = loginId,
                    CityNum = citynum,
                    AreaNum = areanum,
                    SchoolNum = schoolnum,
                    CityResNum = cityresnum,
                    AreaResNum = arearesnum,
                    SchoolResNum = schoolresnum
                }
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IntegralInfo()
        {
            TaskModelEntities task = new TaskModelEntities();
            //获取学分
            object[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@UserID", loginId);
            Decimal TotalScore = task.Database.SqlQuery<Decimal>(
                     "select isnull(Sum(TaskScore),0) TotalScore from UCHome_UserIntergral where UserID=@UserID",
                     paras).FirstOrDefault();
            //获取等级
            object[] paras2 = new SqlParameter[1];
            paras2[0] = new SqlParameter("@Scores", TotalScore);
            LevelInfo LevelInfo = task.Database.SqlQuery<LevelInfo>(
                     "select min(pindex) Pindex,min(Scores) Score,min(pindex)+1 Lindex from UCHome_IntegralLevel where Scores>@Scores",
                     paras2).FirstOrDefault();
            //积分排名
            object[] paras3 = new SqlParameter[1];
            paras3[0] = new SqlParameter("@UserID", loginId);
            long citynum =
                 task.Database.SqlQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by Sum(TaskScore) desc) Number,  UserID UserID,Sum(TaskScore) ACount from UCHome_UserIntergral a inner join Teacher_ZZJG0101 b on a.UserID=b.JSID  group by UserID) a where UserID=@UserID",
                     paras3).FirstOrDefault();
            //区级排名
            object[] paras4 = new SqlParameter[2];
            paras4[0] = new SqlParameter("@UserID", loginId);
            paras4[1] = new SqlParameter("@AreaCode", areacode);
            long areanum =
                 task.Database.SqlQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by Sum(TaskScore) desc) Number,  UserID UserID,Sum(TaskScore) ACount from UCHome_UserIntergral a inner join Teacher_ZZJG0101 b on a.UserID=b.JSID inner join basic_zzxx0101 c on b.XXID=c.XXID where  c.XZQHM=@AreaCode  group by UserID) a where UserID=@UserID",
                     paras4).FirstOrDefault();
            //areanum = anos.Count(a => a.AreaCode == areacode && a.Number < citynum);
            //校级排名
            object[] paras5 = new SqlParameter[2];
            paras5[0] = new SqlParameter("@UserID", loginId);
            paras5[1] = new SqlParameter("@XXID", XXID);
            long schoolnum =
                 task.Database.SqlQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by Sum(TaskScore) desc) Number,  UserID UserID,Sum(TaskScore) ACount from UCHome_UserIntergral a inner join Teacher_ZZJG0101 b on a.UserID=b.JSID where b.XXID=@XXID  group by UserID) a where UserID=@UserID",
                     paras5).FirstOrDefault();
            JsonResult jsonResult = new JsonResult
            {
                Data = new
                {
                    UserID = loginId,
                    TotalScore = TotalScore,
                    Level = LevelInfo == null ? 1 : LevelInfo.Pindex,
                    LLevel = LevelInfo == null ? 1 : LevelInfo.Lindex,
                    Score = LevelInfo == null ? 1 : LevelInfo.Score,
                    CityNum = citynum,
                    AreaNum = areanum,
                    SchoolNum = schoolnum,
                }
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StatData2(Guid teacherid, string xzqid, Guid schoolid)
        {

            //计算文章数及其排名
            UCHomeEntities uc = new UCHomeEntities();

            //if (HttpContext.Cache["ArticleStat"] == null)
            //{
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@UserID", teacherid);
            long citynum =
                 uc.ExecuteStoreQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by count(1) desc) Number,  AddUser UserID,count(1) ACount,getdate() LastTime from UCHome_PersonNew a inner join Teacher_ZZJG0101 b on a.AddUser=b.JSID where (uctype='article' or uctype='log')  group by AddUser) a where UserID=@UserID",
                     paras).FirstOrDefault();
            //区级排名
            object[] paras2 = new SqlParameter[2];
            paras2[0] = new SqlParameter("@UserID", teacherid);
            paras2[1] = new SqlParameter("@AreaCode", xzqid);
            long areanum =
                 uc.ExecuteStoreQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by count(1) desc) Number,  AddUser UserID,count(1) ACount,getdate() LastTime from UCHome_PersonNew a inner join Teacher_ZZJG0101 b on a.AddUser=b.JSID inner join basic_zzxx0101 c on b.XXID=c.XXID where (uctype='article' or uctype='log') and c.XZQHM=@AreaCode  group by AddUser) a where UserID=@UserID",
                     paras2).FirstOrDefault();
            //areanum = anos.Count(a => a.AreaCode == areacode && a.Number < citynum);
            //校级排名
            object[] paras3 = new SqlParameter[2];
            paras3[0] = new SqlParameter("@UserID", teacherid);
            paras3[1] = new SqlParameter("@XXID", schoolid);
            long schoolnum =
                 uc.ExecuteStoreQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by count(1) desc) Number,  AddUser UserID,count(1) ACount,getdate() LastTime from UCHome_PersonNew a inner join Teacher_ZZJG0101 b on a.AddUser=b.JSID where (uctype='article' or uctype='log') and b.XXID=@XXID  group by AddUser) a where UserID=@UserID",
                     paras3).FirstOrDefault();
            //资源排名
            string apiurl = APIHelper.GetApiUrl("getresourceorder", apiconfig);
            apiurl = string.Format(apiurl.IndexOf('?') > -1 ? "{0}&userid={1}" : "{0}/{1}", apiurl, teacherid);
            var resultdata = HttpClientHelper.GETMethod(apiurl);
            long cityresnum = 0, arearesnum = 0, schoolresnum = 0;
            var resorder = JsonConvert.DeserializeObject<ResourceNo>(resultdata);
            if (resorder != null)
            {
                cityresnum = resorder.CityOrder;
                arearesnum = resorder.AreaOrder;
                schoolresnum = resorder.SchoolOrder;
            }
            JsonResult jsonResult = new JsonResult
            {
                Data = new
                {
                    UserID = teacherid,
                    CityNum = citynum,
                    AreaNum = areanum,
                    SchoolNum = schoolnum,
                    CityResNum = cityresnum,
                    AreaResNum = arearesnum,
                    SchoolResNum = schoolresnum
                }
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult IntegralInfo2(Guid teacherid, string xzqid, Guid schoolid)
        {
            TaskModelEntities task = new TaskModelEntities();
            //获取学分
            object[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@UserID", teacherid);
            Decimal TotalScore = task.Database.SqlQuery<Decimal>(
                     "select isnull(Sum(TaskScore),0) TotalScore from UCHome_UserIntergral where UserID=@UserID",
                     paras).FirstOrDefault();
            //获取等级
            object[] paras2 = new SqlParameter[1];
            paras2[0] = new SqlParameter("@Scores", TotalScore);
            LevelInfo LevelInfo = task.Database.SqlQuery<LevelInfo>(
                     "select min(pindex) Pindex,min(Scores) Score,min(pindex)+1 Lindex from UCHome_IntegralLevel where Scores>@Scores",
                     paras2).FirstOrDefault();
            //积分排名
            object[] paras3 = new SqlParameter[1];
            paras3[0] = new SqlParameter("@UserID", teacherid);
            long citynum =
                 task.Database.SqlQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by Sum(TaskScore) desc) Number,  UserID UserID,Sum(TaskScore) ACount from UCHome_UserIntergral a inner join Teacher_ZZJG0101 b on a.UserID=b.JSID  group by UserID) a where UserID=@UserID",
                     paras3).FirstOrDefault();
            //区级排名
            object[] paras4 = new SqlParameter[2];
            paras4[0] = new SqlParameter("@UserID", teacherid);
            paras4[1] = new SqlParameter("@AreaCode", xzqid);
            long areanum =
                 task.Database.SqlQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by Sum(TaskScore) desc) Number,  UserID UserID,Sum(TaskScore) ACount from UCHome_UserIntergral a inner join Teacher_ZZJG0101 b on a.UserID=b.JSID inner join basic_zzxx0101 c on b.XXID=c.XXID where  c.XZQHM=@AreaCode  group by UserID) a where UserID=@UserID",
                     paras4).FirstOrDefault();
            //areanum = anos.Count(a => a.AreaCode == areacode && a.Number < citynum);
            //校级排名
            object[] paras5 = new SqlParameter[2];
            paras5[0] = new SqlParameter("@UserID", teacherid);
            paras5[1] = new SqlParameter("@XXID", schoolid);
            long schoolnum =
                 task.Database.SqlQuery<long>(
                     "select Number from (select ROW_NUMBER() over(order by Sum(TaskScore) desc) Number,  UserID UserID,Sum(TaskScore) ACount from UCHome_UserIntergral a inner join Teacher_ZZJG0101 b on a.UserID=b.JSID where b.XXID=@XXID  group by UserID) a where UserID=@UserID",
                     paras5).FirstOrDefault();
            JsonResult jsonResult = new JsonResult
            {
                Data = new
                {
                    UserID = teacherid,
                    TotalScore = TotalScore,
                    Level = LevelInfo == null ? 1 : LevelInfo.Pindex,
                    LLevel = LevelInfo == null ? 1 : LevelInfo.Lindex,
                    Score = LevelInfo == null ? 1 : LevelInfo.Score,
                    CityNum = citynum,
                    AreaNum = areanum,
                    SchoolNum = schoolnum,
                }
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        //学生主页api
        public JsonResult GetStuCourse(Guid? childidGuid)
        {

            string apiurl = APIHelper.GetApiUrl("getstucourse", apiconfig);
            EbookPara para = new EbookPara
            {
                PageIndex = 1,
                PageSize = 2,
                UserId = childidGuid != null ? childidGuid.Value : loginId
            };
            var resultdata = HttpClientHelper.POSTMethod(apiurl, para);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
        //学校通知
        public JsonResult GetSchoolNotices(Guid xxid, int top)
        {
            string apiurl = APIHelper.GetApiUrl("getschnotices", apiconfig);
            apiurl = string.Format("{0}?userId={1}&xxid={2}&top={3}", apiurl, loginId, xxid, top);
            var resultdata = HttpClientHelper.GETMethod(apiurl);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
        //班级活动
        public JsonResult GetClassAction(Guid classid, string type, int top)
        {

            string apiurl = APIHelper.GetApiUrl("getclassaction", apiconfig);
            apiurl = string.Format("{0}?classid={1}&top={2}&type={3}", apiurl, classid, top, type);
            var resultdata = HttpClientHelper.GETMethod(apiurl);
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
    }
    public class LevelInfo
    {
        public int Pindex { get; set; }
        public int Lindex { get; set; }
        public Decimal Score { get; set; }

    }
    public class ArticleNo
    {
        public Guid UserID { get; set; }
        public long Number { get; set; }
        public int ACount { get; set; }
        public DateTime LastTime { get; set; }
    }
    public class ResourceNo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public long ResCount { get; set; }
        public long CityOrder { get; set; }
        public long AreaOrder { get; set; }
        public long SchoolOrder { get; set; }
    }

    public class EbookPara
    {
        public Guid UserId { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
