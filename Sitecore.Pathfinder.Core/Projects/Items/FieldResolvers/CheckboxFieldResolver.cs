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

        public override bool CanResolve(Field field)
        {
            return string.Compare(field.TemplateField.TypeProperty, "checkbox", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override string Resolve(Field field)
        {
            var value = field.Value.Trim();

            if (string.Compare(value, "True", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(value, "False", StringComparison.OrdinalIgnoreCase) != 0)
            {
                field.WriteDiagnostic(Severity.Error, "Checkbox field value must be 'true' or 'false'.", value);
            }

            if (string.Compare(value, "true", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return "1";
            }

            if (string.Compare(value, "1", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return "1";
            }

            return string.Empty;
        }
    }
}
