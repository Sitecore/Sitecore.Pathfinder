// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    [Export(typeof(IFieldCompiler)), Shared]
    public class JsonLayoutFieldCompiler : FieldCompilerBase
    {
        [ImportingConstructor]
        public JsonLayoutFieldCompiler([NotNull] ITraceService trace, [NotNull] IFileSystemService fileSystem) : base(Constants.FieldCompilers.Normal)
        {
            Trace = trace;
            FileSystem = fileSystem;
        }

        public override bool IsExclusive { get; } = true;

        [NotNull]
        protected ITraceService Trace { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            var textNode = TraceHelper.GetTextNode(field.ValueProperty);
            if (!(textNode is JsonTextNode))
            {
                return false;
            }

            return string.Equals(field.TemplateField.Type, "layout", StringComparison.OrdinalIgnoreCase);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var textNode = TraceHelper.GetTextNode(field.ValueProperty);
            if (textNode == TextNode.Empty)
            {
                return field.Value;
            }

            var textSnapshot = textNode.Snapshot as ITextSnapshot;
            if (textSnapshot == null)
            {
                return field.Value;
            }

            var layoutCompileContext = new LayoutCompileContext(field.Item.Project, field.Database, textSnapshot);

            var compiler = new LayoutCompiler(Trace, FileSystem);
            return compiler.Compile(layoutCompileContext, textNode);
        }
    }
}
