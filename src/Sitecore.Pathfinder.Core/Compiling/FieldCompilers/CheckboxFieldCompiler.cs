// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class CheckboxFieldCompiler : FieldCompilerBase
    {
        public CheckboxFieldCompiler() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return string.Equals(field.TemplateField.Type, "checkbox", StringComparison.OrdinalIgnoreCase);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var value = field.Value.Trim();

            if (!string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) && string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) != 0)
            {
                context.Trace.TraceError(Texts.Checkbox_field_value_must_be__true__or__false__, value);
            }

            if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
            {
                return "1";
            }

            if (string.Equals(value, "1", StringComparison.OrdinalIgnoreCase))
            {
                return "1";
            }

            return string.Empty;
        }
    }
}
