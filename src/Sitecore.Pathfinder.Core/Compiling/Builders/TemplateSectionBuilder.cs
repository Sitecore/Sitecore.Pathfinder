// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.Builders
{
    public class TemplateSectionBuilder
    {
        public TemplateSectionBuilder([NotNull] IFactoryService factory)
        {
            Factory = factory;
        }

        [NotNull]
        [ItemNotNull]
        public IList<TemplateFieldBuilder> Fields { get; } = new List<TemplateFieldBuilder>();

        [NotNull]
        public string SectionId { get; set; } = string.Empty;

        [NotNull]
        public ITextNode SectionIdTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string SectionName { get; set; } = string.Empty;

        [NotNull]
        public ITextNode SectionNameTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public ITextNode SectionTextNode { get; private set; }

        [NotNull]
        public TemplateBuilder TemplateBuilder { get; private set; }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        public TemplateSection Build([NotNull] Template template)
        {
            Guid sectionIdGuid;
            if (Guid.TryParse(SectionId, out sectionIdGuid))
            {
            }

            var section = Factory.TemplateSection(template, sectionIdGuid, SectionTextNode);

            section.SectionName = SectionName;
            if (SectionNameTextNode != TextNode.Empty)
            {
                section.SectionNameProperty.AddSourceTextNode(SectionNameTextNode);
            }

            foreach (var fieldBuilder in Fields)
            {
                var field = fieldBuilder.Build(template);
                section.Fields.Add(field);
            }

            return section;
        }

        [NotNull]
        public TemplateSectionBuilder With([NotNull] TemplateBuilder templateBuilder, [NotNull] ITextNode fieldTextNode)
        {
            TemplateBuilder = templateBuilder;
            SectionTextNode = fieldTextNode;

            return this;
        }
    }
}
