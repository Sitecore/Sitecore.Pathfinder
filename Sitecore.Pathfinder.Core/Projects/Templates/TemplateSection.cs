// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Templates
{
    public class TemplateSection
    {
        public TemplateSection([NotNull] ITextNode templateSectionTextNode)
        {
            TemplateSectionTextNode = templateSectionTextNode;
        }

        [NotNull]
        public IList<TemplateField> Fields { get; } = new List<TemplateField>();

        [NotNull]
        public Attribute<string> Icon { get; } = new Attribute<string>("Icon", string.Empty);

        [NotNull]
        public Attribute<string> SectionName { get; } = new Attribute<string>("Name", string.Empty);

        [NotNull]
        public ITextNode TemplateSectionTextNode { get; }

        public void Merge([NotNull] TemplateSection newSection, bool overwrite)
        {
            if (!string.IsNullOrEmpty(newSection.Icon.Value))
            {
                Icon.Merge(newSection.Icon);
            }

            foreach (var newField in newSection.Fields)
            {
                var field = Fields.FirstOrDefault(f => string.Compare(f.FieldName.Value, newField.FieldName.Value, StringComparison.OrdinalIgnoreCase) == 0);
                if (field == null)
                {
                    Fields.Add(newField);
                    continue;
                }

                field.Merge(newField, overwrite);
            }
        }
    }
}
