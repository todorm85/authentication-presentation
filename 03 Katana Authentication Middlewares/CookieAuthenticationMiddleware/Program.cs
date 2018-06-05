using System;
using System.Diagnostics;
using Microsoft.Owin.Hosting;

namespace CookieAuthenticationMiddleware
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string baseUrl = "http://localhost:8081";

            using (WebApp.Start<KatanaExtensionsPipeline>(new StartOptions(baseUrl)))
            {
                Process.Start(baseUrl);
                Console.WriteLine("Press Enter to quit.");
                Console.ReadKey();
            }
        }
    }
}