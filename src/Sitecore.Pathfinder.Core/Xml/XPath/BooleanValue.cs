// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class BooleanValue : Opcode
    {
        public BooleanValue(bool value)
        {
            Value = value;
        }

        public bool Value { get; }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            return Value;
        }
    }
}
