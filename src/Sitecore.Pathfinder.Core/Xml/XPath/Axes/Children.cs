// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Axes
{
    public class Children : StepBase
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
                throw new ArgumentException(Texts.Node_or_Field_type_expected, nameof(element));
            }
        }

        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public Predicate Predicate { get; }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            var item = context as IXPathItem;
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
                        return item.ItemId;
                    case "@templatename":
                        return item.TemplateName;
                    case "@templatekey":
                        return item.TemplateName.ToLowerInvariant();
                    case "@templateid":
                        return item.TemplateId;
                    case "@path":
                        return item.ItemPath;
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

                Process(xpath, child, Predicate, NextStep, ref result);

                if (Break(xpath, result))
                {
                    break;
                }
            }

            return result;
        }
    }
}
