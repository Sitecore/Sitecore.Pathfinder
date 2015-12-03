// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Pathfinder.Xml.XPath.Functions
{
    public class ContainsFunction : FunctionBase
    {
        public ContainsFunction() : base("contains")
        {
        }

        public override object Evaluate(FunctionArgs args)
        {
            if (args.Arguments.Length != 2)
            {
                throw new XPathException("Too many or to few arguments in Contains()");
            }

            var a0 = args.Arguments[0].Evaluate(args.XPathExpression, args.Context);
            var a1 = args.Arguments[1].Evaluate(args.XPathExpression, args.Context);

            if (!(a0 is string && a1 is string))
            {
                throw new XPathException("String expression expected in Contains()");
            }

            return a0.ToString().IndexOf(a1.ToString(), StringComparison.InvariantCulture) >= 0;
        }
    }
}
