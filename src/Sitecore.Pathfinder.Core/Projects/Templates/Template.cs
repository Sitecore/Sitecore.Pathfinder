// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

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
        private List<TemplateField> _allFields;

        [CanBeNull, ItemNotNull]
        private ID[] _baseTemplateIDs;

        public Template([NotNull] IProjectBase project, [NotNull] ITextNode textNode, Guid guid, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, textNode, guid, databaseName, itemName, itemIdOrPath)
        {
            BaseTemplatesProperty = NewSourceProperty("BaseTemplates", string.Empty);
            LongHelpProperty = NewSourceProperty("LongHelp", string.Empty);
            ShortHelpProperty = NewSourceProperty("ShortHelp", string.Empty);

            Sections = new LockableList<TemplateSection>(this);
        }

        [NotNull, ItemNotNull, Obsolete("Use BaseTemplates instead", false)]
        public ID[] BaseTemplateIDs => _baseTemplateIDs ?? (_baseTemplateIDs = BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries).Select(id => new ID(id)).ToArray());

        [NotNull]
        public string BaseTemplates
        {
            get { return BaseTemplatesProperty.GetValue(); }
            set
            {
                BaseTemplatesProperty.SetValue(value);
                _baseTemplateIDs = null;
            }
        }

        [NotNull]
        public SourceProperty<string> BaseTemplatesProperty { get; }

        [NotNull, ItemNotNull]
        public IEnumerable<TemplateField> Fields => Sections.SelectMany(s => s.Fields);

        [NotNull]
        public string LongHelp
        {
            get { return LongHelpProperty.GetValue(); }
            set { LongHelpProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> LongHelpProperty { get; }

        [NotNull, ItemNotNull]
        public ICollection<TemplateSection> Sections { get; }

        [NotNull]
        public string ShortHelp
        {
            get { return ShortHelpProperty.GetValue(); }
            set { ShortHelpProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> ShortHelpProperty { get; }

        [CanBeNull]
        public Item StandardValuesItem { get; set; }

        [NotNull, ItemNotNull]
        public virtual IEnumerable<TemplateField> GetAllFields()
        {
            if (_allFields != null)
            {
                return _allFields;
            }

            var templates = new List<ProjectItemUri>();
            return GetAllFields(templates);
        }

        [ItemNotNull, NotNull]
        public virtual IEnumerable<Template> GetBaseTemplates()
        {
            var baseTemplates = BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);

            foreach (var baseTemplate in baseTemplates)
            {
                var template = Project.FindQualifiedItem<Template>(new ProjectItemUri(Database.DatabaseName, new Guid(baseTemplate)));
                if (template != null)
                {
                    yield return template;
                }
            }
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

        public bool Is([NotNull] Template template)
        {
            return GetBaseTemplates().Any(t => t.Uri == template.Uri);
        }

        public void Merge([NotNull] Template newTemplate)
        {
            Merge(newTemplate, true);
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<TemplateField> GetAllFields([NotNull, ItemNotNull] ICollection<ProjectItemUri> templates)
        {
            if (_allFields != null)
            {
                return _allFields;
            }

            templates.Add(Uri);
            _allFields = new List<TemplateField>();

            foreach (var field in Sections.SelectMany(s => s.Fields))
            {
                _allFields.Add(field);
            }

            var baseTemplates = BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);
            foreach (var baseTemplateId in baseTemplates)
            {
                if (baseTemplateId == Constants.NullGuidString)
                {
                    continue;
                }

                var baseTemplate = Project.FindQualifiedItem<Template>(Database, baseTemplateId);
                if (baseTemplate == null)
                {
                    continue;
                }

                if (templates.Contains(baseTemplate.Uri))
                {
                    continue;
                }

                foreach (var templateField in baseTemplate.GetAllFields(templates))
                {
                    var guid = templateField.Uri.Guid;
                    if (_allFields.All(f => f.Uri.Guid != guid))
                    {
                        _allFields.Add(templateField);
                    }
                }
            }

            return _allFields;
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
    }
}
