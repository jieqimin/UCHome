using System.Text.RegularExpressions;
using System.Web.Configuration;
using ServiceModel;
using ServiceStack.Service;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCHome.Model;
using UCHome.Models;

namespace UCHome.Controllers
{
    public class PublicShareController : Controller
    {
        private static readonly string http = WebConfigurationManager.AppSettings["APIHttp"];

        readonly IServiceClient client = new JsonServiceClient(http + "/SNSApi/");
        readonly UCHomeBasePage page = new UCHomeBasePage();
        UCHomeEntities uc = new UCHomeEntities();
        #region 属性
        private string dnsconfig
        {
            get
            {
                return Server.MapPath("~\\DNS.config");
            }
        }

        private Model.UserInfo user
        {
            get
            {
                return UCHomeBasePage.RequestUser;
            }
        }
        private Guid userid
        {
            get
            {
                return user.userid;
            }
        }
        private string XXMC
        {
            get { return user.xxmc; }
        }
        private string DisPlayName
        {
            get { return user.username; }
        }

        private string AreaCode
        {
            get { return user.areacode; }
        }
        #endregion
        //
        // GET: /PublicShare/
        public ActionResult PersonList(string usertype, Guid bjid)
        {
            List<UserInfo> userlist = new List<UserInfo>();
            switch (usertype)
            {
                case "T":
                    userlist = GetTeacherList(bjid);
                    break;
                case "S":
                    userlist = GetStudentList(bjid);
                    break;
                case "P":
                    userlist = GetParentList(bjid);
                    break;
            }
            return View("PersonList", userlist);
        }
        //Attentionlist
        public ActionResult Attentionlist(Guid id)
        {
            return PartialView("Attentionlist", id);
        }
        public ActionResult ValidAttent(Guid AttentUser)
        {
            try
            {
                UCHome_Attention vsu = uc.UCHome_Attention.SingleOrDefault(u => u.AttenUser == AttentUser && u.AddUser == userid);
                if (vsu != null)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddAttent(Guid AttentUser)
        {
            View_Simple_User vsu = uc.View_Simple_User.SingleOrDefault(u => u.userid == AttentUser);
            if (vsu != null)
            {
                UCHome_Attention attent = new UCHome_Attention
                {
                    PKID = Guid.NewGuid(),
                    AddUser = userid,
                    AttenUser = vsu.userid,
                    AttenName = vsu.username,
                    AttenTime = DateTime.Now,
                    AttenIdentity = vsu.usertype.ToUpper()
                };
                uc.UCHome_Attention.AddObject(attent);
                uc.SaveChanges();

                AddAddrBookEntry book = new AddAddrBookEntry
                {
                    GroupID = user.userid.ToString(),
                    GroupName = "我的关注",
                    UID = vsu.userid.ToString(),
                    UName = vsu.username
                };

                client.Send<AddAddrBookEntry>(book);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CancelAttent(Guid AttentUser)
        {
            View_Simple_User vsu = uc.View_Simple_User.SingleOrDefault(u => u.userid == AttentUser);
            if (vsu != null)
            {
                UCHome_Attention attent =
                    uc.UCHome_Attention.FirstOrDefault(u => u.AddUser == userid && u.AttenUser == vsu.userid);
                if (attent != null)
                {
                    uc.UCHome_Attention.DeleteObject(attent);
                    uc.SaveChanges();

                    var client1 = new JsonServiceClient(http + "/SNSApi/");
                    DeleteAddrBook book = new DeleteAddrBook
                    {
                        GroupID = user.userid.ToString(),
                        UID = vsu.userid.ToString()
                    };

                    client1.Delete(book);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonArticle(Guid id)
        {
            Guid jsid = id;
            View_Simple_TeaInfo tea = uc.View_Simple_TeaInfo.SingleOrDefault(u => u.jsid == jsid);
            return PartialView("PersonArticle", tea);
        }
        public ActionResult PersonLog(Guid id)
        {
            return PartialView("PersonLog", id);
        }
        public ActionResult PersonDicuss(Guid id)
        {
            ViewBag.XM = DisPlayName;
            return PartialView("PersonDicuss", id);
        }
        public ActionResult PersonSay(Guid id)
        {
            return PartialView("PersonSay", id);
        }
        public ActionResult PersonLog2(Guid id)
        {
            return PartialView("PersonLog2", id);
        }
        public ActionResult PersonSay2(Guid id)
        {
            return PartialView("PersonSay2", id);
        }
        public ActionResult PersonPhoto(Guid id)
        {
            return PartialView("PersonPhoto", id);
        }
        public ActionResult PersonPhoto2(Guid id)
        {
            return PartialView("PersonPhoto2", id);
        }
        public ActionResult PhotoAlbum(Guid id)
        {

            return PartialView("PhotoAlbum", id);
        }
        public ActionResult PhotoShow(Guid id)
        {
            return PartialView("PhotoShow", id);
        }

        public ActionResult PhotoShowByAlbum(Guid albumid)
        {
            UCHome_Album album = uc.UCHome_Album.SingleOrDefault(a => a.PKID == albumid);
            return PartialView("PhotoShowByAlbum", album);
        }
        public ActionResult PhotoAbstract2(Guid id)
        {
            return PartialView("PhotoAbstract2", id);
        }
        public ActionResult PhotoAlbum2(Guid id)
        {

            return PartialView("PhotoAlbum2", id);
        }
        public ActionResult PhotoShow2(Guid id)
        {
            return PartialView("PhotoShow2", id);
        }

        public ActionResult PhotoShowByAlbum2(Guid albumid)
        {
            UCHome_Album album = uc.UCHome_Album.SingleOrDefault(a => a.PKID == albumid);
            return PartialView("PhotoShowByAlbum2", album);
        }
        public ActionResult AddAlbum(FormCollection form)
        {
            string opertype = form["opertype"];
            UCHome_Album album = new UCHome_Album();
            if (opertype == "add")
            {
                album.PKID = Guid.NewGuid();
                album.AlbumName = form["album-name"];
                album.AlbumAbstract = form["album-memo"];
                album.AlbumThemes = form["album-themes"];
                album.AlbumType = form["album-type"];
                album.IsShare = form["album-role"];
                album.AddUser = new Guid(form["AddUser"]);
                album.CreateTime = DateTime.Now;
                album.IsShow = 1;
                uc.UCHome_Album.AddObject(album);
            }
            else
            {
                Guid pkid = new Guid(form["PKID"]);
                album = uc.UCHome_Album.SingleOrDefault(a => a.PKID == pkid);
                if (album != null)
                {
                    album.AlbumName = form["album-name"];
                    album.AlbumAbstract = form["album-memo"];
                    album.AlbumThemes = form["album-themes"];
                    album.AlbumType = form["album-type"];
                    album.IsShare = form["album-role"];
                }
            }
            try
            {
                uc.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddDicussGroup(FormCollection form)
        {
            string opertype = form["opertype"];
            UCHome_Dicuss dicuss = new UCHome_Dicuss();
            if (opertype == "add")
            {
                dicuss.PKID = Guid.NewGuid();
                dicuss.DicussTopic = form["dicuss-name"];
                dicuss.Abstracts = form["dicuss-memo"];
                dicuss.groupuser = form["groupuser"];
                dicuss.AddUser = new Guid(form["AddUser"]);
                dicuss.CreateDate = DateTime.Now;
                dicuss.isshow = 1;
                uc.UCHome_Dicuss.AddObject(dicuss);
            }
            else
            {
                Guid pkid = new Guid(form["PKID"]);
                dicuss = uc.UCHome_Dicuss.SingleOrDefault(a => a.PKID == pkid);
                if (dicuss != null)
                {
                    dicuss.DicussTopic = form["dicuss-name"];
                    dicuss.Abstracts = form["dicuss-memo"];
                    dicuss.groupuser = form["groupuser"];
                }
            }
            try
            {
                uc.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddAlbumPhotoRel(FormCollection form)
        {
            int photos = 0;
            const int statuscode = 200;
            string album = Request["album"];
            string photoids = Request["photoids"];
            string[] photoidarray = photoids.Split(',');
            foreach (string photoid in photoidarray)
            {
                UCHome_Rel_AblumPhoto albump = new UCHome_Rel_AblumPhoto
                {
                    PKID = Guid.NewGuid(),
                    AlbumID = new Guid(album),
                    PhotoID = new Guid(photoid),
                    IsCover = 0
                };
                uc.UCHome_Rel_AblumPhoto.AddObject(albump);
                try
                {
                    uc.SaveChanges();
                    photos++;
                }
                catch (Exception)
                {
                    // ReSharper disable once RedundantJumpStatement
                    continue;
                }
            }
            JsonResult jr = new JsonResult { Data = new { statuscode, photos } };
            return Json(jr, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddHeadimg()
        {
            try
            {
                int startX = int.Parse(Request["x1"]);
                int startY = int.Parse(Request["y1"]);
                int w = int.Parse(Request["Iwidth"]);
                int h = int.Parse(Request["Iheight"]);
                HttpPostedFileBase photofile = Request.Files["exampleInputFile"];
                //保存上传文件到指定路径
                string webpath = SaveAsHeadImg(photofile, userid, startX, startY, w, h);
                JsonResult jr = new JsonResult { Data = new { statuscode = 200, photourl = webpath } };
                return Json(jr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                JsonResult jr = new JsonResult { Data = new { statuscode = 500, photourl = "找不到文件路径" } };
                return Json(jr, JsonRequestBehavior.AllowGet);
            }
        }
        protected string SaveAsHeadImg(HttpPostedFileBase file, Guid AddUser, int sx, int sy, int iw, int ih)
        {
            string fileurl = Guid.NewGuid().ToString();
            string fileext = Path.GetExtension(file.FileName);
            string fname = fileurl + fileext;
            string foldername = DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day;
            string basicPath = "/upload/headimg/" + AddUser;
            if (!Directory.Exists(Server.MapPath(basicPath + "/" + foldername + "/")))
            {
                Directory.CreateDirectory(Server.MapPath(basicPath + "/" + foldername + "/"));
            }
            string webpath = basicPath + "/" + foldername + "/" + fname;
            string webpath_thumbnail = basicPath + "/" + foldername + "/" + fileurl + "_thumbnail" + fileext;
            string webpath_min = basicPath + "/" + foldername + "/" + fileurl + "_min" + fileext;
            string webpath_headimg = basicPath + "/" + foldername + "/" + fileurl + "_headimg" + fileext;

            //单独存放用户头像
            const string basicPath1 = "/upload/headimages/";
            if (!Directory.Exists(Server.MapPath(basicPath1)))
            {
                Directory.CreateDirectory(Server.MapPath(basicPath1));
            }
            string headimages = basicPath1 + "/" + AddUser + ".jpg";



            string Upload_headimages = Server.MapPath(headimages);

            string UploadPath = Server.MapPath(webpath);
            string UploadPath_thumbnail = Server.MapPath(webpath_thumbnail);
            string UploadPath_min = Server.MapPath(webpath_min);
            string UploadPath_headimg = Server.MapPath(webpath_headimg);
            file.SaveAs(UploadPath);
            ImgReduceCutOut(sx, sy, iw, ih, UploadPath, UploadPath_thumbnail);
            SaveAsThumbil(160, 160, UploadPath_thumbnail, UploadPath_min);
            SaveAsThumbil(60, 60, UploadPath_thumbnail, UploadPath_headimg);

            SaveAsThumbil(60, 60, UploadPath_thumbnail, Upload_headimages);

            //Cookie ck = new Cookie();
            //string TempUserID = ck.GetCookie("UserserID");
            //保存文件到数据表
            UCHome_BaseInfo personInfo = uc.UCHome_BaseInfo.SingleOrDefault(p => p.UserID == AddUser);
            if (personInfo != null)
            {
                personInfo.headphoto = webpath_headimg;
                uc.SaveChanges();
                //更改头像cookies值
                HttpCookie cookie = Request.Cookies["SpaceInfo"];
                if (cookie != null)
                {
                    cookie.Values["HeadPic"] = EncryptUtils.Base64Encrypt(webpath_headimg);
                    Response.AppendCookie(cookie);
                }
                else
                {
                    cookie = new HttpCookie("SpaceInfo");
                    cookie.Values.Add("HeadPic", EncryptUtils.Base64Encrypt(webpath_headimg));
                    Response.AppendCookie(cookie);
                }
                return webpath_headimg;
            }
            return string.Empty;
        }
        /// <summary>
        /// 图片编辑
        /// </summary>
        /// <param name="sx">x轴偏离位置</param>
        /// <param name="sy">y轴偏离位置</param>
        /// <param name="iw">截取图片宽度</param>
        /// <param name="ih">截取图片高度</param>
        /// <param name="input_ImgUrl">服务器文件物理地址</param>
        /// <param name="out_ImgUrl">截取后新图片保存地址</param>
        public void ImgReduceCutOut(int sx, int sy, int iw, int ih, string input_ImgUrl, string out_ImgUrl)
        {
            //图片路径
            string ImgFile = input_ImgUrl;
            //遮罩小头像图片的宽度
            int PicWidth = iw;
            //遮罩小头像图片的高度
            int PicHeight = ih;
            //X坐标
            int PointX = sx;
            //Y坐标
            int PointY = sy;

            //按照指定的数据画出画板（位图）
            Image imgPhoto = Image.FromFile(ImgFile);
            Bitmap bmPhoto = new Bitmap(PicWidth, PicHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            //创建画笔并按照数据把指定的部分画到画板上
            using (Graphics gbmPhoto = Graphics.FromImage(bmPhoto))
            {
                gbmPhoto.SmoothingMode = SmoothingMode.AntiAlias;
                gbmPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gbmPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gbmPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, PicWidth, PicHeight), PointX,
                    PointY, PicWidth,
                    PicHeight, GraphicsUnit.Pixel);
                gbmPhoto.Dispose();
            }
            //把画好的画板保存到http数据流中
            string UploadPath = out_ImgUrl;
            bmPhoto.Save(UploadPath, ImageFormat.Jpeg);

            //施放所有资源
            imgPhoto.Dispose();
            bmPhoto.Dispose();
        }

        public ActionResult AddPhoto()
        {
            try
            {
                HttpPostedFileBase photofile = Request.Files["file"];
                //保存上传文件到指定路径
                string webpath = SaveAsFile(photofile, userid);
                string photoid = AddUCHome_Photos(photofile, userid, webpath);
                JsonResult jr = new JsonResult { Data = new { statuscode = 200, photourl = photoid } };
                return Json(jr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                JsonResult jr = new JsonResult { Data = new { statuscode = 500, photourl = "找不到文件路径" } };
                return Json(jr, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddThemes()
        {
            try
            {
                HttpPostedFileBase photofile = Request.Files["file"];
                //保存上传文件到指定路径
                string webpath = SaveAsFile(photofile, Guid.Empty);
                JsonResult jr = new JsonResult
                {
                    Data =
                        new
                        {
                            statuscode = 200,
                            fileid = Guid.NewGuid(),
                            filepath = webpath,
                            filename = photofile == null ? "" : photofile.FileName
                        }
                };
                return Json(jr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                JsonResult jr = new JsonResult { Data = new { statuscode = 500, photourl = "找不到文件路径" } };
                return Json(jr, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddFile()
        {
            try
            {
                HttpPostedFileBase photofile = Request.Files["file"];
                //保存上传文件到指定路径
                string webpath = SaveAsFile(photofile, userid, "homework");
                JsonResult jr = new JsonResult
                {
                    Data =
                        new
                        {
                            statuscode = 200,
                            fileid = Guid.NewGuid(),
                            filepath = webpath,
                            filename = photofile == null ? "" : photofile.FileName
                        }
                };
                return Json(jr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                JsonResult jr = new JsonResult { Data = new { statuscode = 500, photourl = "找不到文件路径" } };
                return Json(jr, JsonRequestBehavior.AllowGet);
            }
        }
        protected string SaveAsFile(HttpPostedFileBase file, Guid AddUser, string savetype = "photos")
        {
            string fileurl = Guid.NewGuid().ToString();
            string fileext = Path.GetExtension(file.FileName);
            string fname = fileurl + fileext;
            string foldername = DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day;
            string basicPath = "/upload/" + savetype + "/" + AddUser;
            if (!Directory.Exists(Server.MapPath(basicPath + "/" + foldername + "/")))
            {
                Directory.CreateDirectory(Server.MapPath(basicPath + "/" + foldername + "/"));
            }
            string webpath = basicPath + "/" + foldername + "/" + fname;
            string webpath_thumbnail = basicPath + "/" + foldername + "/" + fileurl + "_thumbnail" + fileext;
            string webpath_min = basicPath + "/" + foldername + "/" + fileurl + "_min" + fileext;
            string webpath_headimg = basicPath + "/" + foldername + "/" + fileurl + "_headimg" + fileext;
            string webpath_max = basicPath + "/" + foldername + "/" + fileurl + "_max" + fileext;
            string UploadPath = Server.MapPath(webpath);
            file.SaveAs(UploadPath);
            if (savetype == "photos")
            {
                string UploadPath_thumbnail = Server.MapPath(webpath_thumbnail);
                string UploadPath_min = Server.MapPath(webpath_min);
                string UploadPath_headimg = Server.MapPath(webpath_headimg);
                string UploadPath_max = Server.MapPath(webpath_max);
                SaveAsThumbil(170, 135, UploadPath, UploadPath_thumbnail);
                SaveAsThumbil(150, 180, UploadPath, UploadPath_min);
                SaveAsThumbil(80, 80, UploadPath, UploadPath_headimg);
                SaveAsThumbil(1000, 500, UploadPath, UploadPath_max);
            }
            return webpath;
            //Cookie ck = new Cookie();
            //string TempUserID = ck.GetCookie("UserserID");
            //保存文件到数据表

        }
        public string AddUCHome_Photos(HttpPostedFileBase file, Guid AddUser, string webpath)
        {
            UCHome_Photo photo = new UCHome_Photo
            {
                PKID = Guid.NewGuid(),
                PhotoName = file.FileName.Substring(0, file.FileName.LastIndexOf('.')),
                PhotoAbstract = "",
                IsAblumCover = "0",
                AddUser = AddUser,
                CreateTime = DateTime.Now,
                Hits = 0,
                flowers = 0,
                PhotoUrl = webpath,
                IsShow = 1
            };
            uc.UCHome_Photo.AddObject(photo);
            uc.SaveChanges();
            AddSNSFeedEntry feed = new AddSNSFeedEntry
            {
                ObjectType = "更新相册",
                ObjectID = photo.PKID.ToString(),
                UID = AddUser.ToString(),
                UName = DisPlayName,
                School = XXMC,
                Title = photo.CreateTime + "更新了相册",
                Summary = webpath,
                Date = DateTime.Now,
                //URL = "http://www.baidu.com",
                Like = 0,
                Favorite = 0
            };
            client.Send<AddSNSFeedEntry>(feed);
            return photo.PKID.ToString();
        }
        /// <summary>
        /// 缩略图
        /// </summary>
        /// <param name="w">要缩放的宽度</param>
        /// <param name="h">要缩放的高度</param>
        /// <param name="oPath">原始图片地址</param>
        /// <param name="tPath">缩放后新图片地址</param>
        public void SaveAsThumbil(int w, int h, string oPath, string tPath)
        {
            Bitmap originalBmp = new Bitmap(oPath);
            int left, top;
            if (originalBmp.Width <= w && originalBmp.Height <= h)
            {
                left = (int)Math.Round((decimal)(w - originalBmp.Width) / 2);
                top = (int)((decimal)(h - originalBmp.Height) / 2);
                //最终生成的图像
                Bitmap bmpOut = new Bitmap(w, h);
                using (Graphics graphics = Graphics.FromImage(bmpOut))
                {
                    // 设置高质量插值法              
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    // 清空画布并以白色背景色填充             
                    graphics.Clear(Color.Transparent);
                    //加上边框                 
                    //Pen pen = new Pen(ColorTranslator.FromHtml("#cccccc"));        
                    // graphics.DrawRectangle(pen, 0, 0, width - 1, height - 1);          
                    // 把源图画到新的画布上            
                    graphics.DrawImage(originalBmp, left, top);
                    graphics.Dispose();
                }
                bmpOut.Save(tPath);
                originalBmp.Dispose();
                bmpOut.Dispose();
                return;
            }
            int newWidth, newHeight;
            if (w * originalBmp.Height < h * originalBmp.Width)
            {
                newWidth = w;
                newHeight = (int)Math.Round((decimal)originalBmp.Height * w / originalBmp.Width);
                // 缩放成宽度跟预定义的宽度相同的，即left=0，计算top          
                left = 0;
                top = (int)Math.Round((decimal)(h - newHeight) / 2);
            }
            else
            {
                newWidth = (int)Math.Round((decimal)originalBmp.Width * h / originalBmp.Height);
                newHeight = h;
                // 缩放成高度跟预定义的高度相同的，即top=0，计算left     
                left = (int)Math.Round((decimal)(w - newWidth) / 2);
                top = 0;
            }
            // 生成按比例缩放的图，如：160*80的图         
            Bitmap bmpOut2 = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(bmpOut2))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                graphics.DrawImage(originalBmp, 0, 0, newWidth, newHeight);
                graphics.Dispose();

            }
            // 再把该图画到预先定义的宽高的画布上，如160*120          
            Bitmap lastbmp = new Bitmap(w, h);
            using (Graphics graphics = Graphics.FromImage(lastbmp))
            {
                // 设置高质量插值法          
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // 清空画布并以白色背景色填充      
                graphics.Clear(Color.White);
                //加上边框              
                //Pen pen = new Pen(ColorTranslator.FromHtml("#cccccc"));          
                //graphics.DrawRectangle(pen, 0, 0, width - 1, height - 1);        
                // 把源图画到新的画布上            
                graphics.DrawImage(bmpOut2, left, top);
                graphics.Dispose();
            }
            lastbmp.Save(tPath);//保存为文件，tpath 为要保存的路径 
            bmpOut2.Dispose();
            lastbmp.Dispose();
            originalBmp.Dispose();
        }
        public ActionResult PersonMark(Guid id)
        {
            Guid jsid = id;
            View_Simple_TeaInfo tea = uc.View_Simple_TeaInfo.SingleOrDefault(u => u.jsid == jsid);
            return PartialView("PersonMark", tea);
        }
        public ActionResult PersonMessage(Guid id)
        {
            return PartialView("PersonMessage", id);
        }
        public ActionResult PersonMessage2(Guid id)
        {
            return PartialView("PersonMessage2", id);
        }
        public ActionResult MessageBoard(Guid id)
        {
            ViewBag.UserID = id;
            UCHome_Leave ul =
                uc.UCHome_Leave.FirstOrDefault(
                    m => m.AcceptUserID == id && m.MessageUserID == id && m.msgtype == "master");
            if (ul != null)
            {
                return PartialView("MessageBoard", ul);
            }
            return PartialView("MessageBoard", new UCHome_Leave { PKID = Guid.NewGuid(), AcceptUserID = id });
        }
        public ActionResult MessageBoard2(Guid id)
        {
            ViewBag.UserID = id;
            UCHome_Leave ul =
                uc.UCHome_Leave.FirstOrDefault(
                    m => m.AcceptUserID == id && m.MessageUserID == id && m.msgtype == "master");
            if (ul != null)
            {
                return PartialView("MessageBoard2", ul);
            }
            return PartialView("MessageBoard2", new UCHome_Leave { PKID = Guid.NewGuid(), AcceptUserID = id });
        }
        public ActionResult LogList(Guid id, string uctype)
        {
            ViewBag.uctype = uctype;
            return PartialView("LogList", id);
        }
        public ActionResult LogAbstract(Guid id, string uctype = "article")
        {
            ViewBag.UCType = uctype;
            return PartialView("LogAbstract", id);
        }
        public ActionResult LogView(Guid pid, string uctype)
        {
            UCHome_PersonNew ucpn = uc.UCHome_PersonNew.SingleOrDefault(u => u.PKID == pid);
            UCHome_PersonNew nextucpn =
                uc.UCHome_PersonNew.Where(u => u.DeployTime < ucpn.DeployTime && u.UCType == uctype && u.AddUser == userid && u.IsShow == 1)
                    .OrderByDescending(u => u.DeployTime)
                    .FirstOrDefault();
            UCHome_PersonNew lastucpn =
    uc.UCHome_PersonNew.Where(u => u.DeployTime > ucpn.DeployTime && u.UCType == uctype && u.AddUser == userid && u.IsShow == 1)
        .OrderBy(u => u.DeployTime)
        .FirstOrDefault();
            ViewBag.NextArticle = nextucpn;
            ViewBag.LastArticle = lastucpn;
            //评论点赞
            ViewBag.LoginId = userid;
            try
            {
                var newClient = new JsonServiceClient(http + "/SNSApi/");
                SNSFeedEntryDto article = newClient.Get(new GetSNSFeed
                {
                    ObjectID = pid.ToString()
                });
                //Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
                ViewBag.Article = article;
            }
            catch (Exception)
            {
                ViewBag.Article = new SNSFeedEntryDto();
            }
            return PartialView("LogView", ucpn);
        }
        public ActionResult LogAbstract2(Guid id)
        {
            return PartialView("LogAbstract2", id);
        }
        public ActionResult LogView2(Guid pid, string uctype, Guid userid)
        {
            UCHome_PersonNew ucpn = uc.UCHome_PersonNew.SingleOrDefault(u => u.PKID == pid);
            UCHome_PersonNew nextucpn =
                uc.UCHome_PersonNew.Where(u => u.DeployTime < ucpn.DeployTime && u.UCType == uctype && u.AddUser == userid && u.IsShow == 1)
                    .OrderByDescending(u => u.DeployTime)
                    .FirstOrDefault();
            UCHome_PersonNew lastucpn =
    uc.UCHome_PersonNew.Where(u => u.DeployTime > ucpn.DeployTime && u.UCType == uctype && u.AddUser == userid && u.IsShow == 1)
        .OrderBy(u => u.DeployTime)
        .FirstOrDefault();
            ViewBag.NextArticle = nextucpn;
            ViewBag.LastArticle = lastucpn;
            ViewBag.LoginId = userid;
            try
            {
                var newClient = new JsonServiceClient(http + "/SNSApi/");
                SNSFeedEntryDto article = newClient.Get(new GetSNSFeed
                {
                    ObjectID = pid.ToString()
                });
                //Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
                ViewBag.Article = article;
            }
            catch (Exception)
            {
                ViewBag.Article = new SNSFeedEntryDto();
            }
            return PartialView("LogView2", ucpn);
        }
        public ActionResult LogCreate(Guid id)
        {
            return PartialView("LogCreate", id);
        }
        public ActionResult LogEdit(Guid id, Guid pid)
        {
            UCHome_PersonNew loginfo = uc.UCHome_PersonNew.SingleOrDefault(l => l.PKID == pid);
            return PartialView("LogEdit", loginfo);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddMessage(FormCollection form)
        {
            Guid uid = new Guid(form["AddUser"]);
            UCHome_Leave pn = uc.UCHome_Leave.SingleOrDefault(m => m.msgtype == "master" && m.AcceptUserID == uid && m.MessageUserID == uid);
            bool isexist = true;
            if (pn == null)
            {
                isexist = false;
                pn = new UCHome_Leave();
            }
            pn.Contents = form["Contents"];
            pn.AcceptUserID = uid;
            pn.MessageUserID = uid;
            pn.EditDate = DateTime.Now;
            pn.isshow = 1;
            pn.msgtype = "master";
            pn.flowers = 0;

            JsonResult Newjson = new JsonResult();
            try
            {
                if (!isexist)
                {
                    pn.PKID = Guid.NewGuid();
                    uc.UCHome_Leave.AddObject(pn);
                }
                uc.SaveChanges();
                UCHome_PersonNew pn2 = new UCHome_PersonNew
                {
                    PKID = Guid.NewGuid(),
                    AddUser = pn.MessageUserID,
                    UCType = "static",
                    Title = "留言板更新",
                    Abstract = string.Format("更新了留言板【{0}】（{1}）", "新寄语", pn.EditDate),
                    Content = pn.PKID.ToString(),
                    DeployTime = pn.EditDate,
                    IsShare = "9",
                    IsShow = 1,
                    IsAudit = 0,
                    WriteFrom = "message"
                };
                uc.UCHome_PersonNew.AddObject(pn2);
                uc.SaveChanges();
                Newjson.Data = new { statuscode = 0, message = "添加成功" };
            }
            catch (Exception)
            {
                Newjson.Data = new { statuscode = 1, message = "添加失败" };
            }
            return Json(Newjson, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddMessage2(FormCollection form)
        {
            Guid uid = new Guid(form["AddUser"]);
            //获取登录帐号
            Guid msguid =userid;
            JsonResult Newjson = new JsonResult();
            if (msguid != Guid.Empty)
            {
                UCHome_Leave pn = new UCHome_Leave
                {
                    PKID = Guid.NewGuid(),
                    Contents = form["Contents"],
                    AcceptUserID = uid,
                    MessageUserID = msguid,
                    EditDate = DateTime.Now,
                    isshow = 1,
                    msgtype = "message",
                    flowers = 0
                };

                try
                {
                    uc.UCHome_Leave.AddObject(pn);
                    uc.SaveChanges();
                    UCHome_PersonNew pn2 = new UCHome_PersonNew
                    {
                        PKID = Guid.NewGuid(),
                        AddUser = pn.AcceptUserID,
                        UCType = "static",
                        Title = "留言板更新",
                        Abstract = string.Format("有新的留言【{0}】（{1}）", "留言", pn.EditDate),
                        Content = pn.PKID.ToString(),
                        DeployTime = pn.EditDate,
                        IsShare = "9",
                        IsShow = 1,
                        IsAudit = 0,
                        WriteFrom = "message"
                    };
                    uc.UCHome_PersonNew.AddObject(pn2);
                    uc.SaveChanges();
                    Newjson.Data = new { statuscode = 0, message = "添加成功" };
                }
                catch (Exception)
                {
                    Newjson.Data = new { statuscode = 1, message = "添加失败" };
                }
            }
            else
            {
                Newjson.Data = new { statuscode = 1, message = "添加失败，检测未登录" };
            }
            return Json(Newjson, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddLog(FormCollection form)
        {

            UCHome_PersonNew pn = new UCHome_PersonNew
            {
                AddUser = new Guid(form["AddUser"]),
                DeployTime = DateTime.Now,
                Content = form["Content"],
                Hits = 0,
                PKID = Guid.NewGuid(),
                Title = form["Title"],
                UCType = form["UCType"],
                IsShare = form["IsShare"],
                IsAudit = 4,
                isTop = 0,
                IsShow = form["IsShow"].ToLower().Contains("true") ? 0 : 1,
                Abstract = form["Abstract"],
                flowers = 0
            };
            JsonResult Newjson = new JsonResult();
            try
            {
                uc.UCHome_PersonNew.AddObject(pn);
                uc.SaveChanges();
                UCHome_PersonNew pn2 = new UCHome_PersonNew
                {
                    PKID = Guid.NewGuid(),
                    AddUser = pn.AddUser,
                    UCType = "static",
                    Title = "文章更新",
                    Abstract = string.Format("发表了新文章【{0}】（{1}）", pn.Title, pn.DeployTime),
                    Content = pn.PKID.ToString(),
                    DeployTime = pn.DeployTime,
                    IsShare = pn.IsShare,
                    IsShow = pn.IsShow,
                    IsAudit = 0,
                    WriteFrom = pn.UCType
                };
                uc.UCHome_PersonNew.AddObject(pn2);
                uc.SaveChanges();
                if (pn.IsShare != "0" && pn.UCType != "log")
                {
                    AddSNSFeedEntry feed = new AddSNSFeedEntry
                    {
                        ObjectType = "文章日志",
                        ObjectID = pn.PKID.ToString(),
                        UID = pn.AddUser.ToString(),
                        UName = DisPlayName,
                        School = XXMC,
                        Title = form["Title"],
                        Summary = pn.Content,
                        Date = DateTime.Now,
                        URL = "/UCHome/PublicShare/logview2?pid=" + pn.PKID + "&uctype=article&userid=" + pn.AddUser + "",
                        Like = 0,
                        Favorite = 0
                    };
                    client.Send<AddSNSFeedEntry>(feed);
                }
                Newjson.Data = new { statuscode = 0, message = "添加成功" };
            }
            catch (Exception)
            {
                Newjson.Data = new { statuscode = 1, message = "添加失败" };
            }
            return Json(Newjson, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditLog(FormCollection form, Guid PKID)
        {
            JsonResult Newjson = new JsonResult();
            UCHome_PersonNew pn = uc.UCHome_PersonNew.SingleOrDefault(l => l.PKID == PKID);
            if (pn != null)
            {
                pn.AddUser = new Guid(form["AddUser"]);
                pn.DeployTime = DateTime.Now;
                pn.Content = form["Content"];
                pn.Title = form["Title"];
                pn.UCType = form["UCType"];
                pn.IsShare = form["IsShare"];
                pn.Abstract = form["Abstract"];
                pn.IsShow = form["IsShow"].ToLower().Contains("true") ? 0 : 1;
                try
                {
                    uc.SaveChanges();
                    //UCHome_PersonNew pn2 = new UCHome_PersonNew();
                    //pn2.PKID = Guid.NewGuid();
                    //pn2.AddUser = pn.AddUser;
                    //pn2.UCType = "static";
                    //pn2.Title = "文章更新";
                    //pn2.Abstract = string.Format("更新了文章【{0}】（{1}）", pn.Title, pn.DeployTime);
                    //pn2.Content = pn.PKID.ToString();
                    //pn2.DeployTime = pn.DeployTime;
                    //pn2.IsShare = pn.IsShare;
                    //pn2.IsShow = pn.IsShow;
                    //pn2.IsAudit = 0;
                    //pn2.WriteFrom = pn.UCType;
                    //uc.UCHome_PersonNew.AddObject(pn2);
                    //uc.SaveChanges();
                    if (pn.IsShare != "0" && pn.UCType != "log")
                    {
                        AddSNSFeedEntry feed = new AddSNSFeedEntry
                        {
                            ObjectType = "文章日志",
                            ObjectID = pn.PKID.ToString(),
                            UID = pn.AddUser.ToString(),
                            UName = DisPlayName,
                            School = XXMC,
                            Title = form["Title"],
                            Summary = pn.Content,
                            Date = DateTime.Now,
                            URL = "/UCHome/PublicShare/logview2?pid=" + pn.PKID + "&uctype=article&userid=" + pn.AddUser + "",
                            Like = 0,
                            Favorite = 0
                        };
                        client.Send<AddSNSFeedEntry>(feed);
                    }
                    Newjson.Data = new { statuscode = 0, message = "更新成功" };
                }
                catch (Exception)
                {
                    Newjson.Data = new { statuscode = 1, message = "更新失败" };
                }

            }
            else
            {
                Newjson.Data = new { statuscode = 2, message = "日志不存在" };
            }
            return Json(Newjson, JsonRequestBehavior.AllowGet);
        }
        public List<UserInfo> GetTeacherList(Guid bjid)
        {
            List<UserInfo> vctlist = uc.View_Class_TeaInfo.Where(tea => tea.bjid == bjid).AsEnumerable().Select(tea => tea.jsid != null ? new UserInfo { bjname = tea.xzbmc, username = tea.xm, userid = tea.jsid.Value, usertype = "T" } : null).Distinct().ToList();
            return vctlist;
        }
        public List<UserInfo> GetStudentList(Guid bjid)
        {
            List<UserInfo> vctlist = uc.View_Simple_StuInfo.Where(tea => tea.BJID == bjid).AsEnumerable().Select(tea => new UserInfo { bjname = tea.bjmc, username = tea.XM, userid = tea.xsid, usertype = "S" }).Distinct().ToList();
            return vctlist;
        }
        public List<UserInfo> GetParentList(Guid bjid)
        {
            List<UserInfo> vctlist = uc.View_Simple_StuInfo.Where(tea => tea.BJID == bjid).AsEnumerable().Select(tea => new UserInfo { bjname = tea.bjmc, username = tea.XM + "家长", userid = tea.xsid, usertype = "P" }).Distinct().ToList();
            return vctlist;
        }
        public class UserInfo
        {
            public string usertype { get; set; }
            public string username { get; set; }
            public Guid userid { get; set; }
            public string bjname { get; set; }
        }
        public void UrlRedirect(string returnurl, string sysname, bool isRedirect)
        {
            string tourl = returnurl;
            if (isRedirect)
            {
                //获取所在区的域名key值
                string hostkey;
                switch (AreaCode)
                {
                    case "370901":
                        hostkey = "ta" + sysname;
                        break;
                    case "370902":
                        hostkey = "ts" + sysname;
                        break;
                    case "370903":
                        hostkey = "dy" + sysname;
                        break;
                    case "370921":
                        hostkey = "ny" + sysname;
                        break;
                    case "370923":
                        hostkey = "dp" + sysname;
                        break;
                    case "370982":
                        hostkey = "xt" + sysname;
                        break;
                    case "370983":
                        hostkey = "fc" + sysname;
                        break;
                    case "370991":
                        hostkey = "gx" + sysname;
                        break;
                    case "370992":
                        hostkey = "jq" + sysname;
                        break;
                    default:
                        hostkey = "ta" + sysname;
                        break;
                }

                Uri fromurl = new Uri(returnurl);
                string host = fromurl.Host + ":" + fromurl.Port;
                string newhost = APIHelper.GetApiUrl(hostkey, dnsconfig);
                tourl = returnurl.Replace(host, newhost);
            }
            Response.Redirect(tourl);
        }
    }
}
