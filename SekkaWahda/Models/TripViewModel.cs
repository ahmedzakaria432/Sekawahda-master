using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SekkaWahda.Models
{
    public class TripViewModel
    {
        public int ID { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public string PlaceToMeet { get; set; }
        public System.DateTime DateOfTrip { get; set; }
        public System.TimeSpan TimeOfTrip { get; set; }
        public Nullable<int> DriverId { get; set; }

    }
}