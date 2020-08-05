using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SekkaWahda.Models
{
    public class RateDto
    {
        public int RatingId { get; set; }
        
        public Nullable<int> TravellerId { get; set; }
        [Required]
        public Nullable<int> DriverId { get; set; }
        [Required]
        public Nullable<byte> RateValue { get; set; }
    }
}