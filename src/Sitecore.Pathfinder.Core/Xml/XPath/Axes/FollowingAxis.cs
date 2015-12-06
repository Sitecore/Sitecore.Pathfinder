// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Axes
{
    public class FollowingAxis : StepBase
    {
        [NotNull]
        private readonly string _name;

        [CanBeNull]
        private readonly Predicate _predicate;

        public FollowingAxis([NotNull] ElementBase element)
        {
            _name = element.Name;

            var itemElement = element as ItemElement;
            if (itemElement == null)
            {
                throw new ArgumentException(Texts.Element_type_expected, nameof(element));
            }

            _predicate = itemElement.Predicate;
        }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            object result = null;

            var item = context as IXPathItem;
            if (item == null)
            {
                return null;
            }

            var parent = item.GetParent();
            if (parent == null)
            {
                return null;
            }

            var following = false;

            foreach (var child in parent.GetChildren())
            {
                if (following)
                {
                    if (_name == "*" || string.Equals(child.ItemName, _name, StringComparison.OrdinalIgnoreCase))
                    {
                        Process(xpath, child, _predicate, NextStep, ref result);

                        if (Break(xpath, result))
                        {
                            break;
                        }
                    }
                }

                if (child == context)
                {
                    following = true;
                }
            }

            return result;
        }
    }
}
