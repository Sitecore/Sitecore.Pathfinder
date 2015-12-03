// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;

namespace Sitecore.Pathfinder.Xml.XPath.Functions
{
    public class LastFunction : FunctionBase
    {
        public LastFunction() : base("last")
        {
        }

        public override object Evaluate(FunctionArgs args)
        {
            if (args.Arguments.Length != 0)
            {
                throw new XPathException("No arguments allowed in last()");
            }

            var item = args.Context as IXPathItem;
            if (item == null)
            {
                return -1;
            }

            var parent = item.GetParent();
            if (parent != null)
            {
                return parent.GetChildren().Count();
            }

            return -1;
        }
    }
}
