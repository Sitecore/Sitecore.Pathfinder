// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    public class JsonLayoutFieldCompiler : FieldCompilerBase
    {
        [ImportingConstructor]
        public JsonLayoutFieldCompiler([NotNull] IFileSystemService fileSystem) : base(Constants.FieldCompilers.Normal)
        {
            FileSystem = fileSystem;
        }

        public override bool IsExclusive { get; } = true;

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            var textNode = TraceHelper.GetTextNode(field.ValueProperty);
            if (!(textNode is JsonTextNode))
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

            var layoutCompileContext = new LayoutCompileContext(context.Trace, field.Item.Project, textSnapshot);

            var compiler = new JsonLayoutCompiler();
            return compiler.Compile(layoutCompileContext, textNode);
        }

        protected class JsonLayoutCompiler : LayoutCompiler
        {
            protected override string GetPlaceholders(LayoutCompileContext context, ITextNode renderingTextNode, IProjectItem projectItem)
            {
                var childTextNode = renderingTextNode;
                /*
                if (childTextNode.ParentNode != null && childTextNode.ParentNode.Key == "Renderings")
                {
                    childTextNode = childTextNode.ChildNodes.FirstOrDefault();
                    if (childTextNode == null)
                    {
                        return string.Empty;
                    }
                }
                */
                return base.GetPlaceholders(context, childTextNode, projectItem);
            }

            protected override void WriteRendering(LayoutCompileContext context, XmlTextWriter output, IEnumerable<Item> renderingItems, ITextNode renderingTextNode, string placeholders)
            {
                var childNode = renderingTextNode.ChildNodes.FirstOrDefault();
                if (childNode != null)
                {
                    base.WriteRendering(context, output, renderingItems, childNode, placeholders);
                }
            }
        }
    }
}
