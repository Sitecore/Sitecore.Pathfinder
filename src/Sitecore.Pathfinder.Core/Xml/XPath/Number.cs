// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class Number : Opcode
    {
        public Number(int number)
        {
            Value = number;
        }

        public int Value { get; }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            return Value;
        }
    }
}
