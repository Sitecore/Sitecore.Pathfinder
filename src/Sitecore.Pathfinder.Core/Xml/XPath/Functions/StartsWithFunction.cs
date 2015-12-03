// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Pathfinder.Xml.XPath.Functions
{
    public class StartsWithFunction : FunctionBase
    {
        public StartsWithFunction() : base("startswith")
        {
        }

        public override object Evaluate(FunctionArgs args)
        {
            if (args.Arguments.Length != 2)
            {
                throw new XPathException("Too many or to few arguments in StartsWith()");
            }

            var a0 = args.Arguments[0].Evaluate(args.XPathExpression, args.Context);
            var a1 = args.Arguments[1].Evaluate(args.XPathExpression, args.Context);

            if (!(a0 is string && a1 is string))
            {
                throw new XPathException("String expression expected in StartsWith()");
            }

            return a0.ToString().StartsWith(a1.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
