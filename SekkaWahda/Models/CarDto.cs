using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SekkaWahda.Models
{
    public class CarDto
    {
        [Required]
        public string CarModel { get; set; }

        public string carColor { get; set; }

        public string CarUrl { get; set; }

        public Nullable<int> UserId { get; set; }
    }
}