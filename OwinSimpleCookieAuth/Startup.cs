using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using OwinSimpleCookieAuthMVC.Controllers;

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
                LoginPath = new PathString("/account/login"),
                Provider = new CookieAuthenticationProvider()
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = "ExternalApp",
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive
            });

            app.Use<FacecookAuthenticationMiddleware>(new object[] { new FacecookAuthOpt("Facecook") });
        }
    }

    public class CookieAuthenticationProvider : ICookieAuthenticationProvider
    {
        public void ApplyRedirect(CookieApplyRedirectContext context)
        {
            context.Response.Redirect(context.RedirectUri);
        }

        public void Exception(CookieExceptionContext context)
        {
        }

        public void ResponseSignedIn(CookieResponseSignedInContext context)
        {
        }

        public void ResponseSignIn(CookieResponseSignInContext context)
        {
        }

        public void ResponseSignOut(CookieResponseSignOutContext context)
        {
        }

        public Task ValidateIdentity(CookieValidateIdentityContext context)
        {
            return Task.FromResult(context);
        }
    }
}