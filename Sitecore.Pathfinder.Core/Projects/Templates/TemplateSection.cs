// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;

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
        public string Icon { get; set; } = string.Empty;

        [NotNull]
        public Attribute<string> SectionName { get; } = new Attribute<string>("Name", string.Empty);

        [NotNull]
        public ITextNode TemplateSectionTextNode { get; }

        public void Merge([NotNull] TemplateSection newSection, bool overwrite)
        {
            if (!string.IsNullOrEmpty(newSection.Icon))
            {
                Icon = newSection.Icon;
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
