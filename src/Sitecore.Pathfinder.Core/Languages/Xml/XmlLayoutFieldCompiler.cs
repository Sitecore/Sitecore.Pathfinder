// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(IFieldCompiler)), Shared]
    public class XmlLayoutFieldCompiler : FieldCompilerBase
    {
        [ImportingConstructor]
        public XmlLayoutFieldCompiler([NotNull] IFactory factory) : base(Constants.FieldCompilers.Normal)
        {
            Factory = factory;
        }

        public override bool IsExclusive { get; } = true;

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            // avoid being called by Json
            var textNode = TraceHelper.GetTextNode(field.ValueProperty);
            if (!(textNode is XmlTextNode))
            {
                return false;
            }

            return string.Equals(field.TemplateField.Type, "layout", StringComparison.OrdinalIgnoreCase) || field.FieldName == "__Renderings" || field.FieldName == "Final __Renderings";
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

            var layoutResolveContext = Factory.LayoutCompileContext(field.Item.Project, field.Database, textSnapshot);
            var layoutCompiler = Factory.LayoutCompiler();

            return layoutCompiler.Compile(layoutResolveContext, textNode);
        }
    }
}
