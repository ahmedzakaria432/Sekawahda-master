using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SekkaWahda.Models
{
    public class LicenseModel
    {
        [Required(ErrorMessage = "your Car License is required")]
        public string CarLicense { get; set; }
        [Required(ErrorMessage = "your Driver License is required")]
        public string DriverLicense { get; set; }
}
}