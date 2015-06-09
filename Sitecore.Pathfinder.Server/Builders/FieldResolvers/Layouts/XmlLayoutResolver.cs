// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Documents;

namespace Sitecore.Pathfinder.Builders.FieldResolvers.Layouts
{
    public class XmlLayoutResolver : LayoutResolverBase
    {
        public override string Resolve(LayoutResolveContext context, ITextNode textNode)
        {
            var layoutTextNode = textNode.ChildNodes.FirstOrDefault();
            if (layoutTextNode == null)
            {
                return string.Empty;
            }

            return base.Resolve(context, layoutTextNode);
        }
    }
}
