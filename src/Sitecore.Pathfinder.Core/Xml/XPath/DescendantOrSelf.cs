// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class DescendantOrSelf : Descendant
    {
        public DescendantOrSelf([NotNull] ElementBase element) : base(element)
        {
        }

        public override object Evaluate(Query query, object context)
        {
            object result = null;

            TestNode(query, context, ref result);

            Iterate(query, context, ref result);

            return result;
        }
    }
}
