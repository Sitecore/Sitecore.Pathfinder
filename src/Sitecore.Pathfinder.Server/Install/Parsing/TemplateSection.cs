// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Sitecore.Extensions.XElementExtensions;

namespace Sitecore.Pathfinder.Install.Parsing
{
    public class TemplateSection
    {
        [NotNull]
        public List<TemplateField> Fields = new List<TemplateField>();

        [NotNull]
        public string Icon { get; set; }

        public Guid Id { get; private set; }

        [NotNull]
        public string Name { get; private set; }

        public int Sortorder { get; private set; }

        public static TemplateSection Parse(XElement templateSectionElement)
        {
            var template = new TemplateSection
            {
                Id = Guid.Parse(templateSectionElement.GetAttributeValue("id")),
                Name = templateSectionElement.GetAttributeValue("name"),
                Icon = templateSectionElement.GetAttributeValue("icon"),
                Sortorder = int.Parse(templateSectionElement.GetAttributeValue("sortorder"))
            };

            foreach (var templateFieldElement in templateSectionElement.Elements())
            {
                template.Fields.Add(TemplateField.Parse(templateFieldElement));
            }

            return template;
        }
    }
}
