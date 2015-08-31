// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class CheckboxFieldResolver : FieldResolverBase
    {
        public CheckboxFieldResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(ITraceService trace, IProject project, Field field)
        {
            return string.Compare(field.TemplateField.Type, "checkbox", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override string Resolve(ITraceService trace, IProject project, Field field)
        {
            if (string.Compare(field.Value.Value, "true", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return "1";
            }

            if (string.Compare(field.Value.Value, "1", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return "1";
            }

            return string.Empty;
        }
    }
}
