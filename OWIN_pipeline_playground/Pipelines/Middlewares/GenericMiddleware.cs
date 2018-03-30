using Owin;

namespace Pipelines
{
    public static class GenericMiddleware
    {
        public static void UseGenericMiddleware(this IAppBuilder app, string title)
        {
            app.Use(async (ctx, next) =>
            {
                await ctx.Response.WriteAsync(title + " preprocess\n");
                await next();
                await ctx.Response.WriteAsync(title + " postprocess\n");
            });
        }
    }
}