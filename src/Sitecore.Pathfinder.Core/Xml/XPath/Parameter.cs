// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class Parameter : ElementBase
    {
        public Parameter([NotNull] string name) : base(name)
        {
        }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            object result;
            if (xpath.Parameters.TryGetValue(Name, out result))
            {
                return result;
            }

            return null;
        }
    }
}
