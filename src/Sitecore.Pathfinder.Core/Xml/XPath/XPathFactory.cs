// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Xml.XPath.Axes;
using Sitecore.Pathfinder.Xml.XPath.Functions;
using Sitecore.Pathfinder.Xml.XPath.Operators;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class XPathFactory
    {
        [NotNull]
        public Opcode Add([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Literal)
            {
                if (right is Literal)
                {
                    return new Literal(((Literal)left).Text + (right as Literal).Text);
                }

                if (right is Number)
                {
                    return new Literal(((Literal)left).Text + (right as Number).Value);
                }
            }

            if (left is Number)
            {
                if (right is Literal)
                {
                    return new Literal(((Number)left).Value + (right as Literal).Text);
                }

                if (right is Number)
                {
                    return new Number(((Number)left).Value + (right as Number).Value);
                }
            }

            return new AddOperator(left, right);
        }

        [NotNull]
        public virtual StepBase Ancestor([NotNull] ElementBase element) => new AncestorAxis(element);

        [NotNull]
        public virtual StepBase AncestorOrSelf([NotNull] ElementBase element) => new AncestorOrSelfAxis(element);

        [NotNull]
        public virtual Opcode And([NotNull] Opcode left, [NotNull] Opcode right) => new AndOperator(left, right);

        [NotNull]
        public virtual Opcode BooleanValue(bool value) => new BooleanValue(value);

        [NotNull]
        public virtual StepBase Children([NotNull] ElementBase element) => new Children(element);

        [NotNull]
        public virtual StepBase Descendants([NotNull] ElementBase element) => new DescendantAxis(element);

        [NotNull]
        public virtual StepBase DescendantsOrSelf([NotNull] ElementBase element) => new DescendantOrSelfAxis(element);

        [NotNull]
        public virtual Opcode Divide([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Number)
            {
                if (right is Number)
                {
                    return new Number((left as Number).Value / (right as Number).Value);
                }
            }

            return new DivideOperator(left, right);
        }

        [NotNull]
        public virtual Opcode Equals([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Literal)
            {
                if (right is Literal)
                {
                    return new BooleanValue((left as Literal).Text == (right as Literal).Text);
                }

                if (right is Number)
                {
                    return new BooleanValue((left as Literal).Text == (right as Number).Value.ToString());
                }
            }

            if (left is Number)
            {
                if (right is Literal)
                {
                    return new BooleanValue((left as Number).Value.ToString() == (right as Literal).Text);
                }

                if (right is Number)
                {
                    return new BooleanValue((left as Number).Value == (right as Number).Value);
                }
            }

            return new EqualsOperator(left, right);
        }

        [NotNull]
        public virtual FieldElement FieldElement([NotNull] string name) => new FieldElement(name);

        [NotNull]
        public virtual StepBase Following([NotNull] ElementBase element) => new FollowingAxis(element);

        [NotNull]
        public virtual Function Function([NotNull] string name) => new Function(name);

        [NotNull]
        public virtual Opcode Greater([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Number)
            {
                if (right is Number)
                {
                    return new BooleanValue((left as Number).Value > (right as Number).Value);
                }
            }

            if (left is Literal)
            {
                if (right is Literal)
                {
                    return new BooleanValue((left as Literal).Text.CompareTo((right as Literal).Text) > 0);
                }
            }

            return new GreaterOperator(left, right);
        }

        [NotNull]
        public virtual Opcode GreaterOrEquals([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Number)
            {
                if (right is Number)
                {
                    return new BooleanValue((left as Number).Value >= (right as Number).Value);
                }
            }

            if (left is Literal)
            {
                if (right is Literal)
                {
                    return new BooleanValue((left as Literal).Text.CompareTo((right as Literal).Text) >= 0);
                }
            }

            return new GreaterOrEqualsOperator(left, right);
        }

        [NotNull]
        public virtual ItemElement ItemElement([NotNull] string name, [CanBeNull] Predicate predicate) => new ItemElement(name, predicate);

        [NotNull]
        public virtual Opcode Literal([NotNull] string text) => new Literal(text);

        [NotNull]
        public virtual Opcode Minus([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Number)
            {
                if (right is Number)
                {
                    return new Number((left as Number).Value - (right as Number).Value);
                }
            }

            return new MinusOperator(left, right);
        }

        [NotNull]
        public virtual Opcode Modulus([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Number)
            {
                if (right is Number)
                {
                    return new Number((left as Number).Value % (right as Number).Value);
                }
            }

            return new ModulusOperator(left, right);
        }

        [NotNull]
        public virtual Opcode Multiply([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Number)
            {
                if (right is Number)
                {
                    return new Number((left as Number).Value * (right as Number).Value);
                }
            }

            return new MultiplyOperator(left, right);
        }

        [NotNull]
        public virtual Opcode Number(int value) => new Number(value);

        [NotNull]
        public virtual Opcode Or([NotNull] Opcode left, [NotNull] Opcode right) => new OrOperator(left, right);

        [NotNull]
        public virtual StepBase Parent() => new Parent();

        [NotNull]
        public virtual StepBase Preceding([NotNull] ElementBase element) => new PrecedingAxis(element);

        [NotNull]
        public virtual Predicate Predicate([NotNull] Opcode expression) => new Predicate(expression);

        [NotNull]
        public virtual Parameter QueryParameter([NotNull] string name) => new Parameter(name);

        [NotNull]
        public virtual StepBase Root() => new Root();

        [NotNull]
        public virtual StepBase Self([CanBeNull] Predicate predicate) => new Self(predicate);

        [NotNull]
        public virtual Opcode Smaller([NotNull] Opcode left, [NotNull] Opcode right)
        {
            var leftNumber = left as Number;
            if (leftNumber != null)
            {
                var rightNumber = right as Number;
                if (rightNumber != null)
                {
                    return new BooleanValue(leftNumber.Value < rightNumber.Value);
                }
            }

            var leftLiteral = left as Literal;
            if (leftLiteral != null)
            {
                var rightLiteral = right as Literal;
                if (rightLiteral != null)
                {
                    return new BooleanValue(leftLiteral.Text.CompareTo(rightLiteral.Text) < 0);
                }
            }

            return new SmallerOperator(left, right);
        }

        [NotNull]
        public virtual Opcode SmallerOrEquals([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Number)
            {
                if (right is Number)
                {
                    return new BooleanValue((left as Number).Value <= (right as Number).Value);
                }
            }

            if (left is Literal)
            {
                if (right is Literal)
                {
                    return new BooleanValue((left as Literal).Text.CompareTo((right as Literal).Text) <= 0);
                }
            }

            return new SmallerOrEqualsOperator(left, right);
        }

        [NotNull]
        public virtual Opcode Unequals([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Literal)
            {
                if (right is Literal)
                {
                    return new BooleanValue((left as Literal).Text != (right as Literal).Text);
                }

                if (right is Number)
                {
                    return new BooleanValue((left as Literal).Text != (right as Number).Value.ToString());
                }
            }

            if (left is Number)
            {
                if (right is Literal)
                {
                    return new BooleanValue((left as Number).Value.ToString() != (right as Literal).Text);
                }

                if (right is Number)
                {
                    return new BooleanValue((left as Number).Value != (right as Number).Value);
                }
            }

            return new UnequalsOperator(left, right);
        }

        [NotNull]
        public virtual Opcode Xor([NotNull] Opcode left, [NotNull] Opcode right) => new XorOperator(left, right);
    }
}
