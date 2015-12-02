// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
{
    public class QueryBuilder
    {
        [NotNull]
        public Opcode Add([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Literal)
            {
                if (right is Literal)
                {
                    return new Literal((left as Literal).Text + (right as Literal).Text);
                }

                if (right is Number)
                {
                    return new Literal((left as Literal).Text + (right as Number).Value);
                }
            }

            if (left is Number)
            {
                if (right is Literal)
                {
                    return new Literal((left as Number).Value + (right as Literal).Text);
                }

                if (right is Number)
                {
                    return new Number((left as Number).Value + (right as Number).Value);
                }
            }

            return new AddOperator(left, right);
        }

        [NotNull]
        public Step Ancestor([NotNull] ElementBase element)
        {
            return new Ancestor(element);
        }

        [NotNull]
        public Step AncestorOrSelf([NotNull] ElementBase element)
        {
            return new AncestorOrSelf(element);
        }

        [NotNull]
        public Opcode And([NotNull] Opcode left, [NotNull] Opcode right)
        {
            return new AndOperator(left, right);
        }

        [NotNull]
        public Opcode BooleanValue(bool value)
        {
            return new BooleanValue(value);
        }

        [NotNull]
        public Step Children([NotNull] ElementBase element)
        {
            return new Children(element);
        }

        [NotNull]
        public Step Descendants([NotNull] ElementBase element)
        {
            return new Descendant(element);
        }

        [NotNull]
        public Step DescendantsOrSelf([NotNull] ElementBase element)
        {
            return new DescendantOrSelf(element);
        }

        [NotNull]
        public Opcode Divide([NotNull] Opcode left, [NotNull] Opcode right)
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
        public Opcode Equals([NotNull] Opcode left, [NotNull] Opcode right)
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
        public FieldElement FieldElement([NotNull] string name)
        {
            return new FieldElement(name);
        }

        [NotNull]
        public Step Following([NotNull] ElementBase element)
        {
            return new Following(element);
        }

        [NotNull]
        public Function Function([NotNull] string name)
        {
            return new Function(name);
        }

        [NotNull]
        public Opcode Greater([NotNull] Opcode left, [NotNull] Opcode right)
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
        public Opcode GreaterOrEquals([NotNull] Opcode left, [NotNull] Opcode right)
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
        public ItemElement ItemElement([NotNull] string name, [CanBeNull] Predicate predicate)
        {
            return new ItemElement(name, predicate);
        }

        [NotNull]
        public Opcode Literal([NotNull] string text)
        {
            return new Literal(text);
        }

        [NotNull]
        public Opcode Minus([NotNull] Opcode left, [NotNull] Opcode right)
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
        public Opcode Modulus([NotNull] Opcode left, [NotNull] Opcode right)
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
        public Opcode Multiply([NotNull] Opcode left, [NotNull] Opcode right)
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
        public Opcode Number(int value)
        {
            return new Number(value);
        }

        [NotNull]
        public Opcode Or([NotNull] Opcode left, [NotNull] Opcode right)
        {
            return new OrOperator(left, right);
        }

        [NotNull]
        public Step Parent()
        {
            return new Parent();
        }

        [NotNull]
        public Step Preceding([NotNull] ElementBase element)
        {
            return new Preceding(element);
        }

        [NotNull]
        public Predicate Predicate([NotNull] Opcode expression)
        {
            return new Predicate(expression);
        }

        [NotNull]
        public Parameter QueryParameter([NotNull] string name)
        {
            return new Parameter(name);
        }

        [NotNull]
        public Step Root()
        {
            return new Root();
        }

        [NotNull]
        public Step Self([NotNull] Predicate predicate)
        {
            return new Self(predicate);
        }

        [NotNull]
        public Opcode Smaller([NotNull] Opcode left, [NotNull] Opcode right)
        {
            if (left is Number)
            {
                if (right is Number)
                {
                    return new BooleanValue((left as Number).Value < (right as Number).Value);
                }
            }

            if (left is Literal)
            {
                if (right is Literal)
                {
                    return new BooleanValue((left as Literal).Text.CompareTo((right as Literal).Text) < 0);
                }
            }

            return new SmallerOperator(left, right);
        }

        [NotNull]
        public Opcode SmallerOrEquals([NotNull] Opcode left, [NotNull] Opcode right)
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
        public Opcode Unequals([NotNull] Opcode left, [NotNull] Opcode right)
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
        public Opcode Xor([NotNull] Opcode left, [NotNull] Opcode right)
        {
            return new XorOperator(left, right);
        }
    }
}
