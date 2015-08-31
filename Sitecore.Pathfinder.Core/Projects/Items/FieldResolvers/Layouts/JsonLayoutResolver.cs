// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Projects.Layouts;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers.Layouts
{
    public class JsonLayoutResolver : LayoutResolverBase
    {
        protected override void WriteRendering(LayoutResolveContext context, XmlTextWriter output, IEnumerable<Rendering> renderingItems, ITextNode renderingTextNode, string placeholders)
        {
            renderingTextNode = renderingTextNode.ChildNodes.FirstOrDefault();
            if (renderingTextNode != null)
            {
                base.WriteRendering(context, output, renderingItems, renderingTextNode, placeholders);
            }
        }
    }
}
