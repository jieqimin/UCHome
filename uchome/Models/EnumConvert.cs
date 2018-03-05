namespace UCHome.Models
{
    public class EnumConvert
    {
        public enum IsShare
        {
            私有 = 0,
            对好友公开 = 1,
            对同学公开 = 2,
            对老师公开 = 3,
            对学校公开 = 4,
            对本区公开 = 5,
            对全市公开 = 6,
            完全公开 = 9
        }
        public enum AlbumThemes
        {
            其它 = 0,
            普通 = 1,
            教师 = 2,
            同学 = 3,
            校友 = 4,
            朋友 = 5,
            家人 = 6,
        }
        public enum DataType
        {
            文章 = 1,
            教师 = 2,
            班级 = 3,
            学校 = 4,
            学生 = 5
        }
        public enum AlbumType
        {
            活动 = 1,
            风景 = 2,
            最爱 = 3,
            旅游 = 4,
            生活 = 5,
            其他 = 0
        }
        public enum IsAudit
        {
            待审核 = 0,
            班级通过 = 1,
            校级通过 = 2,
            区级通过 = 3,
            市级通过 = 4
        }

        public enum IsShow
        {
            草稿 = 0,
            发布 = 1
        }
        public static string ConvertAudit(int auditvalue)
        {
            switch (auditvalue)
            {
                case 1:
                    return "班级通过";
                case 2:
                    return "校级通过";
                case 3:
                    return "区级通过";
                case 4:
                    return "市级通过";
                default:
                    return "待审核";
            }
        }
        public static string ConvertShare(string sharevalue)
        {
            switch (sharevalue)
            {
                case "1":
                    return "对好友公开";
                case "2":
                    return "对同学公开";
                case "3":
                    return "对老师公开";
                case "4":
                    return "对学校公开";
                case "5":
                    return "对本区公开";
                case "6":
                    return "对全市公开";
                case "9":
                    return "完全公开";
                default:
                    return "私有";
            }
        }
        public static string ConvertAlbumThemes(string sharevalue)
        {
            switch (sharevalue)
            {
                case "1":
                    return "普通";
                case "2":
                    return "教师";
                case "3":
                    return "同学";
                case "4":
                    return "校友";
                case "5":
                    return "朋友";
                case "6":
                    return "家人";
                case "0":
                    return "其它";
                default:
                    return "普通";
            }
        }
        public static string ConvertAlbumType(string sharevalue)
        {
            switch (sharevalue)
            {
                case "1":
                    return "活动";
                case "2":
                    return "风景";
                case "3":
                    return "最爱";
                case "4":
                    return "旅游";
                case "5":
                    return "生活";
                case "0":
                    return "其他";
                default:
                    return "其他";
            }
        }
        public static string ConvertShow(int auditvalue)
        {
            switch (auditvalue)
            {
                case 1:
                    return "已发布";
                default:
                    return "草稿";
            }
        }
    }
}