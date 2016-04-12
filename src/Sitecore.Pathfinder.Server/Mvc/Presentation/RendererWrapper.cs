// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Pathfinder.Mvc.Presentation
{
    public class RendererWrapper : Renderer
    {
        public RendererWrapper([NotNull] IRenderer renderer)
        {
            Renderer = renderer;
        }

        [NotNull]
        public IRenderer Renderer { get; }

        public override void Render([NotNull] TextWriter writer)
        {
            Renderer.Render(writer);
        }
    }
}
