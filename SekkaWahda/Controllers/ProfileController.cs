using SekkaWahda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SekkaWahda.Controllers
{
    public class ProfileController : ApiController
    {
        SECURITY_DBEntities context = new SECURITY_DBEntities();
        public HttpResponseMessage GetProfile(int id)
        {
            try
            {
                var User = context.UserMasters.Find(id);
                if (User==null) { return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user was not found"); }
                var car = User.Cars.FirstOrDefault();
                if (car == null) 
                {
                    car.carColor = string.Empty;
                    car.CarImageRelativeUrl = string.Empty;
                    car.CarModel = string.Empty;
                    car.CarLicense = string.Empty;
                    
                }
                ProfileDto profileDto;
                
                 profileDto = new ProfileDto() 
                {
                    carColor=car.carColor,
                    CarImageUrl=car.CarImageRelativeUrl,
                    CarLicense=car.CarLicense,
                    CarModel=car.CarModel,
                    city=User.city,
                    DriverLicense=User.DriverLicense,
                    FullName=User.FullName,
                    
                    PhoneNumber=User.PhoneNumber,
                    SSN=User.SSN,
                    UserEmailID=User.UserEmailID,
                    UserID=User.UserID,
                    UserName=User.UserName
                
                };
                var CurrentUser = context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name);

                var ReservationOfUserOfThisTrip = CurrentUser.Reservations.FirstOrDefault(r=>r.TripId==User.trips.FirstOrDefault().ID);
                if (User.UserName == RequestContext.Principal.Identity.Name)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, profileDto);
                }

                else if ((ReservationOfUserOfThisTrip == null) || ReservationOfUserOfThisTrip.Accebted == false || ReservationOfUserOfThisTrip.Accebted == null)
                {
                    var ProfileNotAccebted = new
                    {
                        carColor = car.carColor,
                        CarImageUrl = car.CarImageRelativeUrl,
                        CarLicense = car.CarLicense,
                        CarModel = car.CarModel,
                        city = User.city,
                        FullName = User.FullName,
                        UserName = User.UserName


                    };
                    return Request.CreateResponse(HttpStatusCode.OK, ProfileNotAccebted);

                }
                else if (ReservationOfUserOfThisTrip.Accebted == true)
                {
                    var ProfileAccebted = new
                    {
                        carColor = car.carColor,
                        CarImageUrl = car.CarImageRelativeUrl,
                        CarLicense = car.CarLicense,
                        CarModel = car.CarModel,
                        city = User.city,
                        FullName = User.FullName,
                        UserName = User.UserName,
                        PhoneNumber = User.PhoneNumber,
                        SSN = User.SSN,
                        UserEmailID = User.UserEmailID,


                    };
                    return Request.CreateResponse(HttpStatusCode.OK, ProfileAccebted);

                }
                else return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "something went wrong ");

            }
            catch (Exception ex) 
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,ex.Message);
            }



        }
    }
}
 