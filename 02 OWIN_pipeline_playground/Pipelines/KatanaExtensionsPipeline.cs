using System.Threading.Tasks;
using Microsoft.Owin.Extensions;
using Owin;

namespace Pipelines
{
    public class KatanaExtensionsPipeline
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.UseGenericMiddleware("m1");
            appBuilder.UseGenericMiddleware("m2");
            appBuilder.UseGenericMiddleware("m3");
            appBuilder.Use(typeof(BareBonesMiddleware));
            //appBuilder.Run(env =>
            //{
            //    env.Response.Write("Stopping pipeline.\n");
            //    return Task.FromResult<object>(null);
            //});
        }
    }
}