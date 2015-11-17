// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Emitters.Writers
{
    public class TemplateSectionWriter
    {
        [CanBeNull]
        [ItemNotNull]
        private IEnumerable<TemplateFieldWriter> _fieldBuilders;

        public TemplateSectionWriter([NotNull] TemplateSection templateSection)
        {
            TemplateSection = templateSection;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<TemplateFieldWriter> Fields
        {
            get { return _fieldBuilders ?? (_fieldBuilders = TemplateSection.Fields.Select(f => new TemplateFieldWriter(f)).ToList()); }
        }

        [CanBeNull]
        public Item Item { get; set; }

        [NotNull]
        public TemplateSection TemplateSection { get; }

        public void ResolveItem([NotNull] IEmitContext context, [CanBeNull] Item templateItem)
        {
            if (Item == null && templateItem != null)
            {
                Item = templateItem.Children[TemplateSection.SectionName];
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
