// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Axes
{
    public class AncestorOrSelfAxis : AncestorAxis
    {
        public AncestorOrSelfAxis([NotNull] ElementBase element) : base(element)
        {
        }

        protected override IXPathItem GetContextNode(object context)
        {
            return context as IXPathItem;
        }
    }
}
