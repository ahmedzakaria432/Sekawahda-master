using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SekkaWahda.Models
{
    public class UserDto
    {
        public int UserID { get; set; }

        public string UserName { get; set; }
        
        public string UserEmailID { get; set; }

        public string city { get; set; }

        public string FullName { get; set; }
        
        public string PhoneNumber { get; set; }

    }
}