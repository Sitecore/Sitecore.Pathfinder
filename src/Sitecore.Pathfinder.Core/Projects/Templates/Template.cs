// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Templates
{
    public class Template : DatabaseProjectItem
    {
        [NotNull]
        public static readonly Template Empty = new Template(Projects.Database.Empty, new Guid("{00000000-0000-0000-0000-000000000000}"), string.Empty, string.Empty);

        [NotNull]
        private readonly object _syncObject = new object();

        [CanBeNull, ItemNotNull]
        private List<TemplateField> _allFields;

        [CanBeNull, ItemNotNull]
        private ID[] _baseTemplateIDs;

        [FactoryConstructor]
        public Template([NotNull] IDatabase database, Guid guid, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(database, guid, itemName, itemIdOrPath)
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
            get => BaseTemplatesProperty.GetValue();
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
            get => LongHelpProperty.GetValue();
            set => LongHelpProperty.SetValue(value);
        }

        [NotNull]
        public SourceProperty<string> LongHelpProperty { get; }

        [NotNull, ItemNotNull]
        public ICollection<TemplateSection> Sections { get; }

        [NotNull]
        public string ShortHelp
        {
            get => ShortHelpProperty.GetValue();
            set => ShortHelpProperty.SetValue(value);
        }

        [NotNull]
        public SourceProperty<string> ShortHelpProperty { get; }

        [CanBeNull]
        public Item StandardValuesItem { get; set; }

        [NotNull, ItemNotNull]
        public virtual IEnumerable<TemplateField> GetAllFields()
        {
            return InternalGetAllFields(true, new List<string>());
        }

        [ItemNotNull, NotNull]
        public virtual IEnumerable<Template> GetBaseTemplates()
        {
            var baseTemplates = BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);

            foreach (var baseTemplate in baseTemplates)
            {
                var template = Project.Indexes.FindQualifiedItem<Template>(new ProjectItemUri(Database.DatabaseName, new Guid(baseTemplate)));
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

        [NotNull]
        public Template With([NotNull] ITextNode sourceTextNode, bool isEmiitable = true, bool isImport = false)
        {
            AddSourceTextNode(sourceTextNode);
            IsEmittable = isEmiitable;
            IsImport = isImport;
            return this;
        }

        protected override void Merge(IProjectItem newProjectItem, bool overwrite)
        {
            base.Merge(newProjectItem, overwrite);

            var newTemplate = newProjectItem as Template;
            Assert.Cast(newTemplate, nameof(newTemplate));

            if (!string.IsNullOrEmpty(newTemplate.BaseTemplates))
            {
                // todo: join base templates
                BaseTemplatesProperty.SetValue(newTemplate.BaseTemplatesProperty);
            }

            if (!string.IsNullOrEmpty(newTemplate.Icon))
            {
                IconProperty.SetValue(newTemplate.IconProperty);
            }

            if (!string.IsNullOrEmpty(newTemplate.ShortHelp))
            {
                ShortHelpProperty.SetValue(newTemplate.ShortHelpProperty);
            }

            if (!string.IsNullOrEmpty(newTemplate.LongHelp))
            {
                LongHelpProperty.SetValue(newTemplate.LongHelpProperty);
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

        [ItemNotNull, NotNull]
        private IEnumerable<TemplateField> InternalGetAllFields(bool cacheTemplate, [ItemNotNull, NotNull] ICollection<string> recursionGuard)
        {
            if (recursionGuard.Contains(Uri.Guid.Format()))
            {
                return Enumerable.Empty<TemplateField>();
            }

            recursionGuard.Add(Uri.Guid.Format());

            // ReSharper disable once InvertIf
            if (_allFields == null)
            {
                lock (_syncObject)
                {
                    // ReSharper disable once InvertIf
                    if (_allFields == null)
                    {
                        var allFields = new List<TemplateField>();

                        foreach (var field in Sections.SelectMany(s => s.Fields))
                        {
                            allFields.Add(field);
                        }

                        var baseTemplates = BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var baseTemplateId in baseTemplates)
                        {
                            if (baseTemplateId == Constants.NullGuidString)
                            {
                                continue;
                            }

                            var baseTemplate = Database.FindQualifiedItem<Template>(baseTemplateId);
                            if (baseTemplate == null)
                            {
                                continue;
                            }

                            foreach (var templateField in baseTemplate.InternalGetAllFields(false, recursionGuard))
                            {
                                var guid = templateField.Uri.Guid;
                                if (allFields.All(f => f.Uri.Guid != guid))
                                {
                                    allFields.Add(templateField);
                                }
                            }
                        }

                        if (cacheTemplate)
                        {
                            _allFields = allFields;
                        }

                        return allFields;
                    }
                }
            }

            return _allFields;
        }
    }
}
