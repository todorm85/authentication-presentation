using System.Threading.Tasks;
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
                PrintCurrentIntegratedPipelineStage(context, "Middleware 0");
                return next.Invoke();
            });
            app.UseStageMarker(PipelineStage.MapHandler);
            app.Map("/authorize", newApp =>
            {
                newApp.Use((context, next) =>
                {
                    PrintCurrentIntegratedPipelineStage(context, "1st MW");
                    return Task.FromResult(0);
                });
                newApp.UseStageMarker(PipelineStage.Authorize);

            });
            app.Use((context, next) =>
            {
                PrintCurrentIntegratedPipelineStage(context, "2nd MW");
                return next.Invoke();
            });
            //app.UseStageMarker(PipelineStage.Authenticate);
            app.Use((context, next) =>
            {
                PrintCurrentIntegratedPipelineStage(context, "3d MW");
                return next.Invoke();
            });
            app.UseStageMarker(PipelineStage.PostAuthorize);
            app.Run(context =>
            {
                PrintCurrentIntegratedPipelineStage(context, "Last");
                return Task.FromResult<object>(null);
            });
            app.UseStageMarker(PipelineStage.Authorize);
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