// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Builders.Templates
{
    public class TemplateSectionBuilder
    {
        [CanBeNull]
        private IEnumerable<TemplateFieldBuilder> _fieldBuilders;

        public TemplateSectionBuilder([NotNull] TemplateSection templateSection)
        {
            TemplateSection = templateSection;
        }

        [NotNull]
        public IEnumerable<TemplateFieldBuilder> Fields
        {
            get
            {
                return _fieldBuilders ?? (_fieldBuilders = TemplateSection.Fields.Select(f => new TemplateFieldBuilder(f)).ToList());
            }
        }

        [CanBeNull]
        public Item Item { get; set; }

        [NotNull]
        public TemplateSection TemplateSection { get; }

        public void ResolveItem([NotNull] IEmitContext context, [CanBeNull] Item templateItem)
        {
            if (Item == null && templateItem != null)
            {
                Item = templateItem.Children[TemplateSection.SectionName.Value];
            }

            if (Item == null)
            {
                return;
            }

            foreach (var field in Fields)
            {
                field.ResolveItem(context, Item);
            }
        }
    }
}
