// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Emitting.Parsing;

namespace Sitecore.Pathfinder.Emitting.Writers
{
    public class TemplateSectionWriter
    {
        [CanBeNull]
        private IEnumerable<TemplateFieldWriter> _fieldBuilders;

        public TemplateSectionWriter([NotNull] TemplateSection templateSection)
        {
            TemplateSection = templateSection;
        }

        [NotNull]
        public IEnumerable<TemplateFieldWriter> Fields
        {
            get { return _fieldBuilders ?? (_fieldBuilders = TemplateSection.Fields.Select(f => new TemplateFieldWriter(f)).ToList()); }
        }

        [CanBeNull]
        public Data.Items.Item Item { get; set; }

        [NotNull]
        public TemplateSection TemplateSection { get; }

        public void ResolveItem([CanBeNull] Data.Items.Item templateItem)
        {
            if (Item == null && templateItem != null)
            {
                Item = templateItem.Children[TemplateSection.Name];
            }

            if (Item == null)
            {
                return;
            }

            foreach (var field in Fields)
            {
                field.ResolveItem(Item);
            }
        }
    }
}
