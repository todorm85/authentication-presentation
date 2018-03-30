using Owin;

namespace Pipelines
{
    public class KatanaExtensionsPipeline
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/favicon.ico", appNotFound => { });
            app.UseGenericMiddleware("m1");
            app.UseGenericMiddleware("m2");
            app.Map("/custom", m3app => m3app.UseGenericMiddleware("custom"));
            app.UseGenericMiddleware("m3");
            app.Use(typeof(BareBonesMiddleware));
        }
    }
}