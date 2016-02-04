// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.IO;

namespace Sitecore.Mvc.Presentation
{
    public abstract class Renderer
    {
        public abstract void Render(TextWriter writer);
    }
}