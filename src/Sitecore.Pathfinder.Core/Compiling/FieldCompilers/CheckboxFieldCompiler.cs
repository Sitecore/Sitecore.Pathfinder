// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class CheckboxFieldCompiler : FieldCompilerBase
    {
        public CheckboxFieldCompiler() : base(Constants.FieldCompilers.Normal)
        {
        }

        public override bool IsExclusive
        {
            get { return true; }
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return string.Equals(field.TemplateField.Type, "checkbox", StringComparison.OrdinalIgnoreCase);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var value = field.Value.Trim();

            if (!string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) && !string.Equals(value, "false", StringComparison.OrdinalIgnoreCase) && 
                field.Item.Snapshots.All(s => s.Capabilities.HasFlag(SnapshotCapabilities.SupportsTrueAndFalseForBooleanFields)))
            {
                context.Trace.TraceError(Msg.C1043, Texts.Checkbox_field_value_must_be__true__or__false__, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), value);
            }

            if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
            {
                return "1";
            }

            if (value == "1")
            {
                return "1";
            }

            return string.Empty;
        }
    }
}
