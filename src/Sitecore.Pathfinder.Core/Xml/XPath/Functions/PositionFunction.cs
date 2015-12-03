// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;

namespace Sitecore.Pathfinder.Xml.XPath.Functions
{
    public class PositionFunction : FunctionBase
    {
        public PositionFunction() : base("position")
        {
        }

        public override object Evaluate(FunctionArgs args)
        {
            if (args.Arguments.Length != 0)
            {
                throw new XPathException("No arguments allowed in position()");
            }

            var item = args.Context as IXPathItem;
            if (item == null)
            {
                return -1;
            }

            var parent = item.GetParent();
            if (parent == null)
            {
                return -1;
            }

            var children = parent.GetChildren().ToArray();
            for (var n = 0; n < children.Length; n++)
            {
                if (children[n] == args.Context)
                {
                    return n + 1;
                }
            }

            return -1;
        }
    }
}
