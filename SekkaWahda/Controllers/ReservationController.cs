using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SekkaWahda.Models;
namespace SekkaWahda.Controllers
{
    public class ReservationController : ApiController
    {
        SECURITY_DBEntities context = new SECURITY_DBEntities();
        [HttpPost]
        [ActionName("MakeResrvation")]
        public HttpResponseMessage MakeResrvation(int TripID) {
            if (context.Reservations.Where(r => r.TripId == TripID).FirstOrDefault() != null) {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "This trip is already reserved");
            }
            var CurrentUserID = context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name).UserID;
            var reservation = new Reservation();
            reservation.TravellerId = CurrentUserID;
            reservation.TripId = TripID;
            
            try {
                context.Reservations.Add(reservation);
                context.notification_.Add(new notification_ 
                {Message_= "you requested to reserve this trip, please send 10.LE to this vodafone cash 01012345678 " +
                "and you will be notified when your request be accepted",
                RaiserID=context.trips.FirstOrDefault(t=>t.ID==reservation.TripId).DriverId,
                ReceiverID=reservation.TravellerId,
                TypeOfNotification="RequestReserveTrip"

                });

                context.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK,"your request have been sent");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }


        }
        [HttpDelete]

        [ActionName("CancelReservation")]
        public HttpResponseMessage CancelReservation(int TripID)
        {
            var ReservationID =context.Reservations.FirstOrDefault(r=>r.TripId==TripID).ID;
            if (ReservationID==default(int))
                return Request.CreateResponse(HttpStatusCode.OK, "Reservation was not found");
            var CurrentUserID = context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name).UserID;
            if (CurrentUserID != context.Reservations.FirstOrDefault(r => r.ID == ReservationID).TravellerId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "You are Unauthorized to delete this reservation");
            }
            try {
                context.Reservations.Remove(context.Reservations.FirstOrDefault(r => r.ID == ReservationID));
                context.SaveChanges();
               return Request.CreateResponse(HttpStatusCode.OK, "Canceled successfully");

            }
            catch (Exception ex) {


                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            } 


        }
        [HttpGet]
        public HttpResponseMessage GetReservedTrip(int ReservationID)
        {
            var reservation = context.Reservations.FirstOrDefault(r => r.ID == ReservationID);
            try
            {
                trip ReservedTrip = context.trips.FirstOrDefault(t => t.ID == reservation.TripId);
                return Request.CreateResponse(HttpStatusCode.OK, ReservedTrip);


            }
            catch (Exception ex)
            {


                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

        }




    }
}
