// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitters.FieldResolvers.Layouts;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
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

        public override bool CanResolve(ITraceService trace, IProject project, Field field)
        {
            return string.Compare(field.TemplateField.Type, "layout", StringComparison.OrdinalIgnoreCase) == 0 && field.ValueHint.Value.Contains("Layout");
        }

        public override string Resolve(ITraceService trace, IProject project, Field field)
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

            var layoutResolveContext = new LayoutResolveContext(trace, project, textSnapshot, field.Item.DatabaseName);

            var resolver = textNode is XmlTextNode ? (LayoutResolverBase)new XmlLayoutResolver() : new JsonLayoutResolver();

            return resolver.Resolve(layoutResolveContext, textNode);
        }
    }
}
