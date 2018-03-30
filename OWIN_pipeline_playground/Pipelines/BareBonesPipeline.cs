using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace Pipelines
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class BareBonesPipeline
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use(new Func<AppFunc, AppFunc>(next => (async environment =>
            {
                // See http://owin.org/spec/owin-1.0.0.html for standard environment keys.
                Stream responseStream = (Stream)environment["owin.ResponseBody"];

                byte[] responseBytes = Encoding.UTF8.GetBytes("M1 Pre\n");
                var postResponseBytes = Encoding.UTF8.GetBytes("M1 Post\n");

                await responseStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                await next.Invoke(environment);
                await responseStream.WriteAsync(postResponseBytes, 0, postResponseBytes.Length);

            })));

            app.Use(typeof(BareBonesMiddleware));
        }
    }
}