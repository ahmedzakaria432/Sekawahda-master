using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SekkaWahda.Models
{
    public class SearchForTripDto
    {
        
        [Required(ErrorMessage ="you must enter the city you want to travel from")]
        public string FromCity { get; set; }

        [Required(ErrorMessage ="you must enter your destination")]
        public string ToCity { get; set; }

        [Required(ErrorMessage ="you must enter the date you want travel in")]
        public System.DateTime DateOfTrip { get; set; }


    }
}