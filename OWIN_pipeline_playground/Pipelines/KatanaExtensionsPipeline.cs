using Owin;

namespace Pipelines
{
    public class KatanaExtensionsPipeline
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.Map("/favicon.ico", appNotFound => { });
            appBuilder.UseGenericMiddleware("m1");
            appBuilder.UseGenericMiddleware("m2");
            appBuilder.Map("/custom", m3app => m3app.UseGenericMiddleware("custom"));
            appBuilder.UseGenericMiddleware("m3");
            appBuilder.Use(typeof(BareBonesMiddleware));
        }
    }
}