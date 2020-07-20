using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SekkaWahda.Models
{
    public class UserMasterErrorModel
    {
        public UserMasterModel User{get;set;}
        public string ErrorMessage{ get; set; }
    }
}