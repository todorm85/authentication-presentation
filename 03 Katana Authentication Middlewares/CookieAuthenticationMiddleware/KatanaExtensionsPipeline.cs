using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace CookieAuthenticationMiddleware
{
    public class KatanaExtensionsPipeline
    {
        const string LoginPageMarkup = @"<form action=""/authenticate"" method=""GET"">
            <fieldset>
                <label>Name
                    <input type=""text"" name=""name"">
                </label>
            </fieldset>
            <input type=""submit"">
        </form>";

        const string CookieAuthenticationType = "cookie";

        public void Configuration(IAppBuilder app)
        {
            app.Map("/favicon.ico", appNotFound => { });

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = CookieAuthenticationType,
                LoginPath = new PathString("/login") // causes unauthorized requests to be redirected to the path
            });

            app.Map("/login", branchedApp =>
            {
                branchedApp.Use((env, next) =>
                {
                    if (env.IsUserAuthenticated())
                    {
                        env.Response.Write("You are already authenticated.");
                    }
                    else
                    {
                        env.Response.ContentType = "text/html";
                        env.Response.Write(LoginPageMarkup);
                    }

                    return Task.FromResult(0);
                });
            });

            app.Map("/logout", branchedApp =>
            {
                branchedApp.Use((env, next) =>
                {
                    if (env.IsUserAuthenticated())
                    {
                        env.Authentication.SignOut(CookieAuthenticationType);
                        env.Response.ContentType = "text/html";
                        env.Response.Write("You are logged out.<a href=\"/\">Home</a>");
                    }
                    else
                    {
                        env.Response.ContentType = "text/html";
                        env.Response.Write("You are not logged in.");
                    }

                    return Task.FromResult(0);
                });
            });

            app.Map("/authenticate", branchedApp =>
            {
                branchedApp.Use((env, next) =>
                {
                    var names = env.Request.Query.GetValues("name");
                    if (names == null || names.Count != 1)
                    {
                        env.Response.Write("Invalid login info");
                    }
                    else
                    {
                        var name = names[0];
                        var identity = new ClaimsIdentity(CookieAuthenticationType);
                        identity.AddClaim(new Claim("name", name));
                        env.Authentication.SignIn(identity);
                        env.Response.ContentType = "text/html";
                        env.Response.Write("<div>Successfully logged in.</div><a href=\"/\">Home</a>");
                    }

                    return Task.FromResult(0);
                });
            });

            app.Use(async (env, next) =>
            {
                if (!env.IsUserAuthenticated())
                {
                    env.Response.StatusCode = 401; // causes the cookie auth middleware to redirect the request
                    // next is not called which causes the request pipeline to break here
                    env.Authentication.Challenge(CookieAuthenticationType);
                }
                else
                {
                    env.Response.ContentType = "text/html";
                    env.Response.Write("<h1>Cookie Authentication Middleware Demo</h1><a href=\"/logout\">Logout</a>");
                    await next();
                }

            });
        }
    }

    public static class OwinContextExtensions
    {
        public static bool IsUserAuthenticated(this IOwinContext env)
        {
            var user = env.Authentication.User;
            return user != null && user.Identity.IsAuthenticated;
        }
    }
}