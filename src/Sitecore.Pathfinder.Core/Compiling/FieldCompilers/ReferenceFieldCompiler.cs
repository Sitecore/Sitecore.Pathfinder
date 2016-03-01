// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class ReferenceFieldCompiler : FieldCompilerBase
    {
        public ReferenceFieldCompiler() : base(Constants.FieldCompilers.Normal)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            var type = field.TemplateField.Type;
            return string.Equals(type, "reference", StringComparison.OrdinalIgnoreCase);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var value = field.Value.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var item = field.Item.Project.FindQualifiedItem<IProjectItem>(value);
            if (item == null)
            {
                context.Trace.TraceError(Msg.C1045, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), value);
                return string.Empty;
            }

            return item.Uri.Guid.Format();
        }
    }
}
