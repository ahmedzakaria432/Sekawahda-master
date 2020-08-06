using SekkaWahda.ExtensionMethods;
using SekkaWahda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SekkaWahda.Controllers
{
    public class RatingController : ApiController
    {
        SECURITY_DBEntities context = new SECURITY_DBEntities();
      [HttpPost]
        public HttpResponseMessage MakeRate(RateDto rateDto) 
        {
            try
            {
                if (ModelState.IsValid)
                {

                    
                    var CurrentUser = context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name);
                    var Driver = context.UserMasters.Find(rateDto.DriverId);
                    if (Driver == null)
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "this driver was not found");

                   var Reservation = context.Reservations.FirstOrDefault(r => r.TravellerId == CurrentUser.UserID && context.trips.FirstOrDefault(t => t.ID == r.ID).DriverId == Driver.UserID);
                    if (Reservation == null)
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "you can't rate this car owner because you didn't travel with him before");

                    var RecentlyRated = context.Ratings.FirstOrDefault(r => r.TravellerId == CurrentUser.UserID && r.DriverId == Driver.UserID);
                    if (RecentlyRated != null)
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "you have recently rated this user");
                    
                    Rating rate = new Rating()
                    {
                        DriverId = rateDto.DriverId,
                        TravellerId = CurrentUser.UserID,
                        RateValue = rateDto.RateValue,


                    };

                    
                    Driver.DriverTotalRate = Driver.CalcTotalRate();
                    context.Ratings.Add(rate);
                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "rate was made successfully");
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "you didnt complete some required fields");
            }
            catch (Exception ex) 
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            


        }
    }
}
