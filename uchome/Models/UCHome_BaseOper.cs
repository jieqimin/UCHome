using System;
using System.Text;

namespace UCHome.Models
{
    public class UCHome_BaseOper
    {
        //图片大小转换
        public static string ConvertThumbnail(string Path, string imgtype)
        {
            string thumbnailpath = string.Empty;
            if (!string.IsNullOrEmpty(Path))
            {
                string ext = Path.Substring(Path.LastIndexOf('.'));
                string newext;
                switch (imgtype)
                {
                    case "thumbnail":
                        newext = "_thumbnail" + ext;
                        break;
                    case "mini":
                        newext = "_mini" + ext;
                        break;
                    case "headimg":
                        newext = "_headimg" + ext;
                        break;
                    case "max":
                        newext = "_max" + ext;
                        break;
                    default:
                        newext = ext;
                        break;
                }
                thumbnailpath = Path.Replace(ext, newext);
            }
            return thumbnailpath;
        }

        public static string EncodeBase64(string code)
        {
            return EncodeBase64("utf-8", code);
        }
        public static string EncodeBase64(string code_type, string code)
        {
            string encode = string.Empty;
            if (!string.IsNullOrEmpty(code))
            {

                byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
                try
                {
                    encode = Convert.ToBase64String(bytes);
                }
                catch
                {
                    encode = code;
                }
            }
            return encode;
        }

        public static string CutStr(string str, int len)
        {
            int l = str.Length;
            if (l > len)
            {
                return str.Substring(0, len)+"…";
            }
            return str;
        }
    }
}