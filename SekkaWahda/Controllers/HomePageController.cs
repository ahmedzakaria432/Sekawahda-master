using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SekkaWahda.Models;

namespace SekkaWahda.Controllers
{[Authorize]
    public class HomePageController : ApiController
    { SECURITY_DBEntities context = new SECURITY_DBEntities();


        public HttpResponseMessage SearchForTrip(SearchForTripDto searchDto) 
        {
            if (ModelState.IsValid) 
            {
                try
                {
                    var timezone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                    var date = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                    var ResultTripsOfSearch = context.trips.Where
                        (t => DbFunctions.TruncateTime(t.DateOfTrip) == DbFunctions.TruncateTime(searchDto.DateOfTrip) && t.FromCity == searchDto.FromCity && t.ToCity == searchDto.ToCity)
                        .Join(context.UserMasters, t => t.DriverId, u => u.UserID, (tr, us) => new 
                        {


                            DateOfTrip = DbFunctions.TruncateTime(tr.DateOfTrip).Value,
                            DriverId = tr.DriverId,
                            FromCity = tr.FromCity,
                            ID = tr.ID,
                            PlaceToMeet = tr.PlaceToMeet,
                            TimeOfTrip = tr.TimeOfTrip,
                            ToCity = tr.ToCity,
                            Name = (us.FullName == null) ? us.UserName : us.FullName,
                            ImageUrl = us.ImageUrl,
                            PostTime = (DbFunctions.TruncateTime(tr.TimeOfPost) == DbFunctions.TruncateTime(date)) ?
                        (tr.TimeOfPost.Value.Hour == date.Hour ?
                        new { time = DbFunctions.DiffMinutes(tr.TimeOfPost.Value, date).Value, unit = "Minutes" } :
                        new { time = DbFunctions.DiffHours(tr.TimeOfPost.Value, date).Value, unit = "hours" }) :
                        new { time = DbFunctions.DiffDays(tr.TimeOfPost.Value, date).Value, unit = "days" }

                        }).ToList();
                    var tripsEditDate = ResultTripsOfSearch.Select(t=>new
                    {
                        DateOfTrip = t.DateOfTrip.ToString("MM/dd/yyyy"),
                        DriverId = t.DriverId,
                        FromCity = t.FromCity,
                        ID = t.ID,
                        PlaceToMeet = t.PlaceToMeet,
                        TimeOfTrip = t.TimeOfTrip.Hours + ":" + t.TimeOfTrip.Minutes,
                        ToCity = t.ToCity,
                        Name = t.Name,
                        ImageUrl = t.ImageUrl,
                        PostTime = t.PostTime

                    });

                    if (ResultTripsOfSearch == null)
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "There is no trips");
                    var tripssToReturn = ResultTripsOfSearch.
                        Join(context.Reservations,t=>t.ID,r=>r.TripId,(t,r)=>
                            new
                            {
                                DateOfTrip = t.DateOfTrip,
                                DriverId = t.DriverId,
                                FromCity = t.FromCity,
                                ID = t.ID,
                                PlaceToMeet = t.PlaceToMeet,
                                TimeOfTrip = t.TimeOfTrip,
                                ToCity = t.ToCity,
                                Name = t.Name,
                                ImageUrl = t.ImageUrl,
                                PostTime = t.PostTime
                            }).ToList();
                    var triptoreturn = tripssToReturn.Select(t => new
                    {
                        DateOfTrip = t.DateOfTrip.ToString("MM/dd/yyyy"),
                        DriverId = t.DriverId,
                        FromCity = t.FromCity,
                        ID = t.ID,
                        PlaceToMeet = t.PlaceToMeet,
                        TimeOfTrip = t.TimeOfTrip.Hours + ":" + t.TimeOfTrip.Minutes,
                        ToCity = t.ToCity,
                        Name = t.Name,
                        ImageUrl = t.ImageUrl,
                        PostTime = t.PostTime

                    });
                    List<object> listToRet = new List<object>();
                    bool found=false;
                    object ObjToAdd;
                    for (int i = 0; i < tripsEditDate.Count(); i++)
                    {
                        for (int j= 0; j < tripssToReturn.Count(); j++)
                        {
                            if (tripsEditDate.ElementAt(i).ID == tripssToReturn.ElementAt(j).ID)
                            {
                                found = true;
                              
                            }
                        }
                        if (!found)
                            listToRet.Add(tripsEditDate.ElementAt(i));
                        found = false;
                    }
                    
                 
                        return Request.CreateResponse(HttpStatusCode.OK, listToRet);


                }
                catch (Exception ex)
                {

                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                }
                
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "you have to fill required fields");
        }

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
                else
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
        [ActionName("GetMyReservedTrips")]
        public HttpResponseMessage GetMyReservedTrips()
        {
            try
            {

                var timezone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                var date = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);


                var currentUserID = context.UserMasters.FirstOrDefault(u => u.UserName == (RequestContext.Principal.Identity.Name)).UserID;


                var ReservedTripsOfCurrentUser = context.trips
                      .Join(context.Reservations, t => t.ID, r => r.TripId, (tr, re)=>new {
                          DateOfTrip = DbFunctions.TruncateTime(tr.DateOfTrip).Value,
                          DriverId = tr.DriverId,
                          FromCity = tr.FromCity,
                          TripID = tr.ID,
                          PlaceToMeet = tr.PlaceToMeet,
                          TimeOfTrip = tr.TimeOfTrip,
                          ToCity = tr.ToCity,
                          PostTime = (DbFunctions.TruncateTime(tr.TimeOfPost) == DbFunctions.TruncateTime(date)) ?
                        (tr.TimeOfPost.Value.Hour == date.Hour ?
                        new { time = DbFunctions.DiffMinutes(tr.TimeOfPost.Value, date).Value, unit = "Minutes" } :
                        new { time = DbFunctions.DiffHours(tr.TimeOfPost.Value, date).Value, unit = "hours" }) :
                        new { time = DbFunctions.DiffDays(tr.TimeOfPost.Value, date).Value, unit = "days" },
                        re.TravellerId,
                        re.Accebted
                        

                      }).Where(t=>DbFunctions.TruncateTime(date) < DbFunctions.TruncateTime(t.DateOfTrip)&&currentUserID==t.TravellerId)
                      .Join(context.UserMasters,t=>t.DriverId,u=>u.UserID,(tr,us)=>new
                      {

                          DateOfTrip = tr.DateOfTrip,
                          DriverId = tr.DriverId,
                          FromCity = tr.FromCity,
                          TripID = tr.TripID,
                          PlaceToMeet = tr.PlaceToMeet,
                          TimeOfTrip = tr.TimeOfTrip,
                          ToCity = tr.ToCity,
                          PostTime = tr.PostTime,
                          tr.TravellerId,
                          Accebted= tr.Accebted == true ?
                        "Accepted reservation" : "reservation not yet Accepted ",
                        Name = (us.FullName == null) ? us.UserName : us.FullName,
                          ImageUrl = us.ImageUrl
                      }).OrderByDescending(p => p.TripID).ToList(); 
                      
                    //var ReservedTrips = context.trips.Join(context.UserMasters, t => t.DriverId, u => u.UserID,
                    //(tr, us) => new
                    //{
                    //    DateOfTrip = DbFunctions.TruncateTime(tr.DateOfTrip).Value,
                    //    DriverId = tr.DriverId,
                    //    FromCity = tr.FromCity,
                    //    ID = tr.ID,
                    //    PlaceToMeet = tr.PlaceToMeet,
                    //    TimeOfTrip = tr.TimeOfTrip,
                    //    ToCity = tr.ToCity,
                    //    Accebted=context.Reservations.FirstOrDefault(r=>r.TripId==tr.ID).Accebted.Value==true?
                    //    "Accepted reservation": "reservation not yet Accepted "
                    //    ,
                    //    Name = (us.FullName == null) ? us.UserName : us.FullName,
                    //    ImageUrl = us.ImageUrl,
                    //    PostTime = (DbFunctions.TruncateTime(tr.TimeOfPost) == DbFunctions.TruncateTime(date)) ?
                    //    (tr.TimeOfPost.Value.Hour == date.Hour ?
                    //    new { time = DbFunctions.DiffMinutes(tr.TimeOfPost.Value, date).Value, unit = "Minutes" } :
                    //    new { time = DbFunctions.DiffHours(tr.TimeOfPost.Value, date).Value, unit = "hours" }) :
                    //    new { time = DbFunctions.DiffDays(tr.TimeOfPost.Value, date).Value, unit = "days" }

                    //}) .OrderByDescending(p => p.ID).ToList();
                if (ReservedTripsOfCurrentUser == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "there is no Reserved trips");
                }
               var tripssToReturn = ReservedTripsOfCurrentUser.Select(t => new {
                        DateOfTrip = t.DateOfTrip.ToString("MM/dd/yyyy"),
                        DriverId = t.DriverId,
                        FromCity = t.FromCity,
                        ID = t.TripID,
                        PlaceToMeet = t.PlaceToMeet,
                        TimeOfTrip = t.TimeOfTrip.Hours + ":" + t.TimeOfTrip.Minutes,
                        t.Accebted,
                        ToCity = t.ToCity,
                        Name = t.Name,
                        ImageUrl = t.ImageUrl,
                        PostTime = t.PostTime
                    }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, tripssToReturn);

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
                
                var timezone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                
                var date = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
                

                var tripss = context.trips.Where(t=>t.ID!=context.Reservations.FirstOrDefault(r=>r.TripId==t.ID).TripId).Join(context.UserMasters, t => t.DriverId, u => u.UserID,
                    (tr, us) => new
                    {
                        DateOfTrip = DbFunctions.TruncateTime(tr.DateOfTrip).Value,
                        DriverId = tr.DriverId,
                        FromCity = tr.FromCity,
                        ID = tr.ID,
                        PlaceToMeet = tr.PlaceToMeet,
                        TimeOfTrip = tr.TimeOfTrip,
                        ToCity = tr.ToCity,
                        Name = (us.FullName == null) ? us.UserName : us.FullName,
                        ImageUrl = us.ImageUrl,
                        PostTime = (DbFunctions.TruncateTime(tr.TimeOfPost) == DbFunctions.TruncateTime(date)) ?
                        (tr.TimeOfPost.Value.Hour == date.Hour ?
                        new { time = DbFunctions.DiffMinutes(tr.TimeOfPost.Value, date).Value, unit = "Minutes" } :
                        new { time = DbFunctions.DiffHours(tr.TimeOfPost.Value,date ).Value, unit = "hours" }) :
                        new { time = DbFunctions.DiffDays(tr.TimeOfPost.Value, date).Value, unit = "days" }
                      
                    }).OrderByDescending(p=>p.ID).ToList();
                //var tripsAllTrips = context.trips.ToList();
                //var trips = context.trips.ToList().Select(tr => new
                //{
                //    UserName = context.UserMasters.Where(u => u.UserID == tr.DriverId).ToList().FirstOrDefault().UserName,
                //    DateOfTrip = tr.DateOfTrip,
                //    DriverId = tr.DriverId,
                //    FromCity = tr.FromCity,
                //    ID = tr.ID,
                //    PlaceToMeet = tr.PlaceToMeet,
                //    TimeOfTrip = tr.TimeOfTrip,
                //    ToCity = tr.ToCity


                //}).ToList();
                if (tripss == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "There is no trips");
                }
                var tripssToReturn= tripss.Select(t => new {
                    DateOfTrip = t.DateOfTrip.ToString("MM/dd/yyyy"),
                    DriverId = t.DriverId,
                    FromCity = t.FromCity,
                    ID = t.ID,
                    PlaceToMeet = t.PlaceToMeet,
                    TimeOfTrip = t.TimeOfTrip.Hours + ":" + t.TimeOfTrip.Minutes,
                    ToCity = t.ToCity,
                    Name = t.Name,
                    ImageUrl = t.ImageUrl,
                    PostTime = t.PostTime
                }).ToList();
                

                return Request.CreateResponse(HttpStatusCode.OK, tripssToReturn);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);


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
                var timezone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                var date = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                var trip = context.trips.Where(t => t.ID == id)
                    .Join(context.UserMasters,t=>t.DriverId,u=>u.UserID,(tr,us)=>new
                {
                    DateOfTrip = tr.DateOfTrip,
                    DriverId = tr.DriverId,
                    FromCity = tr.FromCity,
                    ID = tr.ID,
                    PlaceToMeet = tr.PlaceToMeet,
                    TimeOfTrip = tr.TimeOfTrip,
                    ToCity = tr.ToCity,
                        Name = (us.FullName == null) ? us.UserName : us.FullName,
                        ImageUrl = us.ImageUrl,
                        PostTime = (DbFunctions.TruncateTime(tr.TimeOfPost) == DbFunctions.TruncateTime(date)) ?
                        (tr.TimeOfPost.Value.Hour == date.Hour ?
                        new { time = DbFunctions.DiffMinutes(tr.TimeOfPost.Value, date).Value, unit = "Minutes" } :
                        new { time = DbFunctions.DiffHours(tr.TimeOfPost.Value, date).Value, unit = "hours" }) :
                        new { time = DbFunctions.DiffDays(tr.TimeOfPost.Value, date).Value, unit = "days" }

                    }).FirstOrDefault();
                if (trip == null) {

                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Requested trip not found");
                }
                var tripToReturn =  new {
                    DateOfTrip = trip.DateOfTrip.ToString("MM/dd/yyyy"),
                    DriverId = trip.DriverId,
                    FromCity = trip.FromCity,
                    ID = trip.ID,
                    PlaceToMeet = trip.PlaceToMeet,
                    TimeOfTrip = trip.TimeOfTrip.Hours + ":" + trip.TimeOfTrip.Minutes,
                    ToCity = trip.ToCity,
                    Name = trip.Name,
                    ImageUrl = trip.ImageUrl,
                    PostTime = trip.PostTime
                };
                return Request.CreateResponse(HttpStatusCode.OK, tripToReturn);
            }
            catch (Exception ex) {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

        }
        [HttpPost]
        [ActionName("PostTrip")]
        public HttpResponseMessage PostTrip([FromBody]TripDTO NewTrip)
        {
            var CurrentUser = context.UserMasters.Where(u => u.UserName == RequestContext.Principal.Identity.Name).FirstOrDefault();
            if (CurrentUser.DriverLicense != null && (CurrentUser.Cars!=null)&&CurrentUser.Cars.Count>0)
            {

                if (ModelState.IsValid)
                {

                    try
                    {
                        var TripToAdd = new trip()
                        {
                            DateOfTrip = NewTrip.DateOfTrip,
                            DriverId = CurrentUser.UserID,
                            FromCity = NewTrip.FromCity,
                            PlaceToMeet = NewTrip.PlaceToMeet,
                            TimeOfPost = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")),
                            TimeOfTrip = NewTrip.TimeOfTrip,
                            ToCity = NewTrip.ToCity,

                        };
                        
                        
                        context.trips.Add(TripToAdd);
                        

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
           try{ var message= Request.CreateResponse(HttpStatusCode.BadRequest, "You should enter your car details and your driving License");
            

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
                
                OldTrip.FromCity = UpdatedTrip.FromCity;
                OldTrip.PlaceToMeet = UpdatedTrip.PlaceToMeet;
                OldTrip.TimeOfTrip = UpdatedTrip.TimeOfTrip;
                OldTrip.ToCity = UpdatedTrip.ToCity;
                try
                {
                    context.notification_.Add(new notification_
                    {
                        RaiserID = OldTrip.DriverId,
                        ReceiverID = context.Reservations.FirstOrDefault(r=>r.TripId==OldTrip.ID).TravellerId,
                        Message_ = "Your reservation was Updated because the car owner Updated the trip details," +
                        " you can go to the trip post and see the new details ",
                        TypeOfNotification = "TripUpdated"

                    });
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
                
                var DeletedReservation= context.Reservations.Remove(context.Reservations.FirstOrDefault(r=>r.TripId== TriptoDelet.ID));
                
                context.trips.Remove(TriptoDelet);
                context.notification_.Add(new notification_
                {
                    RaiserID = TriptoDelet.DriverId,
                    ReceiverID = DeletedReservation.TravellerId,
                    Message_ = "Unfortunately Your reservation was canceled because the car owner deleted the trip , you can go to home page and find another trip ",
                    TypeOfNotification = "TripDeleted"

                }) ;

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


    
