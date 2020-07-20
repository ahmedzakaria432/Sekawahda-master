using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SekkaWahda.Models
{ 
    [MetadataType(typeof(TripValidated))]
    public partial class trip {


    }
    public class TripValidated
    {
        [Required(ErrorMessage = "You must enter Place of trip")]
        public string FromCity { get; set; }
        [Required(ErrorMessage = "You must enter where will you go")]
        public string ToCity { get; set; }
        [Required(ErrorMessage = "You must enter place to meet")]
        public string PlaceToMeet { get; set; }
        [Required(ErrorMessage = "You must enter Date Of Trip")]
        public System.DateTime DateOfTrip { get; set; }
        [Required(ErrorMessage = "You must enter Time Of Trip")]
        public System.TimeSpan TimeOfTrip { get; set; }

        public Nullable<int> DriverId { get; set; }

        
    }
}