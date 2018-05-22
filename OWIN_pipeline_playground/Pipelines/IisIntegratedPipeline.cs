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
            builder.Use((context, next) =>
            {
                PrintCurrentIntegratedPipelineStage(context, "Middleware 0");
                return next.Invoke();
            });

            builder.UseStageMarker(PipelineStage.MapHandler);

            builder.Use(typeof(RawBranchMiddleware), builder);

            builder.Map("/authorize", newApp =>
            {
                newApp.Use((context, next) =>
                {
                    PrintCurrentIntegratedPipelineStage(context, "1st MW");
                    return Task.FromResult(0);
                });
                newApp.UseStageMarker(PipelineStage.Authorize);

            });

            builder.Use((context, next) =>
            {
                PrintCurrentIntegratedPipelineStage(context, "2nd MW");
                return next.Invoke();
            });

            //app.UseStageMarker(PipelineStage.Authenticate);

            builder.Use((context, next) =>
            {
                PrintCurrentIntegratedPipelineStage(context, "3d MW");
                return next.Invoke();
            });

            builder.UseStageMarker(PipelineStage.PostAuthorize);

            builder.Run(context =>
            {
                PrintCurrentIntegratedPipelineStage(context, "Last");
                return Task.FromResult<object>(null);
            });

            builder.UseStageMarker(PipelineStage.Authorize);
        }

        private void PrintCurrentIntegratedPipelineStage(IOwinContext context, string msg)
        {
            msg = msg + "\n";
            var currentIntegratedpipelineStage = HttpContext.Current.CurrentNotification;
            context.Response.Write(
                "Current IIS event: " + currentIntegratedpipelineStage
                + " Msg: " + msg);
        }
    }
}