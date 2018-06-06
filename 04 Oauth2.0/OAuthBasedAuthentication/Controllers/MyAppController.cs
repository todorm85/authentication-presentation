using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Host.SystemWeb;

namespace OwinSimpleCookieAuthMVC.Controllers
{
    public class MyAppController : Controller
    {
        public virtual string AuthenticationType => "MyApp";

        [Authorize]
        [ActionName("profile")]
        public ActionResult ProfileView()
        {
            var ctx = HttpContext.GetOwinContext();
            ViewBag.username = ctx.Authentication.User.Claims.First(x => x.Type == "name").Value;
            ViewBag.provider = ctx.Authentication.User.Claims.First(x => x.Type == "provider").Value;
            return View();
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public virtual ActionResult Login(string user, string pass)
        {
            var ctx = HttpContext.GetOwinContext();
            var identity = new ClaimsIdentity(AuthenticationType);
            identity.AddClaim(new Claim("name", user));
            identity.AddClaim(new Claim("provider", "local"));
            ctx.Authentication.SignIn(identity);
            return this.Redirect(this.Request.QueryString.Get("returnUrl"));
        }

        [Authorize]
        public ActionResult Logout()
        {
            var ctx = HttpContext.GetOwinContext();
            ctx.Authentication.SignOut("MyApp");
            var prv = ctx.Authentication.User.Claims.First(x => x.Type == "provider").Value;
            if (prv != "local")
            {
                ctx.Authentication.SignOut("ExternalApp");
            }

            return this.Redirect("/");
        }

        public ActionResult ExternalLogin()
        {
            var ctx = HttpContext.GetOwinContext();

            // check if an external user is already logged in
            var externalUser = ctx.Authentication.AuthenticateAsync("ExternalApp").Result;
            if (externalUser == null || !externalUser.Identity.IsAuthenticated)
            {
                // if no external user is logged in challenge the requested middleware to authenticate it
                var authenticationType = this.Request.QueryString.Get("authType");
                ctx.Authentication.Challenge(authenticationType);
                return new HttpUnauthorizedResult();
            }

            // if there`s external identity logged in, try to see if it is mapped to a local account and log the mapped account in
            // the local account should be logged in by the MyApp cookie authentication middleware
            var localUser = new ClaimsIdentity("MyApp");
            localUser.AddClaim(new Claim("name", externalUser.Identity.Claims.First(x => x.Type == "name").Value));
            localUser.AddClaim(new Claim("provider", externalUser.Identity.Claims.First(x => x.Type == "issuer").Value));
            ctx.Authentication.SignIn(localUser);
            return Redirect(Request.QueryString.Get("returnUrl"));
            // if the external identity does not match any local account return the login confirmation screen where the user will be prompted to create a local account
            //return View("ExternalLoginConfirmation");
        }
    }
}