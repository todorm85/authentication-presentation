using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IIS
{
    public class AspMod1 : IHttpModule
    {
        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += Context_AuthenticateRequest;
            context.PostAuthenticateRequest += Context_PostAuthenticateRequest;
            context.EndRequest += Context_EndRequest;
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Write("AspMod1_EndRequest\n");
        }

        private void Context_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Write("AspMod1_PostAuthenticateRequest\n");
        }

        private void Context_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Write("AspMod1_AuthenticateRequest\n");
        }
    }
}