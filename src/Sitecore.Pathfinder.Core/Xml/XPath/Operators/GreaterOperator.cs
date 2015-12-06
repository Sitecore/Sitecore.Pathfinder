using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Operators
{
    public class GreaterOperator : BinaryOperator
    {
        public GreaterOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is int && right is int)
            {
                return (int)left > (int)right;
            }

            return left.ToString().Equals(right.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}