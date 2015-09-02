// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class LayoutFieldResolver : FieldResolverBase
    {
        public LayoutFieldResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(Field field)
        {
            return string.Compare(field.TemplateField.Type, "layout", StringComparison.OrdinalIgnoreCase) == 0 && field.ValueHint.Contains("Layout");
        }

        public override string Resolve(Field field)
        {
            var textNode = TraceHelper.FirstTextNode(field.ValueProperty);
            if (textNode == TextNode.Empty)
            {
                return field.Value.Mid(8);
            }

            var textSnapshot = textNode.Snapshot as ITextSnapshot;
            if (textSnapshot == null)
            {
                return field.Value.Mid(8);
            }

            /*
            var layoutResolveContext = new LayoutResolveContext(project, textSnapshot);

            var resolver = textNode is XmlTextNode ? (LayoutResolverBase)new XmlLayoutResolver() : new JsonLayoutResolver();

            return resolver.Resolve(layoutResolveContext, textNode);
            */

            return field.Value.Mid(8);
        }
    }
}
