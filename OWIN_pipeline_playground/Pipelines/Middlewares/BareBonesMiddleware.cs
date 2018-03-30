using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Pipelines
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class BareBonesMiddleware
    {
        protected AppFunc next;

        public BareBonesMiddleware(AppFunc next)
        {
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            string responseText = "Hello World\n";
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);

            // See http://owin.org/spec/owin-1.0.0.html for standard environment keys.
            Stream responseStream = (Stream)environment["owin.ResponseBody"];

            IDictionary<string, string[]> responseHeaders =
                (IDictionary<string, string[]>)environment["owin.ResponseHeaders"];
            responseHeaders["Content-Type"] = new string[] { "text/plain" };

            await responseStream.WriteAsync(responseBytes, 0, responseBytes.Length);
        }
    }
}