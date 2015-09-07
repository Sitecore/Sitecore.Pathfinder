// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Xml;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers.Layouts
{
    [Export(typeof(IFieldResolver))]
    public class XmlLayoutFieldResolver : FieldResolverBase
    {
        public XmlLayoutFieldResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(Field field)
        {
            var textNode = TraceHelper.GetTextNode(field.ValueProperty);
            if (!(textNode is XmlTextNode))
            {
                return false;
            }

            return string.Compare(field.TemplateField.Type, "layout", StringComparison.OrdinalIgnoreCase) == 0 || field.ValueHint.Contains("Layout");
        }

        public override string Resolve(Field field)
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

            var layoutResolveContext = new LayoutResolveContext(field, textSnapshot);

            var resolver = new XmlLayoutResolver();

            return resolver.Resolve(layoutResolveContext, textNode);
        }
    }
}
