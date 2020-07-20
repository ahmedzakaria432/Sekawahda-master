using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SekkaWahda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SekkaWahda.Models
{
    public class UserMasterReprository : IDisposable
    {
        SECURITY_DBEntities context = new SECURITY_DBEntities();
        public UserMaster ValidateUser(string UserName, string Password)
        {
            return context.UserMasters.FirstOrDefault(user => user.UserName.Equals(UserName, StringComparison.OrdinalIgnoreCase)
            && user.UserPassword == Password);



            


        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}