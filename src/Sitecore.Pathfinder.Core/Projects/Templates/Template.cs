// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Templates
{
    public class Template : ItemBase
    {
        [NotNull]
        public static readonly Template Empty = new Template(Projects.Project.Empty, TextNode.Empty, new Guid("{7A3E077F-D985-453F-8773-348ADFEAF2FD}"), string.Empty, string.Empty, string.Empty);

        public Template([NotNull] IProject project, [NotNull] ITextNode textNode, Guid guid, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, textNode, guid, databaseName, itemName, itemIdOrPath)
        {
        }

        [NotNull]
        public string BaseTemplates
        {
            get { return BaseTemplatesProperty.GetValue(); }
            set { BaseTemplatesProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> BaseTemplatesProperty { get; } = new SourceProperty<string>("BaseTemplates", string.Empty);

        [NotNull]
        public string LongHelp
        {
            get { return LongHelpProperty.GetValue(); }
            set { LongHelpProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> LongHelpProperty { get; } = new SourceProperty<string>("LongHelp", string.Empty);

        [NotNull]
        [ItemNotNull]
        public IList<TemplateSection> Sections { get; } = new List<TemplateSection>();

        [NotNull]
        public string ShortHelp
        {
            get { return ShortHelpProperty.GetValue(); }
            set { ShortHelpProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> ShortHelpProperty { get; } = new SourceProperty<string>("ShortHelp", string.Empty);

        [CanBeNull]
        public Item StandardValuesItem { get; set; }

        public void Merge([NotNull] Template newTemplate)
        {
            Merge(newTemplate, true);
        }

        protected override void Merge(IProjectItem newProjectItem, bool overwrite)
        {
            base.Merge(newProjectItem, overwrite);

            var newTemplate = newProjectItem as Template;
            if (newTemplate == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(newTemplate.BaseTemplates))
            {
                // todo: join base templates
                BaseTemplatesProperty.SetValue(newTemplate.BaseTemplatesProperty, SetValueOptions.DisableUpdates);
            }

            if (!string.IsNullOrEmpty(newTemplate.Icon))
            {
                IconProperty.SetValue(newTemplate.IconProperty, SetValueOptions.DisableUpdates);
            }

            if (!string.IsNullOrEmpty(newTemplate.ShortHelp))
            {
                ShortHelpProperty.SetValue(newTemplate.ShortHelpProperty, SetValueOptions.DisableUpdates);
            }

            if (!string.IsNullOrEmpty(newTemplate.LongHelp))
            {
                LongHelpProperty.SetValue(newTemplate.LongHelpProperty, SetValueOptions.DisableUpdates);
            }

            foreach (var newSection in newTemplate.Sections)
            {
                var section = Sections.FirstOrDefault(s => string.Equals(s.SectionName, newSection.SectionName, StringComparison.OrdinalIgnoreCase));
                if (section == null)
                {
                    Sections.Add(newSection);
                    continue;
                }

                section.Merge(newSection, overwrite);
            }
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<TemplateField> GetAllFields()
        {
            var fields = new List<TemplateField>();
            var templates = new List<ProjectItemUri>();

            GetAllFields(fields, templates, this);

            return fields;
        }

        private void GetAllFields([NotNull][ItemNotNull] ICollection<TemplateField> fields, [NotNull][ItemNotNull] ICollection<ProjectItemUri> templates, [NotNull] Template template)
        {
            templates.Add(template.Uri);

            foreach (var field in template.Sections.SelectMany(s => s.Fields))
            {
                if (fields.All(f => !string.Equals(f.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase)))
                {
                    fields.Add(field);
                }
            }

            var nullGuid = Guid.Empty.Format();

            var baseTemplates = template.BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);
            foreach (var baseTemplateId in baseTemplates)
            {
                if (baseTemplateId == nullGuid)
                {
                    continue;
                }

                var baseTemplate = Project.FindQualifiedItem(baseTemplateId) as Template;
                if (baseTemplate == null)
                {
                    continue;
                }

                if (templates.Contains(baseTemplate.Uri))
                {
                    continue;
                }

                GetAllFields(fields, templates, baseTemplate);
            }
        }
    }
}
