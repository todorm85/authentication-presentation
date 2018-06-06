using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(OwinSimpleCookieAuthMVC.Startup))]

namespace OwinSimpleCookieAuthMVC
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = "MyApp",
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                LoginPath = new PathString("/MyApp/login")
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = "ExternalApp",
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive
            });

            app.Use<FacecookAuthenticationMiddleware>(new object[] { new FacecookAuthOpt("Facecook") });
        }
    }
}