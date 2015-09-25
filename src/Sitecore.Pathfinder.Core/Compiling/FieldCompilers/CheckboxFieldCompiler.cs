// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompiler))]
    public class CheckboxFieldCompiler : FieldCompilerBase
    {
        public CheckboxFieldCompiler() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return string.Compare(field.TemplateField.Type, "checkbox", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var value = field.Value.Trim();

            if (string.Compare(value, "True", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(value, "False", StringComparison.OrdinalIgnoreCase) != 0)
            {
                context.Trace.TraceError("Checkbox field value must be 'true' or 'false'.", value);
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
