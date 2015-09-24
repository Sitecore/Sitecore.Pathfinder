// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers.Layouts
{
    public class JsonLayoutResolver : LayoutResolver
    {
        protected override void WriteRendering(LayoutResolveContext context, XmlTextWriter output, IEnumerable<Item> renderingItems, ITextNode renderingTextNode, string placeholders)
        {
            var childNode = renderingTextNode.ChildNodes.FirstOrDefault();
            if (childNode != null)
            {
                base.WriteRendering(context, output, renderingItems, childNode, placeholders);
            }
        }

        protected override string GetPlaceholders(LayoutResolveContext context, ITextNode renderingTextNode, IProjectItem projectItem)
        {
            var childTextNode = renderingTextNode;
            if (childTextNode.Parent != null && childTextNode.Parent.Name == "Renderings")
            {
                childTextNode = childTextNode.ChildNodes.FirstOrDefault();
                if (childTextNode == null)
                {
                    return string.Empty;
                }
            }

            return base.GetPlaceholders(context, childTextNode, projectItem);
        }
    }
}
