// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Templates
{
    public class Template : DatabaseProjectItem
    {
        [NotNull]
        public static readonly Template Empty = new Template(Projects.Project.Empty, TextNode.Empty, new Guid("{00000000-0000-0000-0000-000000000000}"), string.Empty, string.Empty, string.Empty);

        [CanBeNull, ItemNotNull]
        private ID[] _baseTemplates;

        public Template([NotNull] IProject project, [NotNull] ITextNode textNode, Guid guid, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, textNode, guid, databaseName, itemName, itemIdOrPath)
        {
        }

        [NotNull, ItemNotNull, Obsolete("Use BaseTemplates instead", false)]
        public ID[] BaseTemplateIDs => _baseTemplates ?? (_baseTemplates = BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries).Select(id => new ID(id)).ToArray());

        [NotNull]
        public string BaseTemplates
        {
            get { return BaseTemplatesProperty.GetValue(); }
            set
            {
                BaseTemplatesProperty.SetValue(value);
                _baseTemplates = null;
            }
        }

        [NotNull]
        public SourceProperty<string> BaseTemplatesProperty { get; } = new SourceProperty<string>("BaseTemplates", string.Empty);

        [NotNull, ItemNotNull]
        public IEnumerable<TemplateField> Fields => Sections.SelectMany(s => s.Fields);

        [NotNull]
        public string LongHelp
        {
            get { return LongHelpProperty.GetValue(); }
            set { LongHelpProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> LongHelpProperty { get; } = new SourceProperty<string>("LongHelp", string.Empty);

        [NotNull, ItemNotNull]
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

        [NotNull, ItemNotNull]
        public virtual IEnumerable<TemplateField> GetAllFields()
        {
            var templates = new List<ProjectItemUri>();
            return GetAllFields(templates, this);
        }

        [CanBeNull]
        public virtual TemplateField GetField([NotNull] string fieldName)
        {
            foreach (var templateField in GetAllFields())
            {
                if (string.Equals(templateField.FieldName, fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return templateField;
                }
            }

            return null;
        }

        public void Merge([NotNull] Template newTemplate)
        {
            Merge(newTemplate, true);
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<TemplateField> GetAllFields([NotNull, ItemNotNull]  ICollection<ProjectItemUri> templates, [NotNull] Template template)
        {
            templates.Add(template.Uri);

            foreach (var field in template.Sections.SelectMany(s => s.Fields))
            {
                yield return field;
            }

            var baseTemplates = template.BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);
            foreach (var baseTemplateId in baseTemplates)
            {
                if (baseTemplateId == Constants.NullGuidString)
                {
                    continue;
                }

                var baseTemplate = Project.FindQualifiedItem<Template>(baseTemplateId);
                if (baseTemplate == null)
                {
                    continue;
                }

                if (templates.Contains(baseTemplate.Uri))
                {
                    continue;
                }

                foreach (var templateField in GetAllFields(templates, baseTemplate))
                {
                    yield return templateField;
                }
            }
        }

        protected override void Merge(IProjectItem newProjectItem, bool overwrite)
        {
            base.Merge(newProjectItem, overwrite);

            var newTemplate = newProjectItem as Template;
            Assert.Cast(newTemplate, nameof(newTemplate));

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

        protected override void OnUnload()
        {
            base.OnUnload();
            foreach (var unloadable in Sections.OfType<IUnloadable>())
            {
                unloadable.Unload();
            }
        }
    }
}
