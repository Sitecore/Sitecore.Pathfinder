// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Mvc.Presentation;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Mvc.Presentation;

namespace Sitecore.Pathfinder.Mvc.Response.GetRenderer
{

    public class GetRendererPipeline : PipelineBase<GetRendererPipeline>
    {
        [NotNull]
        public LayoutItem LayoutItem { get; private set; }

        [CanBeNull]
        public IRenderer Renderer { get; set; }

        [NotNull]
        public Rendering Rendering { get; private set; }

        [NotNull]
        public Template RenderingTemplate { get; private set; }

        [NotNull]
        public GetRendererPipeline Execute([NotNull] GetRendererArgs args)
        {
            LayoutItem = args.LayoutItem;
            Rendering = args.Rendering;
            RenderingTemplate = args.RenderingTemplate;

            Execute();

            return this;
        }
    }
}
