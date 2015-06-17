// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Items;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Builders.Templates
{
    public class TemplateFieldBuilder
    {
        public TemplateFieldBuilder([NotNull] TemplateField templaterField)
        {
            TemplaterField = templaterField;
        }

        [CanBeNull]
        public Item Item { get; set; }

        [NotNull]
        public TemplateField TemplaterField { get; }

        public void ResolveItem([NotNull] IEmitContext context, [CanBeNull] Item sectionItem)
        {
            if (Item == null && sectionItem != null)
            {
                Item = sectionItem.Children[TemplaterField.FieldName.Value];
            }
        }
    }
}
