// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public class YamlLayoutFieldCompiler : FieldCompilerBase
    {
        [ImportingConstructor]
        public YamlLayoutFieldCompiler([NotNull] IFileSystemService fileSystem) : base(Constants.FieldCompilers.Normal)
        {
            FileSystem = fileSystem;
        }

        public override bool IsExclusive { get; } = true;

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            var textNode = TraceHelper.GetTextNode(field.ValueProperty);
            if (!(textNode is YamlTextNode))
            {
                return false;
            }

            return string.Equals(field.TemplateField.Type, "layout", StringComparison.OrdinalIgnoreCase) || field.ValueHint.Contains("Layout");
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

            var layoutResolveContext = new LayoutCompileContext(context.Trace, field.Item.Project, field.Database, textSnapshot);

            var resolver = new LayoutCompiler(FileSystem);

            return resolver.Compile(layoutResolveContext, textNode);
        }
    }
}
