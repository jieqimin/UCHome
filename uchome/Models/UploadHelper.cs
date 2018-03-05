using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;

namespace UCHome.Models {
	public class UploadHelper {
		protected string SaveAsFile(HttpPostedFileBase file, Guid AddUser, string savetype = "photos") {
			string fileurl = Guid.NewGuid().ToString();
			string fileext = Path.GetExtension(file.FileName);
			string fname = fileurl + fileext;
			string foldername = DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day;
			string basicPath = "/upload/" + savetype + "/" + AddUser;
			if (!Directory.Exists(HttpContext.Current.Server.MapPath(basicPath + "/" + foldername + "/"))) {
				Directory.CreateDirectory(HttpContext.Current.Server.MapPath(basicPath + "/" + foldername + "/"));
			}
			string webpath = basicPath + "/" + foldername + "/" + fname;
			string UploadPath = HttpContext.Current.Server.MapPath(webpath);
			file.SaveAs(UploadPath);
			if (savetype == "photos") {
				string webpath_thumbnail = basicPath + "/" + foldername + "/" + fileurl + "_thumbnail" + fileext;
				string webpath_min = basicPath + "/" + foldername + "/" + fileurl + "_min" + fileext;
				string webpath_headimg = basicPath + "/" + foldername + "/" + fileurl + "_headimg" + fileext;
				string webpath_max = basicPath + "/" + foldername + "/" + fileurl + "_max" + fileext;
				string UploadPath_thumbnail = HttpContext.Current.Server.MapPath(webpath_thumbnail);
				string UploadPath_min = HttpContext.Current.Server.MapPath(webpath_min);
				string UploadPath_headimg = HttpContext.Current.Server.MapPath(webpath_headimg);
				string UploadPath_max = HttpContext.Current.Server.MapPath(webpath_max);
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
		public string SaveAsPublicFile(HttpPostedFileBase file,string savename,string savetype = "public") {
			string fileext =".xls";
			string fname = savename + fileext;
			string basicPath = "/upload/" + savetype + "/";
			if (!Directory.Exists(HttpContext.Current.Server.MapPath(basicPath))) {
				Directory.CreateDirectory(HttpContext.Current.Server.MapPath(basicPath));
			}
			string webpath = basicPath + fname;
			string UploadPath = HttpContext.Current.Server.MapPath(webpath);
			file.SaveAs(UploadPath);			
			return webpath;
		}
		public void SaveAsThumbil(int w, int h, string oPath, string tPath) {
			Bitmap originalBmp = new Bitmap(oPath);
			int left, top;
			if (originalBmp.Width <= w && originalBmp.Height <= h) {
				left = (int)Math.Round((decimal)(w - originalBmp.Width) / 2);
				top = (int)((decimal)(h - originalBmp.Height) / 2);
				//最终生成的图像
				Bitmap bmpOut = new Bitmap(w, h);
				using (Graphics graphics = Graphics.FromImage(bmpOut)) {
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
			if (w * originalBmp.Height < h * originalBmp.Width) {
				newWidth = w;
				newHeight = (int)Math.Round((decimal)originalBmp.Height * w / originalBmp.Width);
				// 缩放成宽度跟预定义的宽度相同的，即left=0，计算top          
				left = 0;
				top = (int)Math.Round((decimal)(h - newHeight) / 2);
			}
			else {
				newWidth = (int)Math.Round((decimal)originalBmp.Width * h / originalBmp.Height);
				newHeight = h;
				// 缩放成高度跟预定义的高度相同的，即top=0，计算left     
				left = (int)Math.Round((decimal)(w - newWidth) / 2);
				top = 0;
			}
			// 生成按比例缩放的图，如：160*80的图         
			Bitmap bmpOut2 = new Bitmap(newWidth, newHeight);
			using (Graphics graphics = Graphics.FromImage(bmpOut2)) {
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
				graphics.DrawImage(originalBmp, 0, 0, newWidth, newHeight);
				graphics.Dispose();

			}
			// 再把该图画到预先定义的宽高的画布上，如160*120          
			Bitmap lastbmp = new Bitmap(w, h);
			using (Graphics graphics = Graphics.FromImage(lastbmp)) {
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
	}
}