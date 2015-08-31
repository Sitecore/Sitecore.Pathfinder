// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Emitters.FieldResolvers.Layouts
{
    public class JsonLayoutResolver : LayoutResolverBase
    {
        protected override void WriteRendering(LayoutResolveContext context, XmlTextWriter output, IEnumerable<Item> renderingItems, Database database, ITextNode renderingTextNode, string placeholders)
        {
            renderingTextNode = renderingTextNode.ChildNodes.FirstOrDefault();
            if (renderingTextNode != null)
            {
                base.WriteRendering(context, output, renderingItems, database, renderingTextNode, placeholders);
            }
        }
    }
}
