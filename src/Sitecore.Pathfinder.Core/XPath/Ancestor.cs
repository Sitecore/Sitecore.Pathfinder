// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.XPath
{
    public class Ancestor : Step
    {
        public Ancestor([NotNull] ElementBase element)
        {
            Name = element.Name;

            var itemElement = element as ItemElement;
            if (itemElement != null)
            {
                Predicate = itemElement.Predicate;
            }
            else
            {
                throw new ArgumentException("Node type expected", nameof(element));
            }
        }

        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public Predicate Predicate { get; }

        public override object Evaluate(Query query, object context)
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
                    Process(query, parent, Predicate, NextStep, ref result);

                    if (Break(query, result))
                    {
                        break;
                    }
                }

                parent = parent.GetParent();
            }

            /*
              if (!Break(query, result) && m_name == "*") {
                parent = new QueryContext(contextNode.Queryable);
                Process(query, parent, m_predicate, NextStep, ref result);
              }
            */

            return result;
        }

        [CanBeNull]
        protected virtual Item GetContextNode([CanBeNull] object context)
        {
            var item = context as Item;
            if (item == null)
            {
                return null;
            }

            return item.GetParent();
        }
    }
}
