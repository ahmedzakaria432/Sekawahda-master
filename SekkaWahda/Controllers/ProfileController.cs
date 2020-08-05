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
                    UserName=User.UserName
                   
                
                };
                var CurrentUser = context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name);

                var UserTrip = User.trips.FirstOrDefault();
                Reservation ReservationOfUserOfThisTrip = new Reservation();
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
                        DriverTotalRate = User.DriverTotalRate


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
        public HttpResponseMessage UpdateProfile() 
        {
            try
            {
                    var currentUser = context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name);

                    var files = HttpContext.Current.Request.Files;
                    if (files.Count == 0) 
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "you must upload image of car");
                    }

                    currentUser.city = HttpContext.Current.Request.Form["city"];
                    currentUser.FullName = HttpContext.Current.Request.Form["FullName"];
                    currentUser.PhoneNumber = HttpContext.Current.Request.Form["PhoneNumber"];
                    currentUser.UserEmailID = HttpContext.Current.Request.Form["UserEmailID"];


                    
                    Car car = context.Cars.FirstOrDefault(c => c.UserId == currentUser.UserID);
                    if (car == null)
                        car = new Car() {UserId=currentUser.UserID };

                    car.carColor = HttpContext.Current.Request.Form["carColor"];
                    car.CarModel = HttpContext.Current.Request.Form["CarModel"];

                    
                    foreach (string file in files)
                    {
                        var PostedImage = files[file];
                        var path= HttpContext.Current.Server.MapPath("~/");
                        var fileNameWithExtension= PostedImage.FileName.NoRepeateInFileName(RequestContext.Principal.Identity.Name);
                        var CarImagePath = HttpContext.Current.Server.MapPath(path + fileNameWithExtension);
                        car.CarImagePath = CarImagePath;
                        car.CarImageRelativeUrl = $"~/{fileNameWithExtension}";
                        PostedImage.SaveAs(CarImagePath);

                    }


                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "your Profile updated successfully");





            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.OK, ex.Message);
            }
        }

    }
}
 