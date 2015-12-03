// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Axes
{
    public class DescendantOrSelfAxis : DescendantAxis
    {
        public DescendantOrSelfAxis([NotNull] ElementBase element) : base(element)
        {
        }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            object result = null;

            TestNode(xpath, context, ref result);

            Iterate(xpath, context, ref result);

            return result;
        }
    }
}
