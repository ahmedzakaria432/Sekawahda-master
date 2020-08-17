using SekkaWahda.ExtensionMethods;
using SekkaWahda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;


namespace SekkaWahda.Controllers
{
    [Authorize]
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
                    car = new Car();
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
                    DriverTotalRate = User.DriverTotalRate,
                    PhoneNumber=User.PhoneNumber,
                    SSN=User.SSN,
                    UserEmailID=User.UserEmailID,
                    UserID=User.UserID,
                    UserName=User.UserName,
                    
                   
                
                };
                if (profileDto.DriverTotalRate == null)
                    profileDto.DriverTotalRate = 0;
                var CurrentUser = context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name);

                var UserTrip = User.trips.FirstOrDefault();
                Reservation ReservationOfUserOfThisTrip =default(Reservation);
                if (UserTrip != null)
                    ReservationOfUserOfThisTrip = CurrentUser.Reservations.FirstOrDefault(r => r.TripId == UserTrip.ID);

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
                        DriverTotalRate = User.DriverTotalRate,
                        CarModel = car.CarModel,
                        city = User.city,
                        FullName = User.FullName,
                        UserName = User.UserName,
                        User.ImageUrl,
                        TypeOfProfile = "ProfileNotReserved"



                    };
                    return Request.CreateResponse(HttpStatusCode.OK, ProfileNotAccebted);

                }
                else if (ReservationOfUserOfThisTrip.Accebted == true)
                {
                    var ProfileAccebted = new
                    {
                        User.ImageUrl,
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
                        DriverTotalRate = User.DriverTotalRate,
                        TypeOfProfile= "ProfileReserved"

                    };
                    return Request.CreateResponse(HttpStatusCode.OK, ProfileAccebted);

                }
                else return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "something went wrong ");

            }
            catch (Exception ex) 
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,ex);
            }



        }
        [HttpPost]
        public HttpResponseMessage UpdateProfile() 
        {
            try
            {
                /*city   , FullName   , PhoneNumber,  UserEmailID,   carColor, CarModel, image file ,  CarLicense  */

                var currentUser = context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name);

                var files = HttpContext.Current.Request.Files;

                if (HttpContext.Current.Request.Form["city"] != null && HttpContext.Current.Request.Form["city"] != string.Empty)
                { currentUser.city = HttpContext.Current.Request.Form["city"]; }
                if (HttpContext.Current.Request.Form["FullName"] != null && HttpContext.Current.Request.Form["FullName"] != string.Empty)
                { currentUser.FullName = HttpContext.Current.Request.Form["FullName"]; }
                if (HttpContext.Current.Request.Form["PhoneNumber"] != null && HttpContext.Current.Request.Form["PhoneNumber"] != string.Empty)
                { currentUser.PhoneNumber = HttpContext.Current.Request.Form["PhoneNumber"]; }

                if (HttpContext.Current.Request.Form["UserEmailID"] != null && HttpContext.Current.Request.Form["UserEmailID"] != string.Empty)
                { currentUser.UserEmailID = HttpContext.Current.Request.Form["UserEmailID"]; }

                if (currentUser.DriverLicense == null || currentUser.DriverLicense == string.Empty)
                {
                    if (HttpContext.Current.Request.Form["DriverLicense"] != null && HttpContext.Current.Request.Form["DriverLicense"] != string.Empty)
                    { currentUser.DriverLicense = HttpContext.Current.Request.Form["DriverLicense"]; }


                }


                Car car = context.Cars.FirstOrDefault(c => c.UserId == currentUser.UserID);
                if (car == null)
                {
                    


                    car = new Car() { UserId = currentUser.UserID,CarLicense="" };
                    context.Cars.Add(car);

                }
                var tset = HttpContext.Current.Request.Form["carColor"];
                if ( (tset != null && tset != string.Empty))
                { car.carColor = HttpContext.Current.Request.Form["carColor"]; }
                if (HttpContext.Current.Request.Form["CarModel"] != null && HttpContext.Current.Request.Form["CarModel"] != string.Empty)
                { car.CarModel = HttpContext.Current.Request.Form["CarModel"]; }
                if (HttpContext.Current.Request.Form["CarLicense"] != null && HttpContext.Current.Request.Form["CarModel"] != string.Empty)
                { car.CarLicense = HttpContext.Current.Request.Form["CarLicense"]; }




                foreach (string file in files)
                    {
                        var PostedImage = files[file];
                        var fileNameWithExtension= PostedImage.FileName.NoRepeateInFileName(RequestContext.Principal.Identity.Name);
                        var CarImagePath = HttpContext.Current.Server.MapPath("~/" + fileNameWithExtension);
                        car.CarImagePath = CarImagePath;
                        car.CarImageRelativeUrl = $"~/{fileNameWithExtension}";
                        PostedImage.SaveAs(CarImagePath);

                    }


                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "your Profile updated successfully");





            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

    }
}
 