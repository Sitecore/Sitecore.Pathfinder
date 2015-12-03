// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Operators
{
    public class AddOperator : BinaryOperator
    {
        public AddOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        [CanBeNull]
        public object Evaluate([CanBeNull] object left, [CanBeNull] object right, out bool handled)
        {
            if (left == null)
            {
                handled = true;
                return right;
            }

            if (right == null)
            {
                handled = true;
                return left;
            }

            var leftString = left as string;
            var rightString = right as string;
            if (leftString != null || rightString != null)
            {
                handled = true;
                return left.ToString() + right;
            }

            if (left is int && right is int)
            {
                handled = true;
                return (int)left + (int)right;
            }

            var leftArray = left as object[];
            var rightArray = right as object[];

            if (leftArray != null)
            {
                if (rightArray != null)
                {
                    handled = true;
                    var result1 = new object[leftArray.Length + rightArray.Length];
                    leftArray.CopyTo(result1, 0);
                    rightArray.CopyTo(result1, leftArray.Length);
                    return result1;
                }

                handled = true;
                var result = new object[leftArray.Length + 1];
                leftArray.CopyTo(result, 0);
                result[result.Length - 1] = right;
                return result;
            }

            if (rightArray != null)
            {
                handled = true;
                var result3 = new object[rightArray.Length + 1];
                rightArray.CopyTo(result3, 1);
                result3[0] = left;
                return result3;
            }

            handled = true;
            var result2 = new object[2];
            result2[0] = left;
            result2[1] = right;
            return result2;
        }

        public override object EvaluateOperands(object left, object right)
        {
            bool handled;

            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            var result = Evaluate(left, right, out handled);
            if (handled)
            {
                return result;
            }

            result = Evaluate(right, left, out handled);
            if (handled)
            {
                return result;
            }

            throw new XPathException("Type mismatch");
        }
    }
}
