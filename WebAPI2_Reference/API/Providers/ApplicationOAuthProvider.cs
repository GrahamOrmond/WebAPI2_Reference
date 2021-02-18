using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using WebAPI2_Reference.Models;

namespace WebAPI2_Reference.API.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        // get private token settings
        private static int _tokenExpireTimeMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["Token:TokenExpireTimeMinutes"]);
        private readonly string _publicClientId;
        private readonly string _chromeAppId = ConfigurationManager.AppSettings["Extension:Id"];
        private readonly string _chromeRedirectUri = ConfigurationManager.AppSettings["Extension:Uri"];

        // stores short lived code grants
        private readonly ConcurrentDictionary<string, string> _authenticationCodes =
        new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }
            _publicClientId = publicClientId;
        }

        // /api/authorize
        // used to generate and authentication code on login through this endpoint
        public override async Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            
            if (context.Request.User != null 
                && context.Request.User.Identity.IsAuthenticated) // user loggedin and is authenicated
            {
                // get request parameters
                var redirectUri = context.Request.Query["redirect_uri"];
                var clientId = context.Request.Query["client_id"];

                // get user instance by name
                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
                ApplicationUser user = await userManager
                    .FindByNameAsync(context.Request.User.Identity.Name);

                // create code grant
                const int strengthInBits = 1024;
                var timeNow = DateTime.UtcNow;
                CodeGrantTicket codeGrant = new CodeGrantTicket()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Code = RandomOAuthGenerator.Generate(strengthInBits),
                    ExpiresUtc = timeNow.AddMinutes(5),
                    IssuedUtc = timeNow,
                };
                
                // save the code grant to the dictionary
                _authenticationCodes.TryAdd(codeGrant.Code, JsonConvert.SerializeObject(codeGrant));

                //// clear cookies (not sure if this should be done or not so i left out just incase it messes something up)
                //var cookies = context.Request.Cookies.ToList();
                //foreach (var c in cookies)
                //{
                //    context.Response.Cookies.Delete(c.Key, new CookieOptions());
                //}

                // redirect to redirectUri with code in url
                context.Response.Redirect(redirectUri + "?code=" + Uri.EscapeDataString(codeGrant.Code));
            }
            else // user is not authenicated
            {
                // redirect to login page to authenticate the user
                context.Response.Redirect("/account/login?returnUrl=" + Uri.EscapeDataString(context.Request.Uri.ToString()));
            }
            context.RequestCompleted();
        }

        // custom grant to give access token through
        // called through grant_type = "*any grant not existing already*"
        public override async Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            // get the token from the database
            var oauthCode = context.Parameters["code"];
            var grant_type = context.Parameters["grant_type"];
            if (String.IsNullOrEmpty(oauthCode) || String.IsNullOrEmpty(grant_type))
            {
                context.SetError("invalid_grant", "Invalid Access");
                return;
            }

            // check set grant type defined by me that it must equal
            if (grant_type != "code") // does not equal the defined grant type
            {
                context.SetError("invalid_grant", "Invalid Access");
                return;
            }

            // get the short lived code ticket
            string serializedTicket = null;
            _authenticationCodes.TryRemove(oauthCode, out serializedTicket);
            if (serializedTicket == null)
            {
                context.SetError("invalid_grant", "Invalid Access");
                return;
            }

            // deserialize and check the expires time 
            var codeGrantTicket = JsonConvert.DeserializeObject<CodeGrantTicket>(serializedTicket);
            if (DateTime.UtcNow >= codeGrantTicket.ExpiresUtc) // token expired
            {
                context.SetError("invalid_grant", "Invalid Access");
                return;
            }

            // get the user
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            ApplicationUser user = await userManager.FindByIdAsync(codeGrantTicket.UserId);
            if (user == null)
            {
                context.SetError("invalid_grant", "Invalid Access");
                return;
            }

            // add claims
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            // add ticket and validate
            AuthenticationProperties properties = CreateProperties(user.UserName, user.Id);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        // grant_type = password
        // get access tokens from username and password
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // get the user
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            // create claims
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            // create ticket and validate
            AuthenticationProperties properties = CreateProperties(user.UserName, user.Id);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        // returns the token parameters on token endpoint call 
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            /* add all properties from the dictionary to the results */
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }

        // validate the incoming request for the authorize endpoint
        public override async Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            // client id and grant type must match to continue
            if (context.AuthorizeRequest.ClientId == "self" 
                && context.AuthorizeRequest.IsAuthorizationCodeGrantType)
                context.Validated();
            else // invalid clientId or grant type
                context.Rejected();
        }

        // validate the client authentication
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId, clientSecret;
            context.TryGetBasicCredentials(out clientId, out clientSecret);
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null || context.ClientId == "self")
            {
                context.Validated(clientId);
            }
            return Task.FromResult<object>(null);
        }

        // validate the given redirect uri 
        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");
                if (context.RedirectUri.Contains(expectedRootUri.AbsoluteUri)  // local redirect
                     || _chromeRedirectUri == context.RedirectUri) // added chrome extension redirect
                {
                    context.Validated();
                }
            }
            return Task.FromResult<object>(null);
        }

        // validate the incoming request for access tokens
        public override async Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            // validate the code grant requested
            if (context.TokenRequest.IsCustomExtensionGrantType // custom grant
                || context.TokenRequest.IsRefreshTokenGrantType // refresh grant
                || context.TokenRequest.IsResourceOwnerPasswordCredentialsGrantType) // credentials grant
                context.Validated();
            else // user is requesting invalid grant type
                context.Rejected();
        }

        // create token properties to include with access and refresh tokens
        public static AuthenticationProperties CreateProperties(string userName, string userId)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName },
                { "userId", userId },
                { "expires_in", _tokenExpireTimeMinutes.ToString() }
            };
            return new AuthenticationProperties(data);
        }

        /*
         *  Generate a random string
         *  Used for generating refresh token
         */
        private static class RandomOAuthGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        // holds data about a short lived 5 min code grant
        private class CodeGrantTicket
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string Code { get; set; }
            public DateTime IssuedUtc { get; set; }
            public DateTime ExpiresUtc { get; set; }
        }
    }
}