// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Builders.FieldResolvers.Layouts;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Documents.Xml;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class LayoutFieldResolver : FieldResolverBase
    {
        public LayoutFieldResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(IEmitContext context, TemplateField templateField, Field field)
        {
            return string.Compare(templateField.Type, "layout", StringComparison.OrdinalIgnoreCase) == 0 && field.Value.Value.StartsWith("Layout: ");
        }

        public override string Resolve(IEmitContext context, TemplateField templateField, Field field)
        {
            var textNode = field.Value.Source ?? TextNode.Empty;
            if (textNode == TextNode.Empty)
            {
                return field.Value.Value.Mid(8);
            }

            var textSnapshot = textNode.Snapshot as ITextSnapshot;
            if (textSnapshot == null)
            {
                return field.Value.Value.Mid(8);
            }

            var layoutResolveContext = new LayoutResolveContext(context, textSnapshot, field.Item.DatabaseName);

            var resolver = textNode is XmlTextNode ? (LayoutResolverBase)new XmlLayoutResolver() : new JsonLayoutResolver();

            return resolver.Resolve(layoutResolveContext, textNode);
        }
    }
}
