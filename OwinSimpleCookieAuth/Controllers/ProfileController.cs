using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OwinCookieAuthMVC.Controllers
{
    public class ProfileController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var ctx = HttpContext.GetOwinContext();
            ViewBag.username = ctx.Authentication.User.Claims.First(x => x.Type == "name").Value;
            ViewBag.provider = ctx.Authentication.User.Claims.First(x => x.Type == "provider").Value;
            return View();
        }
    }
}