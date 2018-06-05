using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityServer3.Core.Models;

namespace MVCClient
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
            {
            new Client
            {
                Enabled = true,
                ClientName = "MVC Client",
                ClientId = "mvc",
                Flow = Flows.Implicit,

                RedirectUris = new List<string>
                {
                    "http://localhost:57694/"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "http://localhost:57694/"
                },
                AllowAccessToAllScopes = true
            }
        };
        }
    }
}