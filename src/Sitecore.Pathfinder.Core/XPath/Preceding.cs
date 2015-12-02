// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.XPath
{
    public class Preceding : Step
    {
        private readonly string _name;

        private readonly Predicate _predicate;

        public Preceding([NotNull] ElementBase element)
        {
            _name = element.Name;

            if (element is ItemElement)
            {
                _predicate = (element as ItemElement).Predicate;
            }
            else
            {
                throw new ArgumentException("Node type expected", nameof(element));
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

            foreach (Item child in parent.GetChildren())
            {
                if (child == item)
                {
                    break;
                }

                if (_name == "*" || string.Equals(child.ItemName, _name, StringComparison.OrdinalIgnoreCase))
                {
                    Process(query, child, _predicate, NextStep, ref result);

                    if (Break(query, result))
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}
