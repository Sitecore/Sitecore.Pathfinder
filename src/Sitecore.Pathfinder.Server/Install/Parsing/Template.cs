// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Sitecore.Extensions.XElementExtensions;

namespace Sitecore.Pathfinder.Install.Parsing
{
    public class Template
    {
        [NotNull]
        public List<TemplateSection> Sections = new List<TemplateSection>();

        [NotNull]
        public string BaseTemplates { get; private set; }

        [NotNull]
        public string Database { get; private set; }

        [NotNull]
        public string Icon { get; private set; }

        public Guid Id { get; private set; }

        [NotNull]
        public string LongHelp { get; private set; }

        [NotNull]
        public string Name { get; private set; }

        [NotNull]
        public string Path { get; private set; }

        [NotNull]
        public string ShortHelp { get; private set; }

        [NotNull]
        public string StandardValuesItemId { get; private set; }

        public static Template Parse(XElement templateElement)
        {
            var template = new Template
            {
                Id = Guid.Parse(templateElement.GetAttributeValue("id")),
                Database = templateElement.GetAttributeValue("database"),
                Name = templateElement.GetAttributeValue("name"),
                Path = templateElement.GetAttributeValue("path"),
                Icon = templateElement.GetAttributeValue("icon"),
                LongHelp = templateElement.GetAttributeValue("longhelp"),
                ShortHelp = templateElement.GetAttributeValue("shorthelp"),
                BaseTemplates = templateElement.GetAttributeValue("basetemplates"),
                StandardValuesItemId = templateElement.GetAttributeValue("standardvaluesid")
            };

            foreach (var templateSectionElement in templateElement.Elements())
            {
                template.Sections.Add(TemplateSection.Parse(templateSectionElement));
            }

            return template;
        }
    }
}
