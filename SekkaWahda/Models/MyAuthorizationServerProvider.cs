using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SekkaWahda.Models
{
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();

        }



        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (UserMasterReprository repo = new UserMasterReprository())
            {
                var user = repo.ValidateUser(context.UserName, context.Password);
                if (user == null)
                {

                    context.SetError("invalid_grant", "Provided username and password is incorrect");
                    return;

                }
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Role, user.UserRoles));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.UserEmailID));
                

                context.Validated(identity);

            }

            //  return base.GrantResourceOwnerCredentials(context);
        }
    }

}