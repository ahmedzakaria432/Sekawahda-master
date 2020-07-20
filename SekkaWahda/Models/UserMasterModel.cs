using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using SekkaWahda.Models;

namespace SekkaWahda.Models
{
    public class UserMasterModel
    {

        


        [Required( ErrorMessage ="You must enter User Name")]
       
        public string UserName { get; set; }
        [Required(ErrorMessage = "You must enter Password")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",ErrorMessage = " At least one upper case english letter ,At least one lower case english letter ,At least one digit ,At least one special character and Minimum 8 in length")]

        public string UserPassword { get; set; }
        
        [EmailAddress]
        [Required(ErrorMessage = "You must enter E-mail")]
        public string UserEmailID { get; set; }

        public string city { get; set; }
        [Required(ErrorMessage = "You must enter your SSN")]
        public string SSN { get; set; }
        [Required(ErrorMessage = "You must enter your Phone Number")]
        public string PhoneNumber { get; set; }

       





    }


}
