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

        public override bool CanResolve(ITraceService trace, IProject project, Field field)
        {
            return string.Compare(field.TemplateField.Type, "general link", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(field.TemplateField.Type, "link", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override string Resolve(ITraceService trace, IProject project, Field field)
        {
            var qualifiedName = field.Value.Value;
            if (string.IsNullOrEmpty(qualifiedName))
            {
                return string.Empty;
            }

            var item = project.FindQualifiedItem(qualifiedName);
            if (item == null)
            {
                trace.Writeline("Link field reference not found", qualifiedName);
            }

            return $"<link text=\"\" linktype=\"internal\" url=\"\" anchor=\"\" title=\"\" class=\"\" target=\"\" querystring=\"\" id=\"{item.Guid.Format()}\" />";
        }
    }
}
