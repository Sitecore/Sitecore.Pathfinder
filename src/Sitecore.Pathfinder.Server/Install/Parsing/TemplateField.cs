// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Xml.Linq;
using Sitecore.Extensions.XElementExtensions;

namespace Sitecore.Pathfinder.Install.Parsing
{
    public class TemplateField
    {
        [NotNull]
        public string Icon { get; set; }

        public Guid Id { get; private set; }
        public int Sortorder { get; private set; }

        public bool IsShared { get; set; }

        public bool IsUnversioned { get; set; }

        [NotNull]
        public string LongHelp { get; set; }

        [NotNull]
        public string Name { get; private set; }

        [NotNull]
        public string ShortHelp { get; set; }

        [NotNull]
        public string Source { get; set; }

        [NotNull]
        public string Type { get; set; }

        public static TemplateField Parse(XElement templateFieldElement)
        {
            var template = new TemplateField
            {
                Id = Guid.Parse(templateFieldElement.GetAttributeValue("id")),
                Name = templateFieldElement.GetAttributeValue("name"),
                Icon = templateFieldElement.GetAttributeValue("icon"),
                Sortorder = int.Parse(templateFieldElement.GetAttributeValue("sortorder")),
                LongHelp = templateFieldElement.GetAttributeValue("longhelp"),
                ShortHelp = templateFieldElement.GetAttributeValue("shorthelp"),
                Source = templateFieldElement.GetAttributeValue("source"),
                Type = templateFieldElement.GetAttributeValue("type"),
                IsShared = templateFieldElement.GetAttributeValue("sharing") == "shared",
                IsUnversioned = templateFieldElement.GetAttributeValue("sharing") == "unversioned"
            };

            return template;
        }
    }
}
