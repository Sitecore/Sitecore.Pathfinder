// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompiler)), Shared]
    public class ReferenceFieldCompiler : FieldCompilerBase
    {
        [NotNull]
        protected ITraceService Trace { get; }

        [ImportingConstructor]
        public ReferenceFieldCompiler([NotNull] ITraceService trace) : base(Constants.FieldCompilers.Normal)
        {
            Trace = trace;
        }

        public override bool CanCompile(IFieldCompileContext context, Field field) => string.Equals(field.TemplateField.Type, "reference", StringComparison.OrdinalIgnoreCase);

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var value = field.Value.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var item = field.Item.Project.Indexes.FindQualifiedItem<IProjectItem>(value);
            if (item == null)
            {
                Trace.TraceError(Msg.C1045, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), value);
                return value;
            }

            return item.Uri.Guid.Format();
        }
    }
}
