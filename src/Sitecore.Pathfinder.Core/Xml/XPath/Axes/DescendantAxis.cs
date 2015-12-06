// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Axes
{
    public class DescendantAxis : StepBase
    {
        public DescendantAxis([NotNull] ElementBase element)
        {
            Name = element.Name;

            var itemElement = element as ItemElement;
            if (itemElement != null)
            {
                Predicate = itemElement.Predicate;
            }
            else if (!(element is FieldElement))
            {
                throw new ArgumentException(Texts.Node_or_Field_type_expected, nameof(element));
            }
        }

        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public Predicate Predicate { get; }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            object result = null;

            Iterate(xpath, context, ref result);

            return result;
        }

        protected void Iterate([NotNull] XPathExpression xpath, [CanBeNull] object context, [CanBeNull] ref object result)
        {
            var item = context as IXPathItem;
            if (item == null)
            {
                return;
            }

            foreach (var child in item.GetChildren())
            {
                TestNode(xpath, child, ref result);

                if (Break(xpath, result))
                {
                    break;
                }

                Iterate(xpath, child, ref result);

                if (Break(xpath, result))
                {
                    break;
                }
            }
        }

        protected void TestNode([NotNull] XPathExpression xpath, [CanBeNull] object context, [CanBeNull] ref object result)
        {
            if (Name == "*" || string.Equals((context as IXPathItem)?.ItemName, Name, StringComparison.OrdinalIgnoreCase))
            {
                Process(xpath, context, Predicate, NextStep, ref result);
            }
        }
    }
}
