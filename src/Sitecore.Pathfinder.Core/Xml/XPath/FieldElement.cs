// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class FieldElement : ElementBase
    {
        public FieldElement([NotNull] string name) : base(name)
        {
        }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            var item = context as IXPathItem;
            if (item == null)
            {
                return null;
            }

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
    }
}
