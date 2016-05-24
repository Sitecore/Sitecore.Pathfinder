// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pipelines;

namespace Sitecore.Pathfinder.Pipelines.Loader
{
    public class ExecuteLoaderPipeline
    {
        public void Process([NotNull] PipelineArgs args)
        {
            var app = WebsiteHost.Host;
            if (app == null)
            {
                return;
            }

            var pipelines = app.CompositionService.Resolve<IPipelineService>();
            pipelines.Resolve<LoaderPipeline>().Execute();
        }
    }
}
