// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Reflection;
using System.Web;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pipelines;

namespace Sitecore.Pathfinder.Pipelines.Loader
{
    public class ExecuteLoaderPipeline
    {
        public void Process([NotNull] PipelineArgs args)
        {
            try
            {
                var app = WebsiteHost.Host;
                if (app == null)
                {
                    return;
                }

                var pipelines = app.CompositionService.Resolve<IPipelineService>();
                pipelines.Resolve<LoaderPipeline>().Execute();
            }
            catch (ReflectionTypeLoadException ex)
            {
                var statusDescription = new StringWriter();

                statusDescription.WriteLine(ex.Message);
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    statusDescription.WriteLine();
                    statusDescription.WriteLine(loaderException.Message);
                }

                HttpContext.Current.Response.Write(statusDescription.ToString());
                HttpContext.Current.Response.Flush();

                throw;
            }
        }
    }
}
