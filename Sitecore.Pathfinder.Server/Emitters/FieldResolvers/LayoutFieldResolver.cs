// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Emitters.FieldResolvers.Layouts;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Items.FieldResolvers;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Xml;

namespace Sitecore.Pathfinder.Emitters.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class LayoutFieldResolver : FieldResolverBase
    {
        public LayoutFieldResolver() : base(Constants.FieldResolvers.High)
        {
        }

        public override bool CanResolve(Field field)
        {
            return string.Compare(field.TemplateField.Type, "layout", StringComparison.OrdinalIgnoreCase) == 0 && field.ValueHint.Contains("Layout");
        }

        public override string Resolve(Field field)
        {
            var textNode = TraceHelper.GetTextNode(field.ValueProperty);
            if (textNode == TextNode.Empty)
            {
                return field.Value.Mid(8);
            }

            var textSnapshot = textNode.Snapshot as ITextSnapshot;
            if (textSnapshot == null)
            {
                return field.Value.Mid(8);
            }

            var layoutResolveContext = new LayoutResolveContext(field, field.Item.Project, textSnapshot, field.Item.DatabaseName);

            var resolver = textNode is XmlTextNode ? (LayoutResolverBase)new XmlLayoutResolver() : new JsonLayoutResolver();

            return resolver.Resolve(layoutResolveContext, textNode);
        }
    }
}
