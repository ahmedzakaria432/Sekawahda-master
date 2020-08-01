using SekkaWahda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SekkaWahda.ExtensionMethods;

namespace SekkaWahda.Controllers
{

    public class CarController : ApiController
    {
        SECURITY_DBEntities context = new SECURITY_DBEntities();

        public HttpResponseMessage AddCar([FromBody] CarDto car)
        {
            try
            {
                var CarToAdd = new Car();
                CarToAdd.carColor = car.carColor;
                CarToAdd.CarModel = car.CarModel;

                CarToAdd.UserId = context.UserMasters.FirstOrDefault(u => u.UserName == (RequestContext.Principal.Identity.Name)).UserID;

                var files = HttpContext.Current.Request.Files;
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
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        public HttpResponseMessage UpdateCarImage()
        {
            try {
                var files = HttpContext.Current.Request.Files;
                foreach (string file in files)
                {
                    var PostedImage = files[file];
                    var filename = PostedImage.FileName.NoRepeateInFileName(RequestContext.Principal.Identity.Name);
                    var postedImagePath = HttpContext.Current.Server.MapPath("~/" + filename);
                    PostedImage.SaveAs(postedImagePath);

                    using (SECURITY_DBEntities context = new SECURITY_DBEntities())
                    {
                        context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name)
                            .imagePath = postedImagePath;
                        context.SaveChanges();
                    }
                 

                }
                return Request.CreateResponse(HttpStatusCode.OK, "image Updated Successfully.");
            }
            catch (Exception ex) 
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

            }

        }


    }
}
