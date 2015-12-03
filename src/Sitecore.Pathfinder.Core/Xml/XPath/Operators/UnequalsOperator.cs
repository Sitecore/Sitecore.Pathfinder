using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Operators
{
    public class UnequalsOperator : BinaryOperator
    {
        public UnequalsOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left == null && right == null)
            {
                return false;
            }

            if (left == null || right == null)
            {
                return true;
            }

            bool handled;
            var result = EqualsOperator.Compare(left, right, out handled);
            if (handled)
            {
                return !result;
            }

            result = EqualsOperator.Compare(right, left, out handled);
            if (handled)
            {
                return !result;
            }

            throw new XPathException("Cannot compare objects of types " + left.GetType().Name + " and " + right.GetType().Name);
        }
    }
}