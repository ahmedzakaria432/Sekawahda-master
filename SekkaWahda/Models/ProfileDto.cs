using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SekkaWahda.Models
{
    public class ProfileDto
    {
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string UserEmailID { get; set; }

        public string city { get; set; }

        public string SSN { get; set; }

        public string DriverLicense { get; set; }

        public string FullName { get; set; }

        

        public string PhoneNumber { get; set; }

        

        public string CarModel { get; set; }

        public string carColor { get; set; }

        public string CarLicense { get; set; }
        
        public string CarImageUrl { get; set; }
        public Nullable<decimal> DriverTotalRate { get; set; }
        public string TypeOfProfile = "UserProfile";



    }
}