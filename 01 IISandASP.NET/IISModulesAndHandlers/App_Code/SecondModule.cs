using System;
using System.Web;

namespace ModulesAndHandlers.App_Code
{
    public class SecondModule : IHttpModule
    {
        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            context.LogRequest += OnLogRequest;
            context.AuthenticateRequest += OnAuthenticateRequest;
        }

        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Write("Second_Auth<br>");
        }

        private void OnLogRequest(object source, EventArgs e)
        {
            HttpContext.Current.Response.Write("Second_Log<br>");
        }
    }
}
