// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.XPath
{
    public class Following : Step
    {
        [NotNull]
        private readonly string _name;

        [CanBeNull]
        private readonly Predicate _predicate;

        public Following([NotNull] ElementBase element)
        {
            _name = element.Name;

            var itemElement = element as ItemElement;
            if (itemElement != null)
            {
                _predicate = itemElement.Predicate;
            }
            else
            {
                throw new ArgumentException("Element type expected", nameof(element));
            }
        }

        public override object Evaluate(Query query, object context)
        {
            object result = null;

            var item = context as Item;
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

            foreach (var child in parent.Children)
            {
                if (following)
                {
                    if (_name == "*" || string.Equals(child.ItemName, _name, StringComparison.OrdinalIgnoreCase))
                    {
                        Process(query, child, _predicate, NextStep, ref result);

                        if (Break(query, result))
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
