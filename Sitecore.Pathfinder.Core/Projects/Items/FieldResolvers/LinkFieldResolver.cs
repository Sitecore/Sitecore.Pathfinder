// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class LinkFieldResolver : FieldResolverBase
    {
        public LinkFieldResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(Field field)
        {
            return string.Compare(field.TemplateField.Type, "general link", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(field.TemplateField.Type, "link", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override string Resolve(ITraceService trace, Field field)
        {
            var qualifiedName = field.Value.Trim();
            if (string.IsNullOrEmpty(qualifiedName))
            {
                return string.Empty;
            }

            var item = field.Item.Project.FindQualifiedItem(qualifiedName);
            if (item == null)
            {
                trace.TraceError("Link field reference not found", qualifiedName);
                return string.Empty;
            }

            return $"<link text=\"\" linktype=\"internal\" url=\"\" anchor=\"\" title=\"\" class=\"\" target=\"\" querystring=\"\" id=\"{item.Uri.Guid.Format()}\" />";
        }
    }
}
