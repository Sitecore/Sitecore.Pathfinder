// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Axes
{
    public class AncestorAxis : StepBase
    {
        public AncestorAxis([NotNull] ElementBase element)
        {
            Name = element.Name;

            var itemElement = element as ItemElement;
            if (itemElement == null)
            {
                throw new ArgumentException(Texts.Node_type_expected, nameof(element));
            }

            Predicate = itemElement.Predicate;
        }

        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public Predicate Predicate { get; }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            object result = null;

            var parent = GetContextNode(context);
            if (parent == null)
            {
                return null;
            }

            while (parent != null)
            {
                if (Name == "*" || string.Equals(parent.ItemName, Name, StringComparison.OrdinalIgnoreCase))
                {
                    Process(xpath, parent, Predicate, NextStep, ref result);

                    if (Break(xpath, result))
                    {
                        break;
                    }
                }

                parent = parent.GetParent();
            }

            return result;
        }

        [CanBeNull]
        protected virtual IXPathItem GetContextNode([CanBeNull] object context)
        {
            var item = context as IXPathItem;
            if (item == null)
            {
                return null;
            }

            return item.GetParent();
        }
    }
}
