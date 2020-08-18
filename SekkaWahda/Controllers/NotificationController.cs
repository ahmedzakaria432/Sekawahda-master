using SekkaWahda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SekkaWahda.Controllers
{
    [Authorize]
    public class NotificationController : ApiController
    {
        SECURITY_DBEntities context = new SECURITY_DBEntities();

   public HttpResponseMessage GetNotifications()
        {
            try
            {
                var ListOfNotifications = new List<object>();

                var notifications = context.notification_.Where(n => n.ReceiverID == context.UserMasters
                .FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name).UserID).ToList();
                foreach (var notification in notifications)
                {
                    switch (notification.TypeOfNotification)
                    {
                        case "RequestReserveTrip":
                            ListOfNotifications.Add(new
                            {
                                notification.Message_,
                                notification.TypeOfNotification,
                                TripID=context.trips.FirstOrDefault(t => t.DriverId == notification.RaiserID).ID
                            });
                            break;
                        case "TripUpdated":
                            ListOfNotifications.Add(new
                            {
                                notification.Message_,
                                notification.TypeOfNotification,
                                TripID=context.trips.FirstOrDefault(t => t.DriverId == notification.RaiserID).ID

                            });

                            break;

                        case "TripDeleted":
                            ListOfNotifications.Add(new { notification.Message_, notification.TypeOfNotification });
                            break;
                        case "TripReserved":
                            ListOfNotifications.Add(new
                            {
                                notification.Message_,
                                notification.TypeOfNotification,
                                TripID=context.trips.FirstOrDefault(t => t.DriverId == notification.RaiserID).ID
                            });
                            break;
                        case "ReserveAccepredDriver":
                            ListOfNotifications.Add(new
                            {
                                notification.Message_,
                                notification.TypeOfNotification,
                                notification.RaiserID
                            });
                            break;
                        case "reserveAcceptedTraveller":
                            ListOfNotifications.Add(new
                            {
                                notification.Message_,
                                notification.TypeOfNotification,
                                
                                TripID=context.trips.FirstOrDefault(t => t.DriverId == notification.RaiserID).ID
                            });
                            break;
                        
                        case "ReserveNotAccepted":
                            ListOfNotifications.Add(new
                            {
                                notification.Message_,
                                notification.TypeOfNotification,

                            });
                            break;
                        default:
                            break;
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, ListOfNotifications);
            }
            catch (Exception ex) 
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

            }
        }

    }
}
