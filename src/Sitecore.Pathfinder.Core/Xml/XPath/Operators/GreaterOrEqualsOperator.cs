using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Operators
{
    public class GreaterOrEqualsOperator : BinaryOperator
    {
        public GreaterOrEqualsOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is int && right is int)
            {
                return (int)left >= (int)right;
            }

            return left.ToString().CompareTo(right.ToString()) >= 0;
        }
    }
}