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

     /*   public HttpResponseMessage GetNotifications()
        { var list = new List<object>();

            var notifications = context.notification_.Where(n => n.ReceiverID == context.UserMasters
            .FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name).UserID).ToList();
            foreach (var notification in notifications)
            {
                switch (notification.TypeOfNotification)
                {
                    case "RequestReserveTrip":
                        list.Add(new {notification.Message_,
                            context.UserMasters.FirstOrDefault(u=>u.UserID==notification.RaiserID).ImageUrl,
                            context.trips.FirstOrDefault(t=>t.DriverId==notification.RaiserID).ID
                        });
                        break;
                    case "TripUpdated":
                        list.Add(new 
                        {
                            notification.Message_,
                            context.UserMasters.FirstOrDefault(u => u.UserID == notification.RaiserID).ImageUrl,
                            context.trips.FirstOrDefault(t => t.DriverId == notification.RaiserID).ID

                        });

                        break;

                    case "TripDeleted":
                        list.Add
                        break;

                    default:
                        break;
                }
            }

        }
*/
    }
}
