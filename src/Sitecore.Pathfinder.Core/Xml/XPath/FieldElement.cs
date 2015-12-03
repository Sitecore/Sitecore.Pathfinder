// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

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

            if (!Name.StartsWith("@", StringComparison.InvariantCulture))
            {
                if (item != null)
                {
                    return item[Name];
                }
            }

            var name = Name.Mid(1);

            switch (name.ToLowerInvariant())
            {
                case "name":
                    return item?.ItemName;

                case "key":
                    return item?.ItemName.ToLowerInvariant();

                case "id":
                    return item?.ItemId;

                case "templateid":
                    return item?.TemplateId;

                case "templatename":
                    return item?.TemplateName;

                case "templatekey":
                    return item?.TemplateName.ToLowerInvariant();
            }

            if (item != null)
            {
                return item[Name];
            }

            return null;
        }
    }
}
