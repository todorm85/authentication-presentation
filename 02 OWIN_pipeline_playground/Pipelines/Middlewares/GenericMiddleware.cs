using Owin;

namespace Pipelines
{
    public static class GenericMiddleware
    {
        public static void UseGenericMiddleware(this IAppBuilder app, string title)
        {
            app.Use(async (ctx, next) =>
            {
                ctx.Response.Write(title + " preprocess\n");
                await next();
                ctx.Response.Write(title + " postprocess\n");
            });
        }
    }
}