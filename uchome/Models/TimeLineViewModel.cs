using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UCHome.Models
{
    /// <summary>
    /// 时光轴类
    /// </summary>
    public partial class TimeLineViewModel
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 图片Http前缀地址
        /// </summary>
        public string ImageUrl { get; set; }
    }
}