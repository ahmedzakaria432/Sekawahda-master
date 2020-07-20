﻿using SekkaWahda.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI;

namespace SekkaWahda.Controllers
{
    public class AccountController : ApiController
    {
        SECURITY_DBEntities context = new SECURITY_DBEntities();
        [Authorize]
        [HttpGet]
        public HttpResponseMessage GetCurrentUser() {
            try
            {
                var CurrentUserName = RequestContext.Principal.Identity.Name;
                var CurrentUser = context.UserMasters.Where(U => U.UserName == CurrentUserName).FirstOrDefault();
                if (CurrentUser == null) {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user was not found");
                }
                var UserToReturn = new
                {
                    UserName = CurrentUser.UserName,
                    UserCity = CurrentUser.city,
                    UserSSN = CurrentUser.SSN,
                    UserEmailID = CurrentUser.UserEmailID,
                    FullName = CurrentUser.FullName,
                    CarLicense = CurrentUser.CarLicense,
                    DriverLicense = CurrentUser.DriverLicense
                   

                };
                return Request.CreateResponse(HttpStatusCode.OK, UserToReturn);


            }
            catch (Exception e) {


                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }



        }

        [HttpPost]
        [Authorize]
        [ActionName("AddPhoto")]
        public async Task<HttpResponseMessage> AddPhoto()
        {
            #region oldMethod
            /*

            var CurrentUserName = RequestContext.Principal.Identity.Name;
            var CurrentUser = context.UserMasters.FirstOrDefault(c => c.UserName == CurrentUserName);
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/UsersPhotos");
            var Provider = new MultipartFormDataStreamProvider(root);

       
            try
            {
                await Request.Content.ReadAsMultipartAsync(Provider);
                string FilePath=string.Empty;
                foreach (var file in Provider.FileData )
                {
                    var name = file.Headers.ContentDisposition.FileName;

                    name=name.Trim('"');

                    var Extension = name.Substring(name.LastIndexOf("."));
                    var NameWithoutExtension = name.Substring(0, name.LastIndexOf("."));


                    var LocalFileName = file.LocalFileName;
                    NameWithoutExtension = NameWithoutExtension +CurrentUser.UserName+DateTime.Now;
                    List<string> charToRemove = new List<string>()
                    {"/",":"

                    };
                    foreach (var c in charToRemove )
                    {
                        NameWithoutExtension = NameWithoutExtension.Replace(c, string.Empty);

                    }

                    name = NameWithoutExtension + Extension;
                   name= String.Concat(name.Where(c=>!char.IsWhiteSpace(c)));
                     FilePath = Path.Combine(root, name);
                    File.Move(LocalFileName, FilePath);
                    

                
                }
                CurrentUser.imagePath = FilePath; 
                var user= context.UserMasters.Where(u=>u.UserID==CurrentUser.UserID).FirstOrDefault();
                user.imagePath = CurrentUser.imagePath;
                context.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.Created, "Photo added successfully");

            
            }
            catch (Exception ex)
            {


                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,ex.InnerException);
            }
             */
            #endregion
            var CurrentUserName = RequestContext.Principal.Identity.Name;
            var CurrentUser = context.UserMasters.FirstOrDefault(c => c.UserName == CurrentUserName);

            var httpRequest = HttpContext.Current.Request;
            var files = httpRequest.Files;
            if (files.Count < 1)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "you must upload image");
            foreach (string file in files)
            {
                var postedFile = files[file];
                var filePath = HttpContext.Current.Server.MapPath("~/UsersPhotos/"+ postedFile.FileName);
                postedFile.SaveAs(filePath);
                CurrentUser.imagePath = filePath;
                context.SaveChanges();

            }
            return Request.CreateResponse(HttpStatusCode.Created,"photo Added successfully");

            //var folderName = Path.Combine("UsersPhotos");
        
            //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            //if (files.Any(f => f.Length == 0))
            //    throw new Exception();



        }



        [HttpGet]
        [ActionName("GetUserPhoto")]
        public HttpResponseMessage GetUserPhoto() 
        
        {
            string url ="";
            try
            {
                var CurrentUserName = RequestContext.Principal.Identity.Name;
                var CurrentUser = context.UserMasters.FirstOrDefault(c => c.UserName == CurrentUserName);

                #region test
                /* 
                var response = Request.CreateResponse(HttpStatusCode.OK);

                try
                {
                    var CurrentUserName = RequestContext.Principal.Identity.Name;
                var CurrentUser = context.UserMasters.FirstOrDefault(c => c.UserName == CurrentUserName);



                    var Extension = Path.GetExtension( CurrentUser.imagePath);
                    Extension = Extension.Substring(1);
                    var contents = File.ReadAllBytes(CurrentUser.imagePath);
                    MemoryStream ms = new MemoryStream(contents);
                    response.Content = new StreamContent(ms);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/" + Extension);



                }





                catch (Exception ex)
                {

                    Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);   
                }
                return response;
                */
                #endregion
                
                string photosLocationPath = HttpContext.Current.Server.MapPath("~/UsersPhotos");
                if (Directory.Exists(photosLocationPath))
                {
                    string filename = Path.GetFileName(CurrentUser.imagePath);
                    string[] files = Directory.GetFiles(photosLocationPath, filename);

                    foreach (var item in files)
                    {
                        if (item.Contains(filename))
                        {
                            string filenameRelative = "~/UsersPhotos" + Path.GetFileName(item);
                            url = Url.Content(filenameRelative);
                            url = HttpContext.Current.Request.MapPath(filenameRelative);



                        }

                    }
                    
                }
                return Request.CreateResponse(HttpStatusCode.OK, url);
            }
            catch(Exception ex) {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, url);

            /*  try
              {

                  string LocalPath = Path.GetFileName(CurrentUser.imagePath);
                  var imageurl = Url.Content(LocalPath);  

                  return Request.CreateResponse(HttpStatusCode.OK, imageurl);


              }
              catch (Exception ex)
              {

                  return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
              }

      */

        }




        [HttpPost]
        [ActionName("Register")]
        public HttpResponseMessage Register([FromBody]UserMasterModel NewUser)
        {
            try
            {
                if (ModelState.IsValid)
                {

                             
                        if (context.UserMasters.FirstOrDefault(u => u.SSN == NewUser.SSN)!=null)
                        {
                            UserMasterErrorModel ErrorSSN = new UserMasterErrorModel();
                            ErrorSSN.User = NewUser;
                            ErrorSSN.ErrorMessage = "SSN must be unique";
                           

                            return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorSSN);

                        }
                        var found = context.UserMasters.FirstOrDefault(u=>u.UserName==NewUser.UserName);
                        if (found == null)
                        {
                            var user = new UserMaster();
                            user.UserName = NewUser.UserName;
                            user.UserEmailID = NewUser.UserEmailID;
                            user.UserRoles = "user";
                            user.UserPassword = NewUser.UserPassword;

                            user.PhoneNumber = NewUser.PhoneNumber;
                         
                            user.city = NewUser.city;
                            user.SSN = NewUser.SSN;
                            

                            context.UserMasters.Add(user);
                            context.SaveChanges();
                            return Request.CreateResponse(HttpStatusCode.OK, "registeration was sucessful");
                        }
                        UserMasterErrorModel error = new UserMasterErrorModel();
                        error.User = NewUser;
                        error.ErrorMessage = "User name must be unique";
                        var message = Request.CreateResponse(HttpStatusCode.BadRequest, error);
                       
                        message.Headers.Location = new Uri(Request.RequestUri + "/");
                     
                        return message;
                    }

                    else
                    {
                        var message = Request.CreateResponse(HttpStatusCode.BadRequest, NewUser);
                        message.Headers.Location = new Uri(Request.RequestUri + "/");

                        return message;

                    }


                
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.InnerException);

            }


        }



    }


}

