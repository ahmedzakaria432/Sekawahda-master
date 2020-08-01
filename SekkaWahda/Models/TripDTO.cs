﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SekkaWahda.Models
{
    public class TripDTO
    {
        [Required (ErrorMessage ="You should enter from where you will travel")]
        public string FromCity { get; set; }
        [Required(ErrorMessage =  "You should enter to where you will travel")]
        public string ToCity { get; set; }
        [Required (ErrorMessage = "You should enter from where you will meet traveller")]
        public string PlaceToMeet { get; set; }
        [Required(ErrorMessage = "You should enter from the date you will travel in")]
        public System.DateTime DateOfTrip { get; set; }
        [Required(ErrorMessage = "You should enter from when you will meet traveller")]
        public System.TimeSpan TimeOfTrip { get; set; }

        public Nullable<int> DriverId { get; set; }


    }
}