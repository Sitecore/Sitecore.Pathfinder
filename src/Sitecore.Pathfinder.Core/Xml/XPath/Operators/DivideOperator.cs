using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Operators
{
    public class DivideOperator : BinaryOperator
    {
        public DivideOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is int && right is int)
            {
                return (int)left / (int)right;
            }

            throw new XPathException("Type mismatch");
        }
    }
}