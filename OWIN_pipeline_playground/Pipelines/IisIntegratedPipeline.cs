using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Owin;

namespace Pipelines
{
    public class IisIntegratedPipeline
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use((context, next) =>
            {
                PrintCurrentIntegratedPipelineStage(context, "Middleware 1");
                return next.Invoke();
            });
            app.UseStageMarker(PipelineStage.Authorize);
            app.Use((context, next) =>
            {
                PrintCurrentIntegratedPipelineStage(context, "2nd MW");
                return next.Invoke();
            });
            app.UseStageMarker(PipelineStage.Authenticate);
            app.Run(context =>
            {
                PrintCurrentIntegratedPipelineStage(context, "3rd MW");
                return context.Response.WriteAsync("Hello world");
            });
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