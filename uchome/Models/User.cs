using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UCHome.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid SchoolId { get; set; }
    }
}