// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Mvc.Presentation;

namespace Sitecore.Pathfinder.Mvc.Response.GetRenderer
{
    public class GetRenderer : GetRendererProcessor
    {
        public override void Process([Diagnostics.NotNull] GetRendererArgs args)
        {
            if (args.Result != null)
            {
                return;
            }

            var app = WebsiteHost.App;
            if (app == null)
            {
                return;
            }

            var pipelines = app.CompositionService.Resolve<IPipelineService>();
            var pipeline = pipelines.Resolve<GetRendererPipeline>().Execute(args);

            if (pipeline.Renderer != null)
            {
                args.Result = new RendererWrapper(pipeline.Renderer);
            }
        }
    }
}
