using SekkaWahda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SekkaWahda.ExtensionMethods;
using System.IO;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace SekkaWahda.Controllers
{
    [Authorize]

    public class CarController : ApiController
    {
        SECURITY_DBEntities context = new SECURITY_DBEntities();
        [HttpPost]
        public HttpResponseMessage AddCar()
        {
            try
            {

                var CarToAdd = new Car();
                var form = HttpContext.Current.Request.Form;
                    CarToAdd.carColor = HttpContext.Current.Request.Form["carColor"];
                CarToAdd.CarModel = HttpContext.Current.Request.Form["CarModel"];
                CarToAdd.CarLicense= HttpContext.Current.Request.Form["CarLicense"];
                CarToAdd.UserId = context.UserMasters.FirstOrDefault(u => u.UserName == (RequestContext.Principal.Identity.Name)).UserID;

                var files = HttpContext.Current.Request.Files;
                if (files == null||files.Count==0)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "You must upload photo of car");
                foreach (string file in files)
                {
                    var PostedImage = files[file];
                    var filename = PostedImage.FileName.NoRepeateInFileName(RequestContext.Principal.Identity.Name);
                    var postedImagePath = HttpContext.Current.Server.MapPath("~/" + filename);
                    PostedImage.SaveAs(postedImagePath);

                    CarToAdd.CarImagePath = postedImagePath;
                    
                }
                
                context.Cars.Add(CarToAdd);
               
                    context.SaveChanges();
               
                               return Request.CreateResponse(HttpStatusCode.OK, "Car added successfully");
            }
            catch (Exception ex) 
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        [HttpPut]
        public HttpResponseMessage UpdateCarImage()
        {
            try {
                var files = HttpContext.Current.Request.Files;
                if (files == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "You must upload photo of car");
                foreach (string file in files)
                {
                    var PostedImage = files[file];
                    var filename = PostedImage.FileName.NoRepeateInFileName(RequestContext.Principal.Identity.Name);
                    var postedImagePath = HttpContext.Current.Server.MapPath("~/" + filename);
                    PostedImage.SaveAs(postedImagePath);

                   
                        context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name)
                            .imagePath = postedImagePath;
                        context.SaveChanges();
                                    

                }
                return Request.CreateResponse(HttpStatusCode.OK, "image Updated Successfully.");
            }
            catch (Exception ex) 
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

            }

        }

        [HttpGet]
        public HttpResponseMessage GetCar(int id)
        {
            try
            {
                var car = context.Cars.Find(id);
            string url = "";
           
                var CurrentUserName = RequestContext.Principal.Identity.Name;
                var CurrentUser = context.UserMasters.FirstOrDefault(c => c.UserName == CurrentUserName);

                

                string photosLocationPath = HttpContext.Current.Server.MapPath("~/");
                if (Directory.Exists(photosLocationPath))
                {
                    string filename = Path.GetFileName(car.CarImagePath);
                    string[] files = Directory.GetFiles(photosLocationPath, filename);

                    foreach (var item in files)
                    {
                        if (item.Contains(filename))
                        {
                            string filenameRelative = "~/" + Path.GetFileName(item);
                            filenameRelative = filenameRelative.Replace(HttpContext.Current.Server.MapPath("~/"), "~/").Replace(@"\", "/");
                            url = filenameRelative;
                            
                        }

                    }

                }
                var carDTo = new CarDto
                {
                    carColor = car.carColor,
                    CarModel=car.CarModel,
                    CarUrl=url,
                    UserId=CurrentUser.UserID

                   
                };

                return Request.CreateResponse(HttpStatusCode.OK, carDTo);
            }
           
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            

        }
        [HttpDelete]
        public HttpResponseMessage DeleteCar(int id)
        {
            try
            {
                var CurrentUser = context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name);
                var CarToDelete = context.Cars.FirstOrDefault(c => c.ID == id);
                if (CarToDelete != null && CurrentUser.UserID == CarToDelete.UserId)
                {
                    context.Cars.Remove(CarToDelete);
                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "car deleted successfully");


                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"the car with id {id} wasn't found or you may not authorize to delete this car");
            }
            catch (Exception ex) 
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPut]
        public HttpResponseMessage UpdateCar(int id, CarDto car) 
        {
            try
            {
                var CurrentUser = context.UserMasters.FirstOrDefault(u=>u.UserName==RequestContext.Principal.Identity.Name);
                var CarToUpdate = context.Cars.Find(id);
                if (CarToUpdate != null && CarToUpdate.UserId == CurrentUser.UserID) 
                {
                    CarToUpdate.carColor = car.carColor;
                    CarToUpdate.CarModel = car.CarModel;
                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Car was Updated Successfully");
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Car was not found or yo are not authorize to update it");


            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }






    }
}
