// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Mvc.Names;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Mvc.Response.GetRenderer;

namespace Sitecore.Pathfinder.PageHtml.PageHtml
{
    public class PageHtmlGetRendererProcessor : PipelineProcessorBase<GetRendererPipeline>
    {
        // must come before HtmlGetRendererProcessor
        public PageHtmlGetRendererProcessor() : base(500)
        {
        }

        protected override void Process(GetRendererPipeline pipeline)
        {
            var filePath = GetViewPath(pipeline);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            if (!filePath.EndsWith(".page.html", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            pipeline.Renderer = new PageHtmlRenderer
            {
                FilePath = filePath
            };

            pipeline.Abort();
        }

        [Diagnostics.CanBeNull]
        private string GetViewPath([Diagnostics.NotNull] GetRendererPipeline pipeline)
        {
            return GetViewPathFromRenderingType(pipeline) ?? GetViewPathFromRenderingItem(pipeline);
        }

        [Diagnostics.CanBeNull]
        private string GetViewPathFromRenderingItem([Diagnostics.NotNull] GetRendererPipeline pipeline)
        {
            var renderingItem = pipeline.Rendering.RenderingItem;
            if (renderingItem == null || renderingItem.InnerItem.TemplateID != TemplateIds.ViewRendering)
            {
                return null;
            }

            var filePath = renderingItem.InnerItem["Path"];
            return string.IsNullOrWhiteSpace(filePath) ? null : filePath;
        }

        [Diagnostics.CanBeNull]
        private string GetViewPathFromRenderingType([Diagnostics.NotNull] GetRendererPipeline pipeline)
        {
            if (pipeline.Rendering.RenderingType == "r")
            {
                var filePath = pipeline.Rendering.RenderingItem.InnerItem["Path"];
                return string.IsNullOrWhiteSpace(filePath) ? null : filePath;
            }

            if (pipeline.Rendering.RenderingType == "View")
            {
                var filePath = pipeline.Rendering["Path"];
                return string.IsNullOrWhiteSpace(filePath) ? null : filePath;
            }

            if (pipeline.Rendering.RenderingType == "Layout")
            {
                var filePath = pipeline.LayoutItem.FilePath;
                return string.IsNullOrWhiteSpace(filePath) ? null : filePath;
            }

            return null;
        }
    }
}
