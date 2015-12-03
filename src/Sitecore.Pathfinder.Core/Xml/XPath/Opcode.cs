// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public abstract class Opcode
    {
        [CanBeNull]
        public virtual object Evaluate([NotNull] XPathExpression xpath, [CanBeNull] object context)
        {
            throw new XPathException("Cannot evaluate this element: " + GetType().Name);
        }

        protected virtual void Process([NotNull] XPathExpression xpath, [CanBeNull] object context, [CanBeNull] Predicate predicate, [CanBeNull] Opcode nextStep, [CanBeNull] ref object result)
        {
            if (predicate != null)
            {
                if (!predicate.Test(xpath,  context))
                {
                    return;
                }
            }

            if (nextStep == null && xpath.PredicateCounter == 0)
            {
                result = ProcessResults(result, context);
            }
            else
            {
                var nextResult = ProcessNextStep(xpath, context, nextStep);
                result = ProcessResults(result, nextResult);
            }
        }

        [CanBeNull]
        protected virtual object ProcessNextStep([NotNull] XPathExpression xpath, [CanBeNull] object context, [CanBeNull] Opcode nextStep)
        {
            if (nextStep != null)
            {
                return nextStep.Evaluate(xpath, context);
            }

            return context;
        }

        [CanBeNull]
        protected virtual object ProcessResults([CanBeNull] object left, [CanBeNull] object right)
        {
            bool handled;

            var result = ProcessResults(left, right, out handled);
            if (handled)
            {
                return result;
            }

            throw new XPathException("Type mismatch");
        }

        [CanBeNull]
        protected virtual object ProcessResults([CanBeNull] object left, [CanBeNull] object right, out bool handled)
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

            // todo: throw if incompatible types

            handled = true;
            var result2 = new object[2];
            result2[0] = left;
            result2[1] = right;
            return result2;
        }
    }
}
