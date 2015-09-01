// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Templates
{
    public class Template : ItemBase
    {
        public static readonly Template Empty = new Template(Projects.Project.Empty, "{7A3E077F-D985-453F-8773-348ADFEAF2FD}", TextNode.Empty, string.Empty, string.Empty, string.Empty);

        public Template([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode document, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, projectUniqueId, document, databaseName, itemName, itemIdOrPath)
        {
        }

        [NotNull]
        public Attribute<string> BaseTemplates { get; } = new Attribute<string>("BaseTemplates", string.Empty);

        [NotNull]
        public Attribute<string> LongHelp { get; } = new Attribute<string>("LongHelp", string.Empty);

        [NotNull]
        public IList<TemplateSection> Sections { get; } = new List<TemplateSection>();

        [NotNull]
        public Attribute<string> ShortHelp { get; } = new Attribute<string>("ShortHelp", string.Empty);

        [CanBeNull]
        public Item StandardValuesItem { get; set; }

        public void Merge([NotNull] IParseContext context, [NotNull] Template newTemplate)
        {
            Merge(context, newTemplate, true);
        }

        protected override void Merge(IParseContext context, IProjectItem newProjectItem, bool overwrite)
        {
            base.Merge(context, newProjectItem, overwrite);

            var newTemplate = newProjectItem as Template;
            if (newTemplate == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(newTemplate.BaseTemplates.Value))
            {
                // todo: join base templates
                BaseTemplates.Merge(newTemplate.BaseTemplates);
            }

            if (!string.IsNullOrEmpty(newTemplate.Icon.Value))
            {
                Icon.Merge(newTemplate.Icon);
            }

            if (!string.IsNullOrEmpty(newTemplate.ShortHelp.Value))
            {
                ShortHelp.Merge(newTemplate.ShortHelp);
            }

            if (!string.IsNullOrEmpty(newTemplate.LongHelp.Value))
            {
                LongHelp.Merge(newTemplate.LongHelp);
            }

            foreach (var newSection in newTemplate.Sections)
            {
                var section = Sections.FirstOrDefault(s => string.Compare(s.SectionName.Value, newSection.SectionName.Value, StringComparison.OrdinalIgnoreCase) == 0);
                if (section == null)
                {
                    Sections.Add(newSection);
                    continue;
                }

                section.Merge(newSection, overwrite);
            }
        }
    }
}
