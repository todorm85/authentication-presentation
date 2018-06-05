using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OAuth2CodeGrant.Controllers
{
    public class ProfileController : ApiController
    {
        private Dictionary<string, int> UsersAges = new Dictionary<string, int>();

        public ProfileController()
        {
            this.UsersAges.Add("bob", 55);
        }

        [HttpGet]
        public string Age()
        {
            var accessToken = this.Request.Headers.Authorization.Parameter;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{this.Request.RequestUri.Scheme}://{this.Request.RequestUri.Host}:{this.Request.RequestUri.Port}/AuthServer/validate-token?access_token={accessToken}");
            var response = new HttpClient().SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var username = response.Content.ReadAsStringAsync().Result;
            int age = 0;
            this.UsersAges.TryGetValue(username, out age);
            return (age == 0) ? "no age found" : age.ToString();
        }
    }
}
