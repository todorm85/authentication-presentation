using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace OAuth2CodeGrant.Controllers
{
    public class MyAppController : Controller
    {
        private static Dictionary<string, string> tokens = new Dictionary<string, string>();
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("external-profile")]
        public ActionResult ExternalProfile()
        {
            string accessToken = null;
            var sessionIdCookie = Request.Cookies.Get("session");
            if (sessionIdCookie != null)
            {
                tokens.TryGetValue(sessionIdCookie.Value, out accessToken);
            }

            if (accessToken == null)
            {
                var redirectUri = HttpUtility.UrlEncode($"{this.Request.Url}");
                return this.Redirect($"/myApp/external-authorize?redirect_uri={redirectUri}");
            }
            else
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, 
                    $@"{this.Request.Url.Scheme}://{this.Request.Url.Host}:{this.Request.Url.Port}/api/profile/age");

                https://tools.ietf.org/html/rfc6749#section-7
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
                var response = new HttpClient().SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                this.ViewBag.Age = response.Content.ReadAsStringAsync().Result;
                return View("Index");
            }
        }

        [ActionName("external-authorize")]
        public ActionResult ExternalAuthorize()
        {
            var redirectUri = HttpUtility.UrlEncode($"/myApp/external-authorize-cb?redirect_uri={this.Request.QueryString.Get("redirect_uri")}");

            // https://tools.ietf.org/html/rfc6749#section-4.1.1
            return this.Redirect($"/AuthServer/authorize?response_type=code&client_id=cool_app&redirect_uri={redirectUri}");
        }

        [ActionName("external-authorize-cb")]
        public ActionResult ExternalAuthorizeCb()
        {
            var authCode = this.Request.QueryString.Get("auth_code");

            https://tools.ietf.org/html/rfc6749#section-4.1.3
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, 
                $@"{this.Request.Url.Scheme}://{this.Request.Url.Host}:{this.Request.Url.Port}/AuthServer/token?grant_type=code&code={authCode}&client_id=cool_app&client_secret=secret");
            var response = new HttpClient().SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var accessToken = response.Content.ReadAsStringAsync().Result;

            var sessionId = Guid.NewGuid().ToString();
            var sessionCookie = new HttpCookie("session", sessionId);
            sessionCookie.Path = "myApp";
            this.Response.Cookies.Add(sessionCookie);
            tokens.Add(sessionId, accessToken);

            var redirectUri = this.Request.QueryString.Get("redirect_uri");
            return this.Redirect(redirectUri);
        }
    }
}
