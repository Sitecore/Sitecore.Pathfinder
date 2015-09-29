// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Emitters.Writers
{
    public class TemplateFieldWriter
    {
        public TemplateFieldWriter([NotNull] TemplateField templateField)
        {
            TemplateField = templateField;
        }

        [CanBeNull]
        public Item Item { get; set; }

        [NotNull]
        public TemplateField TemplateField { get; }

        public void ResolveItem([NotNull] IEmitContext context, [CanBeNull] Item sectionItem)
        {
            if (Item == null && sectionItem != null)
            {
                Item = sectionItem.Children[TemplateField.FieldName];
            }
        }
    }
}
