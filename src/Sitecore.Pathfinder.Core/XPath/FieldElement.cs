// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.XPath
{
    public class FieldElement : ElementBase
    {
        public FieldElement(string name) : base(name)
        {
        }

        public override object Evaluate(Query query, object context)
        {
            var item = context as Item;
            var projectItem = context as IProjectItem;

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
                    return projectItem?.ShortName;

                case "key":
                    return projectItem?.ShortName.ToLowerInvariant();

                case "id":
                    return projectItem?.Uri.Guid.Format();

                case "templateid":
                    return item?.Template.Uri.Guid.Format();

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
