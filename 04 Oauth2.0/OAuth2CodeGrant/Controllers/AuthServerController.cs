using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace OAuth2CodeGrant.Controllers
{
    public class AuthServerController : Controller
    {
        private static Dictionary<string, string> codes = new Dictionary<string, string>();
        private static Dictionary<string, string> tokens = new Dictionary<string, string>();
        private static Dictionary<string, string> registeredClients = new Dictionary<string, string>();

        static AuthServerController()
        {
            registeredClients.Add("cool_app", "secret");
        }

        public ActionResult Login()
        {
            ViewBag.returnUrl = HttpUtility.UrlEncode(this.Request.QueryString["redirect_uri"]);
            return View();
        }

        [HttpPost]
        public ActionResult Login(string user, string pass)
        {
            var cookie = new HttpCookie("authserverlogin", user);
            cookie.Path = "/AuthServer";
            this.Response.Cookies.Add(cookie);
            return this.Redirect(this.Request.QueryString.Get("redirect_uri"));
        }

        [ActionName("authorize")]
        public ActionResult AuthorizeView()
        {
            var clientId = Request.QueryString.Get("client_id");
            if (clientId == null || !registeredClients.ContainsKey(clientId))
            {
                return Json("Client not found", JsonRequestBehavior.AllowGet);
            }

            var loggedInUser = this.Request.Cookies.Get("authserverlogin");
            if (loggedInUser != null)
            {
                ViewBag.client = clientId;
                ViewBag.user = loggedInUser.Value;
                return View("Authorize");
            }
            else
            {
                var originalRedirectUrl = this.Request.QueryString.Get("redirect_uri");
                var returnUrl = HttpUtility.UrlEncode($"/AuthServer/authorize?redirect_uri={originalRedirectUrl}&client_id={clientId}");
                return this.Redirect($"login?redirect_uri={returnUrl}");
            }
        }

        [HttpPost]
        public virtual ActionResult Authorize()
        {
            var code = Guid.NewGuid().ToString();
            var loggedInUser = this.Request.Cookies.Get("authserverlogin");
            codes.Add(code, loggedInUser.Value);

            https://tools.ietf.org/html/rfc6749#section-4.1.2
            var url = $"{Request.QueryString.Get("redirect_uri")}&auth_code={code}";
            return this.Redirect(url);
        }

        public ActionResult Token()
        {
            var code = Request.QueryString.Get("code");
            var clientId = Request.QueryString.Get("client_id");
            var secret = Request.QueryString.Get("client_secret");
            var grant = Request.QueryString.Get("grant_type");
            if (grant == null || grant != "code")
            {
                return Json("Only authorization code grant is supported.", JsonRequestBehavior.AllowGet);
            }

            var user = codes[code];
            if (user == null)
            {
                return Json("Invalid code", JsonRequestBehavior.AllowGet);
            }

            var clientSecret = registeredClients[clientId];
            if (clientSecret == null || clientSecret != secret)
            {
                return Json("Invalid client", JsonRequestBehavior.AllowGet);
            }

            var token = Guid.NewGuid().ToString();
            tokens.Add(token, user);
            return new ContentResult() { Content = token };
        }

        [ActionName("validate-token")]
        public ActionResult ValidateToken()
        {
            var token = Request.QueryString.Get("access_token");
            var user = tokens[token];
            if (user == null)
            {
                return Json("Invalid token!", JsonRequestBehavior.AllowGet);
            }

            return new ContentResult() { Content = user };
        }
    }
}