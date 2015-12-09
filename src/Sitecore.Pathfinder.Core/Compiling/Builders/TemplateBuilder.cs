// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Compiling.Builders
{
    public class TemplateBuilder
    {
        public TemplateBuilder([NotNull] IFactoryService factory)
        {
            Factory = factory;
        }

        [NotNull]
        public string BaseTemplates { get; set; } = string.Empty;

        [NotNull]
        public ITextNode BaseTemplatesTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string DatabaseName { get; set; } = string.Empty;

        [NotNull, ItemNotNull]
        public IList<FieldBuilder> Fields { get; } = new List<FieldBuilder>();

        [NotNull]
        public string Guid { get; set; } = string.Empty;

        [NotNull]
        public string Icon { get; set; } = string.Empty;

        [NotNull]
        public ITextNode IconTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string ItemIdOrPath { get; set; } = string.Empty;

        [NotNull]
        public string LongHelp { get; set; } = string.Empty;

        [NotNull]
        public ITextNode LongHelpTextNode { get; set; } = TextNode.Empty;

        [NotNull, ItemNotNull]
        public IList<TemplateSectionBuilder> Sections { get; } = new List<TemplateSectionBuilder>();

        [NotNull]
        public string ShortHelp { get; set; } = string.Empty;

        [NotNull]
        public ITextNode ShortHelpTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string TemplateName { get; set; } = string.Empty;

        [NotNull]
        public ITextNode TemplateNameTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        public Template Build([NotNull] IProject project, [NotNull] ITextNode rootTextNode)
        {
            var guid = StringHelper.GetGuid(project, Guid);

            var template = Factory.Template(project, guid, rootTextNode, DatabaseName, TemplateName, ItemIdOrPath);
            if (TemplateNameTextNode != TextNode.Empty)
            {
                template.ItemNameProperty.AddSourceTextNode(TemplateNameTextNode);
            }

            template.BaseTemplates = BaseTemplates;
            if (BaseTemplatesTextNode != TextNode.Empty)
            {
                template.BaseTemplatesProperty.SetValue(BaseTemplatesTextNode);
            }

            template.Icon = Icon;
            if (IconTextNode != TextNode.Empty)
            {
                template.IconProperty.SetValue(IconTextNode);
            }

            template.ShortHelp = ShortHelp;
            if (ShortHelpTextNode != TextNode.Empty)
            {
                template.ShortHelpProperty.SetValue(ShortHelpTextNode);
            }

            template.LongHelp = LongHelp;
            if (LongHelpTextNode != TextNode.Empty)
            {
                template.LongHelpProperty.SetValue(LongHelpTextNode);
            }

            foreach (var sectionBuilder in Sections)
            {
                var field = sectionBuilder.Build(template);
                template.Sections.Add(field);
            }

            return template;
        }
    }
}
