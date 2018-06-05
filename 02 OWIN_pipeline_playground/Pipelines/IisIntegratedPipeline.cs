using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Owin;

namespace Pipelines
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class IisIntegratedPipeline
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.Use(async (context, next) =>
            {
                PrintCurrentIntegratedPipelineStage(context, "Base Middleware PreHandler");
                await next.Invoke();
                //context.Response.OnSendingHeaders(param =>
                //{
                //    PrintCurrentIntegratedPipelineStage(context, "Base Middleware On sending headers");
                //}, null);
                PrintCurrentIntegratedPipelineStage(context, "Base Middleware AfterHandler");
            });

            builder.Use(typeof(RawBranchMiddleware), builder);

            builder.UseStageMarker(PipelineStage.PostAcquireState);


            builder.Map("/branched", newApp =>
            {
                newApp.Use((context, next) =>
                {
                    PrintCurrentIntegratedPipelineStage(context, "Authorize Branch MW");
                    return Task.FromResult(0);
                });

                newApp.UseStageMarker(PipelineStage.Authorize);
            });

            builder.Use((context, next) =>
            {
                PrintCurrentIntegratedPipelineStage(context, "2nd MW");
                return next.Invoke();
            });

            builder.UseStageMarker(PipelineStage.Authenticate);

            builder.Use((context, next) =>
            {
                PrintCurrentIntegratedPipelineStage(context, "3d MW");
                return next.Invoke();
            });

            //builder.UseStageMarker(PipelineStage.PostAuthorize);

            //builder.Run(context =>
            //{
            //    PrintCurrentIntegratedPipelineStage(context, "Last");
            //    return Task.FromResult<object>(null);
            //});

            //builder.UseStageMarker(PipelineStage.Authorize);
        }

        public static void PrintCurrentIntegratedPipelineStage(IOwinContext context, string msg)
        {
            msg = msg + "\n";
            var currentIntegratedpipelineStage = HttpContext.Current.CurrentNotification;
            context.Response.Write(
                "Current IIS event: " + currentIntegratedpipelineStage
                + " Msg: " + msg);
        }
    }
}