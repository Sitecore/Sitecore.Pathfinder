// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Emitting.Parsing;

namespace Sitecore.Pathfinder.Emitting.Writers
{
    public class TemplateFieldWriter
    {
        public TemplateFieldWriter([NotNull] TemplateField templateField)
        {
            TemplateField = templateField;
        }

        [CanBeNull]
        public Data.Items.Item Item { get; set; }

        [NotNull]
        public TemplateField TemplateField { get; }

        public void ResolveItem([CanBeNull] Data.Items.Item sectionItem)
        {
            if (Item == null && sectionItem != null)
            {
                Item = sectionItem.Children[TemplateField.Name];
            }
        }
    }
}
