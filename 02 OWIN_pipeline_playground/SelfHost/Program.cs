using System;
using System.Diagnostics;
using Microsoft.Owin.Hosting;
using Owin;
using Pipelines;

namespace SelfHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string baseUrl = "http://localhost:8080";

            using (WebApp.Start<KatanaExtensionsPipeline>(new StartOptions(baseUrl)))
            {
                Process.Start(baseUrl);
                Console.WriteLine("Press Enter to quit.");
                Console.ReadKey();
            }
        }
    }
}