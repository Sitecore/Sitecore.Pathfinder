// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Pathfinder.Xml.XPath.Functions
{
    public class EndsWithFunction : FunctionBase
    {
        public EndsWithFunction() : base("endswith")
        {
        }

        public override object Evaluate(FunctionArgs args)
        {
            if (args.Arguments.Length != 2)
            {
                throw new XPathException("Too many or to few arguments in EndsWith()");
            }

            var a0 = args.Arguments[0].Evaluate(args.XPathExpression, args.Context);
            var a1 = args.Arguments[1].Evaluate(args.XPathExpression, args.Context);

            if (!(a0 is string && a1 is string))
            {
                throw new XPathException("String expression expected in EndsWith()");
            }

            return a0.ToString().EndsWith(a1.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
