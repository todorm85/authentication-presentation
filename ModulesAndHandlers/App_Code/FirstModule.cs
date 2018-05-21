using System;
using System.IO;
using System.Text;
using System.Web;

namespace ModulesAndHandlers.App_Code
{
    public class FirstModule : IHttpModule
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
            HttpContext.Current.Response.Write("First_Auth<br>");
        }

        private void OnLogRequest(object source, EventArgs e)
        {
            HttpContext.Current.Response.Write("First_Log<br>");
        }
    }
}
