using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Newtonsoft.Json.Linq;

namespace OwinSimpleCookieAuthMVC
{
    public class FacecookAuthenticationMiddleware : AuthenticationMiddleware<FacecookAuthOpt>
    {
        public FacecookAuthenticationMiddleware(OwinMiddleware next, FacecookAuthOpt options) : base(next, options)
        {

        }

        protected override AuthenticationHandler<FacecookAuthOpt> CreateHandler()
        {
            return new FacecookAuthenticationHandler();
        }
    }

    public class FacecookAuthOpt : AuthenticationOptions
    {
        public FacecookAuthOpt(string authenticationType) : base(authenticationType)
        {
            // should not try to get the user claims from facecook on each request
            // only when there`s a callback request redirect back from facecook with the necessary authorization code that facecook issued
            // then the auth middleware should use the code to request the user info from facecook and sign in the received identity
            this.AuthenticationMode = AuthenticationMode.Passive;
        }
    }

    public class FacecookAuthenticationHandler : AuthenticationHandler<FacecookAuthOpt>
    {
        // redirect the user to facecook login if the app requested 
        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return Task.FromResult<object>(null);
            }

            AuthenticationResponseChallenge challenge = Helper.LookupChallenge("Facecook", Options.AuthenticationMode);
            if (challenge != null)
            {
                var originalRedirect = Request.Query.Get("returnUrl");
                // client_id omitted for brievety
                var redirectUrl = Uri.EscapeUriString($"/signin-facecook?returnUrl={originalRedirect}");
                Response.Redirect($"/facecook/authorize?returnUrl={redirectUrl}");
                return Task.FromResult<object>(null);
            }
            else
            {
                return Task.FromResult<object>(null);
            }
        }

        public override async Task<bool> InvokeAsync()
        {
            if (Request.Path.Value.Contains("signin-facecook"))
            {
                // this is a callback request that was redirected from facecook and should carry authorization token that should be used to obtain the external user claims
                var ticket = await AuthenticateAsync();

                // this call will trigger the external cookie auth middleware to persist the external facecook identity to cookie
                Context.Authentication.SignIn(ticket.Identity);

                // redirect the request to the originally requested resource
                var redirectUrl = Request.Query.First(x => x.Key == "returnUrl").Value.First();
                Response.Redirect($"MyApp/ExternalLogin?returnUrl={redirectUrl}");

                // return true to stop next middlewares from being called
                return true;
            }

            // if the request does not carry auth info from Facecook do nothing
            return false;
        }


        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            // get the token from the request
            var code = Request.Query.First(q => q.Key == "auth_code").Value.First();

            // get the user from Facecook using the provided authorization code
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{this.Request.Uri.Scheme}://{this.Request.Uri.Host}:{this.Request.Uri.Port}/facecook/token?auth_code={code}");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = new HttpClient().SendAsync(request, Request.CallCancelled).Result;
            response.EnsureSuccessStatusCode();
            var user = await response.Content.ReadAsStringAsync();

            // create the external user claims identity
            var externalCookieMiddlewareAuthenticationType = "ExternalApp";
            var externalUser = new ClaimsIdentity(externalCookieMiddlewareAuthenticationType);
            externalUser.AddClaim(new Claim("name", user));
            externalUser.AddClaim(new Claim("issuer", "Facecook"));


            var externalUserTicket = new AuthenticationTicket(externalUser, null);
            return externalUserTicket;
        }

    }
}