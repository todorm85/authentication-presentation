using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace Pipelines
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class RawBranchMiddleware
    {
        protected AppFunc nextApp;
        private IAppBuilder builder;

        public RawBranchMiddleware(AppFunc next, IAppBuilder builder)
        {
            this.nextApp = next;
            this.builder = builder;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            IOwinContext context = new OwinContext(env);
            PathString path = context.Request.Path;
            if (path.Value.EndsWith("raw-branch"))
            {
                var newAppBuilder = this.builder.New();

                newAppBuilder.Use((newContext, next) =>
                {
                    IisIntegratedPipeline.PrintCurrentIntegratedPipelineStage(newContext, "BranchMiddleware 0");
                    return next.Invoke();
                });
                newAppBuilder.Run(newAppEnv => newAppEnv.Response.WriteAsync("raw branch\n"));

                var newApp = (AppFunc)newAppBuilder.Build(typeof(AppFunc));

                await newApp.Invoke(env);
            }
            else
            {
                await this.nextApp.Invoke(env);
            }
        }
    }
}