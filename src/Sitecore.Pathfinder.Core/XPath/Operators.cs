// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
{
    public class AddOperator : BinaryOperator
    {
        public AddOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        [CanBeNull]
        public object Evaluate([CanBeNull] object left, [CanBeNull] object right, out bool handled)
        {
            handled = false;

            if (left is string && right is string)
            {
                handled = true;
                return (left as string) + (right as string);
            }

            if (left is int && right is int)
            {
                handled = true;

                return (int)left + (int)right;
            }

            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
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

            throw new QueryException("Type mismatch");
        }
    }

    public class AndOperator : BinaryOperator
    {
        public AndOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is bool && right is bool)
            {
                return (bool)left && (bool)right;
            }

            if (left is int && right is int)
            {
                return ((int)left) & ((int)right);
            }

            throw new QueryException("Type mismatch");
        }
    }

    public abstract class BinaryOperator : Opcode
    {
        protected BinaryOperator([NotNull] Opcode left, [NotNull] Opcode right)
        {
            Left = left;
            Right = right;
        }

        [NotNull]
        public Opcode Left { get; }

        [NotNull]
        public Opcode Right { get; }

        public override object Evaluate(Query query, object context)
        {
            var result1 = Left.Evaluate(query, context);
            var result2 = Right.Evaluate(query, context);

            return EvaluateOperands(result1, result2);
        }

        [CanBeNull]
        public abstract object EvaluateOperands([CanBeNull] object left, [CanBeNull] object right);
    }

    public class DivideOperator : BinaryOperator
    {
        public DivideOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is int && right is int)
            {
                return ((int)left) / ((int)right);
            }

            throw new QueryException("Type mismatch");
        }
    }

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

            throw new QueryException("Cannot compare objects of types " + left.GetType().Name + " and " + right.GetType().Name);
        }
    }

    public class GreaterOperator : BinaryOperator
    {
        public GreaterOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is int && right is int)
            {
                return ((int)left) > ((int)right);
            }

            return left.ToString().Equals(right.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }

    public class GreaterOrEqualsOperator : BinaryOperator
    {
        public GreaterOrEqualsOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is int && right is int)
            {
                return ((int)left) >= ((int)right);
            }
            return left.ToString().CompareTo(right.ToString()) >= 0;
        }
    }

    public class MinusOperator : BinaryOperator
    {
        public MinusOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is int && right is int)
            {
                return ((int)left) - ((int)right);
            }

            throw new QueryException("Type mismatch");
        }
    }

    public class ModulusOperator : BinaryOperator
    {
        public ModulusOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is int && right is int)
            {
                return ((int)left) % ((int)right);
            }

            throw new QueryException("Type mismatch");
        }
    }

    public class MultiplyOperator : BinaryOperator
    {
        public MultiplyOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is int && right is int)
            {
                return ((int)left) * ((int)right);
            }

            throw new QueryException("Type mismatch");
        }
    }

    public class OrOperator : BinaryOperator
    {
        public OrOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is bool && right is bool)
            {
                return ((bool)left) || ((bool)right);
            }

            if (left is int && right is int)
            {
                return ((int)left) | ((int)right);
            }

            throw new QueryException("Type mismatch");
        }
    }

    public class SmallerOperator : BinaryOperator
    {
        public SmallerOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is int && right is int)
            {
                return ((int)left) < ((int)right);
            }
            return left.ToString().CompareTo(right.ToString()) < 0;
        }
    }

    public class SmallerOrEqualsOperator : BinaryOperator
    {
        public SmallerOrEqualsOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is int && right is int)
            {
                return ((int)left) <= ((int)right);
            }

            return left.ToString().CompareTo(right.ToString()) <= 0;
        }
    }

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

            throw new QueryException("Cannot compare objects of types " + left.GetType().Name + " and " + right.GetType().Name);
        }
    }

    public class XorOperator : BinaryOperator
    {
        public XorOperator([NotNull] Opcode left, [NotNull] Opcode right) : base(left, right)
        {
        }

        public override object EvaluateOperands(object left, object right)
        {
            if (left is bool && right is bool)
            {
                return ((bool)left) ^ ((bool)right);
            }

            if (left is int && right is int)
            {
                return ((int)left) ^ ((int)right);
            }

            throw new QueryException("Type mismatch");
        }
    }
}
