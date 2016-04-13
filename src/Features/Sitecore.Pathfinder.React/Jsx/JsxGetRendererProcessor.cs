// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Names;
using Sitecore.Mvc.Presentation;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Mvc.Response.GetRenderer;

namespace Sitecore.Pathfinder.React.Jsx
{
    public class JsxGetRendererProcessor : PipelineProcessorBase<GetRendererPipeline>
    {
        public JsxGetRendererProcessor() : base(1000)
        {
        }

        [Diagnostics.CanBeNull]
        protected virtual string GetViewPath([Diagnostics.NotNull] GetRendererPipeline pipeline)
        {
            return GetViewPathFromRenderingType(pipeline) ?? GetViewPathFromRenderingItem(pipeline);
        }

        protected override void Process(GetRendererPipeline pipeline)
        {
            var filePath = GetViewPath(pipeline);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            if (!filePath.EndsWith(".jsx", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            pipeline.Renderer = new JsxRenderer(pipeline.Rendering, filePath);
            pipeline.Abort();
        }

        [Diagnostics.CanBeNull]
        private string GetViewPathFromInnerItem([Diagnostics.NotNull] Rendering rendering)
        {
            var filePath = rendering.RenderingItem.InnerItem["Path"];
            return string.IsNullOrWhiteSpace(filePath) ? null : filePath;
        }

        [Diagnostics.CanBeNull]
        private string GetViewPathFromLayoutItem([Diagnostics.NotNull] GetRendererPipeline pipeline)
        {
            var filePath = pipeline.LayoutItem.ValueOrDefault(item => item.FilePath);
            return string.IsNullOrWhiteSpace(filePath) ? null : filePath;
        }

        [Diagnostics.CanBeNull]
        private string GetViewPathFromPathProperty([Diagnostics.NotNull] Rendering rendering)
        {
            var filePath = rendering["Path"];
            return string.IsNullOrWhiteSpace(filePath) ? null : filePath;
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
                return GetViewPathFromInnerItem(pipeline.Rendering);
            }

            if (pipeline.Rendering.RenderingType == "View")
            {
                return GetViewPathFromPathProperty(pipeline.Rendering);
            }

            if (pipeline.Rendering.RenderingType == "Layout")
            {
                return GetViewPathFromLayoutItem(pipeline);
            }

            return null;
        }
    }
}
