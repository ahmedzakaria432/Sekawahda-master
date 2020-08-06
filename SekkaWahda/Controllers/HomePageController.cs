using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SekkaWahda.Models;

namespace SekkaWahda.Controllers
{[Authorize]
    public class HomePageController : ApiController
    { SECURITY_DBEntities context = new SECURITY_DBEntities();



        [HttpGet]
        [ActionName("UserInfoToAccessProfile")]
        public HttpResponseMessage UserInfoToAccessProfile()
        {
            try
            {
                var CurrentUser = context.UserMasters.FirstOrDefault(u => u.UserName == RequestContext.Principal.Identity.Name);
                string name;
                if (CurrentUser.FullName == null || CurrentUser.FullName == string.Empty)
                    name = CurrentUser.UserName;
                name = CurrentUser.FullName;
                var userInfoToReturn = new { name = name, UserID = CurrentUser.UserID, CurrentUser.ImageUrl };
                return Request.CreateResponse(HttpStatusCode.OK, userInfoToReturn);
            }
            catch (Exception ex) 
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

            }
        }

        [HttpGet]
        [ActionName("GetAllTrips")]
        public HttpResponseMessage GetAllTrips()
        {
            try
            {
                var tripsAllTrips = context.trips.ToList();
                var trips = context.trips.ToList().Select(tr => new
                {
                    UserName = context.UserMasters.Where(u => u.UserID == tr.DriverId).ToList().FirstOrDefault().UserName,
                    DateOfTrip = tr.DateOfTrip,
                    DriverId = tr.DriverId,
                    FromCity = tr.FromCity,
                    ID = tr.ID,
                    PlaceToMeet = tr.PlaceToMeet,
                    TimeOfTrip = tr.TimeOfTrip,
                    ToCity = tr.ToCity


                }).ToList();
                if (tripsAllTrips == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "There is no trips");
                }

                return Request.CreateResponse(HttpStatusCode.OK, trips);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);


            }
        }
         [HttpGet]
       
        [ActionName("GetTrips")]
        public HttpResponseMessage GetTrips()
        {
            try
            {
                var tripsInUserCity = context.trips.Where(x => x.FromCity == context.UserMasters.Where
                (u => u.UserName == RequestContext.Principal.Identity.Name).FirstOrDefault().city).ToList().Select(tr=>new {
                    DateOfTrip=   tr.DateOfTrip,
                    DriverId= tr.DriverId,
                    FromCity= tr.FromCity,
                    ID=tr.ID,
                    PlaceToMeet=tr.PlaceToMeet,
                    TimeOfTrip = tr.TimeOfTrip,
                    ToCity = tr.ToCity


                }).ToList();
                if (tripsInUserCity == null) {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "There is no trips in your city");
                }

                return Request.CreateResponse(HttpStatusCode.OK, tripsInUserCity);
            }
            catch (Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,ex.Message);

                
            }


        }
        [HttpGet]
        [ActionName("GetTrip")]
        public HttpResponseMessage GetTrip(int id)
        {

            try
            {
                var trip = context.trips.Where(t => t.ID == id).ToList().Select(tr => new {
                    DateOfTrip = tr.DateOfTrip,
                    DriverId = tr.DriverId,
                    FromCity = tr.FromCity,
                    ID = tr.ID,
                    PlaceToMeet = tr.PlaceToMeet,
                    TimeOfTrip = tr.TimeOfTrip,
                    ToCity = tr.ToCity


                }).FirstOrDefault();
                if (trip == null) {

                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Requested trip not found");
                }
                return Request.CreateResponse(HttpStatusCode.OK, trip);
            }
            catch (Exception ex) {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

        }
        [HttpPost]
        [ActionName("PostTrip")]
        public HttpResponseMessage PostTrip([FromBody]trip NewTrip)
        {
            var CurrentUser = context.UserMasters.Where(u => u.UserName == RequestContext.Principal.Identity.Name).FirstOrDefault();
            if (CurrentUser.DriverLicense != null && (CurrentUser.Cars!=null||CurrentUser.Cars.Count>0))
            {

                if (ModelState.IsValid)
                {

                    try
                    {
                        NewTrip.DriverId = CurrentUser.UserID;
                        context.trips.Add(NewTrip);
                        context.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.Created, "Trip was successfully Added");


                    }
                    catch (Exception ex)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

                    }
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, NewTrip);
            }
           try{ var message= Request.CreateResponse(HttpStatusCode.BadRequest, "You should enter your car License and your driving License");
            

            message.Headers.Location = new Uri("https://seka.azurewebsites.net/" + "/api/HomePage/AddLicense/");
            return message;
}

catch (Exception ex)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

                    }
         
            }

        [ActionName("CheckForLicense")]
        public HttpResponseMessage CheckForLicense() 
        {
            if (ModelState.IsValid) 
            {
                try
                {
                   
                    var CurrentUser=context.UserMasters.FirstOrDefault(u=>u.UserName== RequestContext.Principal.Identity.Name);
                    HttpResponseMessage response;
                    var resp=(CurrentUser.Cars == null||CurrentUser.Cars.Count==0 || CurrentUser.DriverLicense == null) ?
                        response=Request.CreateResponse(HttpStatusCode.NotFound,"you should enter details for your car and driving license "):
                        response = Request.CreateResponse(HttpStatusCode.OK, "you have currently entered your car details and driving license ");
                    return response;
                }
                catch (Exception ex)
                {

                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                } 

                    

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
       
        
        [ActionName("AddLicense")]
        public HttpResponseMessage AddLicense([FromBody]LicenseModel licences) {
            var Current = RequestContext.Principal.Identity.Name;

            var user = context.UserMasters.FirstOrDefault(u => u.UserName == Current);
           
            user.DriverLicense = licences.DriverLicense;
            try
            {
                context.SaveChanges();

            }
            catch (Exception ex) {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

            }
            
            var message = Request.CreateResponse(HttpStatusCode.Redirect);
            message.Headers.Location = new Uri(Request.RequestUri.Authority + "/"+"api/HomePage/GetTrips/");
            return message;

        }


        [HttpPut]
        [ActionName("PutTrip")]
        public HttpResponseMessage PutTrip(int id, [FromBody]trip UpdatedTrip)
        {
            if (ModelState.IsValid)
            {

                var OldTrip = context.trips.FirstOrDefault(t => t.ID == id);
                if (OldTrip == null) {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,"trip was not found");

                }
                OldTrip.DateOfTrip = UpdatedTrip.DateOfTrip;
                OldTrip.DriverId = UpdatedTrip.DriverId;
                OldTrip.FromCity = UpdatedTrip.FromCity;
                OldTrip.PlaceToMeet = UpdatedTrip.PlaceToMeet;
                OldTrip.TimeOfTrip = UpdatedTrip.TimeOfTrip;
                OldTrip.ToCity = UpdatedTrip.ToCity;
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex) {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

                }
               
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Updated successfully");
            }
        [HttpDelete]
        [ActionName("DeleteTrip")]
        
        public HttpResponseMessage DeleteTrip(int id)
        {
            try
            {
                var TriptoDelet = context.trips.FirstOrDefault(t => t.ID == id);
                if (TriptoDelet.DriverId != context.UserMasters.Where(u => u.UserName == RequestContext.Principal.Identity.Name).FirstOrDefault().UserID) {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "You are unauthorized to delete this trip");

                }
                context.trips.Remove(TriptoDelet);
                context.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Deleted sucessfully");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

            }
        }



    }
  
        }


    
