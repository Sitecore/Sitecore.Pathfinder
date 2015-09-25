// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Json;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers.Layouts
{
    [Export(typeof(IFieldCompiler))]
    public class JsonLayoutFieldCompiler : FieldCompilerBase
    {
        [ImportingConstructor]
        public JsonLayoutFieldCompiler([NotNull] IFileSystemService fileSystem) : base(Constants.FieldResolvers.Normal)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            var textNode = TraceHelper.GetTextNode(field.ValueProperty);
            if (!(textNode is JsonTextNode))
            {
                return false;
            }

            return string.Compare(field.TemplateField.Type, "layout", StringComparison.OrdinalIgnoreCase) == 0 || field.ValueHint.Contains("Layout");
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

            var layoutResolveContext = new LayoutCompileContext(context, FileSystem, field, textSnapshot);

            var resolver = new JsonLayoutCompiler();

            return resolver.Compile(layoutResolveContext, textNode);
        }
    }
}
