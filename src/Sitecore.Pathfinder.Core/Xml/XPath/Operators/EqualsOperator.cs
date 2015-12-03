using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Operators
{
    public class EqualsOperator : BinaryOperator
    {
        public EqualsOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public static bool Compare([NotNull] object left, [NotNull] object right, out bool handled)
        {
            var comparable = left as IComparable;
            if (comparable != null)
            {
                handled = true;
                return comparable.CompareTo(right) == 0;
            }

            handled = true;
            var leftArray = left as object[];
            var rightArray = right as object[];

            if (leftArray == null || rightArray == null)
            {
                return false;
            }

            if (leftArray.Length != rightArray.Length)
            {
                return false;
            }

            for (var index = 0; index < leftArray.Length; index++)
            {
                var leftComparable = leftArray[index] as IComparable;
                if (leftComparable != null)
                {
                    if (leftComparable.CompareTo(right) != 0)
                    {
                        return false;
                    }
                }

                if (leftArray[index] != rightArray[index])
                {
                    return false;
                }
            }

            return true;
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left == null && right == null)
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            bool handled;
            var result = Compare(left, right, out handled);

            if (handled)
            {
                return result;
            }

            result = Compare(right, left, out handled);
            if (handled)
            {
                return result;
            }

            throw new XPathException("Cannot compare objects of types " + left.GetType().Name + " and " + right.GetType().Name);
        }
    }
}