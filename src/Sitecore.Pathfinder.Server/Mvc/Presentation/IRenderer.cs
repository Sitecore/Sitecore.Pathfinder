// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.IO;

namespace Sitecore.Pathfinder.Mvc.Presentation
{
    public interface IRenderer
    {
        void Render([Diagnostics.NotNull] TextWriter writer);
    }
}
