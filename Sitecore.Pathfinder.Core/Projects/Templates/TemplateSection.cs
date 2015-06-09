// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Templates
{
    public class TemplateSection
    {
        public TemplateSection()
        {
            Name = string.Empty;

            Fields = new List<TemplateField>();
        }

        [NotNull]
        public IList<TemplateField> Fields { get; }

        [NotNull]
        public string Icon { get; set; }

        [NotNull]
        public string Name { get; set; }

        public void Merge([NotNull] TemplateSection newSection, bool overwrite)
        {
            if (!string.IsNullOrEmpty(newSection.Icon))
            {
                Icon = newSection.Icon;
            }

            foreach (var newField in newSection.Fields)
            {
                var field = Fields.FirstOrDefault(f => string.Compare(f.Name, newField.Name, StringComparison.OrdinalIgnoreCase) == 0);
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
