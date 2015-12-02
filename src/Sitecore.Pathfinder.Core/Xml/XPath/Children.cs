// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class Children : Step
    {
        private readonly bool _field;

        public Children([NotNull] ElementBase element)
        {
            Name = element.Name;

            var itemElement = element as ItemElement;
            if (itemElement != null)
            {
                Predicate = itemElement.Predicate;
            }
            else if (element is FieldElement)
            {
                _field = true;
            }
            else
            {
                throw new ArgumentException("Node or Field type expected", nameof(element));
            }
        }

        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public Predicate Predicate { get; }

        public override object Evaluate(Query query, object context)
        {
            var item = context as Item;
            if (item == null)
            {
                return null;
            }

            if (_field)
            {
                switch (Name.ToLowerInvariant())
                {
                    case "@name":
                        return item.ItemName;
                    case "@key":
                        return item.ItemName.ToLowerInvariant();
                    case "@id":
                        return item.Uri.Guid.Format();
                    case "@templatename":
                        return item.TemplateName;
                    case "@templatekey":
                        return item.TemplateName.ToLowerInvariant();
                    case "@templateid":
                        return item.Template.Uri.Guid.Format();
                    default:
                        return item[Name];
                }
            }

            object result = null;

            foreach (var child in item.GetChildren())
            {
                if (Name != "*" && !string.Equals(Name, child.ItemName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Process(query, child, Predicate, NextStep, ref result);

                if (Break(query, result))
                {
                    break;
                }
            }

            return result;
        }
    }
}
