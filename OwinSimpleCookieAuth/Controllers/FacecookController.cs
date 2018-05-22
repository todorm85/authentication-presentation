using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OwinCookieAuthMVC.Controllers
{
    public class FacecookController : Controller
    {
        private static string loggedInUser = null;
        private static Dictionary<string, string> codes = new Dictionary<string, string>();

        public ActionResult Login()
        {
            ViewBag.returnUrl = Uri.EscapeUriString(this.Request.QueryString["returnUrl"]);
            return View();
        }

        [HttpPost]
        public ActionResult Login(string user, string pass)
        {
            FacecookController.loggedInUser = user;
            return this.Redirect("/" + this.Request.QueryString.Get("returnUrl"));
        }

        [ActionName("authorize")]
        public ActionResult AuthorizeView()
        {
            var originalRedirectUrl = this.Request.QueryString.Get("returnUrl");
            if (loggedInUser != null)
            {
                ViewBag.user = loggedInUser;
                return View("Authorize");
            }
            else
            {
                var returnUrl = Uri.EscapeUriString($"facecook/authorize?returnUrl={originalRedirectUrl}");
                return this.Redirect($"login?returnUrl={returnUrl}");
            }
        }

        [HttpPost]
        public virtual ActionResult Authorize()
        {
            var token = Guid.NewGuid().ToString();
            codes.Add(token, loggedInUser);
            var url = $"{Request.QueryString.Get("returnUrl")}&amp;auth_code={token}";
            return this.Redirect(url);
        }

        public ActionResult Token()
        {
            var code = Request.QueryString.Get("auth_code");
            var user = codes[code];
            return new ContentResult() { Content = user };
        }

        public ActionResult Logout()
        {
            loggedInUser = null;
            return new ContentResult() { Content = "You were logged out of Facecook" };
        }
    }
}