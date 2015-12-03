// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Xml.XPath.Functions
{
    public class NotFunction : FunctionBase
    {
        public NotFunction() : base("not")
        {
        }

        public override object Evaluate(FunctionArgs args)
        {
            if (args.Arguments.Length != 1)
            {
                throw new XPathException("Too many or to few arguments in not()");
            }

            var a0 = args.Arguments[0].Evaluate(args.XPathExpression, args.Context);

            if (!(a0 is bool))
            {
                throw new XPathException("Boolean expression expected in not()");
            }

            return !(bool)a0;
        }
    }
}
