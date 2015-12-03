// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Xml.XPath.Functions
{
    public class NumberFunction : FunctionBase
    {
        public NumberFunction() : base("number")
        {
        }

        public override object Evaluate(FunctionArgs args)
        {
            if (args.Arguments.Length != 1)
            {
                throw new XPathException("Too many or to few arguments in number()");
            }

            var a0 = args.Arguments[0].Evaluate(args.XPathExpression, args.Context);

            var s = a0 as string;

            if (s != null)
            {
                int i;

                if (!int.TryParse(s, out i))
                {
                    throw new XPathException("Integer expression expected in number()");
                }

                a0 = i;
            }

            if (!(a0 is int))
            {
                throw new XPathException("Integer expression expected in number()");
            }

            return (int)a0;
        }
    }
}
