using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IdentityModel.Client;
using IdentityServer3.Core.Models;
using Newtonsoft.Json.Linq;

namespace IdentityServer3Demo.Controllers
{
    public class CallApiController : Controller
    {
        // GET: CallApi/ClientCredentials
        public async Task<ActionResult> ClientCredentials()
        {
            var response = await GetTokenAsync();
            var result = await CallApi(response.AccessToken);

            ViewBag.Json = result;
            return View("Show");
        }

        public async Task<ActionResult> ResourceOwner()
        {
            var response = await GetResourceOwnerTokenAsync();
            var result = await CallApi(response.AccessToken);

            ViewBag.Json = result;
            return View("Show");
        }

        private async Task<IdentityModel.Client.TokenResponse> GetResourceOwnerTokenAsync()
        {
            var client = new TokenClient(
                "http://localhost:57054/identity/connect/token",
                "mvc_service_resource_owner",
                "secret");

            return await client.RequestResourceOwnerPasswordAsync("bob", "secret", "identityApi");
        }

        private async Task<IdentityModel.Client.TokenResponse> GetTokenAsync()
        {
            var client = new TokenClient(
                "http://localhost:57054/identity/connect/token",
                "mvc_service",
                "secret");

            return await client.RequestClientCredentialsAsync("identityApi");
        }

        private async Task<string> CallApi(string token)
        {
            var client = new HttpClient();
            client.SetBearerToken(token);

            var json = await client.GetStringAsync("http://localhost:62441/identity");
            return JArray.Parse(json).ToString();
        }
    }
}