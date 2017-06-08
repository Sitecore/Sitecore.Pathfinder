// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Configuration;
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
        public JsonLayoutFieldCompiler([NotNull] IFactory factory) : base(Constants.FieldCompilers.Normal)
        {
            Factory = factory;
        }

        public override bool IsExclusive { get; } = true;

        [NotNull]
        protected IFactory Factory { get; }

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

            var layoutCompileContext = Factory.LayoutCompileContext(field.Item.Project, field.Database, textSnapshot);
            var layoutCompiler = Factory.LayoutCompiler();

            return layoutCompiler.Compile(layoutCompileContext, textNode);
        }
    }
}
