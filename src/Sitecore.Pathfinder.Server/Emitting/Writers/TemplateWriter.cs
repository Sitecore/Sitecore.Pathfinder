// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Extensions.StringExtensions;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting.Parsing;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Emitting.Writers
{
    public class TemplateWriter
    {
        [CanBeNull]
        private IEnumerable<TemplateSectionWriter> _sectionBuilders;

        public TemplateWriter([NotNull] Template template)
        {
            Template = template;
        }

        [CanBeNull]
        public Data.Items.Item Item { get; set; }

        [NotNull]
        public IEnumerable<TemplateSectionWriter> Sections
        {
            get { return _sectionBuilders ?? (_sectionBuilders = Template.Sections.Select(s => new TemplateSectionWriter(s)).ToList()); }
        }

        [NotNull]
        public Template Template { get; }

        [CanBeNull]
        public Data.Items.Item Write()
        {
            List<Data.Templates.TemplateField> inheritedFields;
            string baseTemplates;

            GetInheritedFields(Template, out inheritedFields, out baseTemplates);

            if (Item == null)
            {
                ResolveItem();
            }

            if (Item == null)
            {
                WriteNewTemplate(inheritedFields, baseTemplates);
                if (Item == null)
                {
                    return null;
                }
            }
            else
            {
                WriteTemplate( inheritedFields, baseTemplates);
                DeleteSections();
            }

            SortSections(this);

            return Item;
        }

        protected virtual void DeleteFields([NotNull] TemplateSectionWriter templateSectionWriter)
        {
            if (templateSectionWriter.Item == null)
            {
                return;
            }

            foreach (Data.Items.Item child in templateSectionWriter.Item.Children)
            {
                if (child.TemplateID != TemplateIDs.TemplateField)
                {
                    continue;
                }

                var found = false;

                foreach (var field in templateSectionWriter.Fields)
                {
                    if (field.Item == null)
                    {
                        continue;
                    }

                    if (field.Item.ID == child.ID)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    child.Recycle();
                }
            }
        }

        protected virtual void DeleteSections()
        {
            if (Item == null)
            {
                return;
            }

            foreach (Data.Items.Item child in Item.Children)
            {
                if (child.TemplateID != TemplateIDs.TemplateSection)
                {
                    continue;
                }

                var found = false;

                foreach (var section in Sections)
                {
                    if (section.Item == null)
                    {
                        continue;
                    }

                    if (section.Item.ID == child.ID)
                    {
                        DeleteFields(section);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    child.Recycle();
                }
            }
        }

        
        protected void GetInheritedFields([NotNull] Template template, [NotNull] out List<Data.Templates.TemplateField> fields, [NotNull] out string baseTemplates)
        {
            fields = new List<Data.Templates.TemplateField>();

            var database = Factory.GetDatabase(template.Database);
            var baseTemplateList = new List<Data.Items.Item>();

            var templates = template.BaseTemplates.Split(new [] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var templateId in templates)
            {
                // resolve possible item paths
                var baseTemplateItem = database.GetItem(templateId);
                if (baseTemplateItem == null)
                {
                    throw new RetryableEmitException("Template missing", templateId);
                }

                baseTemplateList.Add(baseTemplateItem);

                var t = TemplateManager.GetTemplate(baseTemplateItem.ID, database);
                if (t == null)
                {
                    throw new RetryableEmitException("Template missing", templateId);
                }

                var templateFields = t.GetFields(true);

                foreach (var templateField in templateFields)
                {
                    if (fields.All(f => f.Name != templateField.Name))
                    {
                        fields.Add(templateField);
                    }
                }
            }

            baseTemplates = string.Join("|", baseTemplateList.Select(t => t.ID.ToString()));
        }

        [CanBeNull]
        protected virtual Data.Items.Item GetParentItem([NotNull] Database database)
        {
            var parentPath = GetItemParentPath(Template.Path);
            if (string.IsNullOrEmpty(parentPath))
            {
                return null;
            }

            var parentItem = database.GetItem(parentPath);
            if (parentItem == null)
            {
                var innerItem = database.GetItem(TemplateIDs.TemplateFolder);
                if (innerItem != null)
                {
                    var templateFolder = new TemplateItem(innerItem);
                    parentItem = database.CreateItemPath(parentPath, templateFolder, templateFolder);
                }
            }

            return parentItem;
        }

        protected virtual void ResolveItem()
        {
            var database = Factory.GetDatabase(Template.Database);
            if (database == null)
            {
                return;
            }

            Item = database.GetItem(new ID(Template.Id));
            if (Item == null)
            {
                return;
            }

            foreach (var section in Sections)
            {
                section.ResolveItem(Item);
            }
        }

        protected virtual void SortFields([NotNull] TemplateSectionWriter templateSectionWriter)
        {
            var lastSortorder = 0;

            var fields = templateSectionWriter.Fields.ToList();
            for (var index = 0; index < fields.Count; index++)
            {
                var fieldWriter = fields.ElementAt(index);
                if (fieldWriter.Item == null)
                {
                    continue;
                }

                var sortorder = fieldWriter.Item.Appearance.Sortorder;

                if (sortorder <= lastSortorder)
                {
                    var nextSortorder = lastSortorder + 200;

                    if (index < fields.Count - 1)
                    {
                        var nextField = fields.ElementAt(index + 1);
                        if (nextField.Item != null)
                        {
                            nextSortorder = nextField.Item.Appearance.Sortorder;
                            if (nextSortorder < lastSortorder + 2)
                            {
                                nextSortorder = lastSortorder + 200;
                            }
                        }
                    }

                    sortorder = lastSortorder + (nextSortorder - lastSortorder) / 2;

                    using (new EditContext(fieldWriter.Item))
                    {
                        fieldWriter.Item.Appearance.Sortorder = sortorder;
                    }
                }

                lastSortorder = sortorder;
            }
        }

        protected virtual void SortSections([NotNull] TemplateWriter templateWriter)
        {
            var lastSortorder = 0;

            var sections = templateWriter.Sections.ToList();
            for (var index = 0; index < sections.Count; index++)
            {
                var section = sections.ElementAt(index);
                if (section.Item == null)
                {
                    continue;
                }

                var sortorder = section.Item.Appearance.Sortorder;

                if (sortorder <= lastSortorder)
                {
                    var nextSortorder = lastSortorder + 200;

                    if (index < sections.Count - 1)
                    {
                        var nextSection = sections.ElementAt(index + 1);
                        if (nextSection.Item != null)
                        {
                            nextSortorder = nextSection.Item.Appearance.Sortorder;
                            if (nextSortorder < lastSortorder + 2)
                            {
                                nextSortorder = lastSortorder + 200;
                            }
                        }
                    }

                    sortorder = lastSortorder + (nextSortorder - lastSortorder) / 2;

                    using (new EditContext(section.Item))
                    {
                        section.Item.Appearance.Sortorder = sortorder;
                    }
                }

                SortFields(section);

                lastSortorder = sortorder;
            }
        }

        protected virtual void WriteField([NotNull] TemplateSectionWriter templateSectionWriter, [NotNull] TemplateFieldWriter templateFieldWriter, [NotNull] IEnumerable<Data.Templates.TemplateField> inheritedFields)
        {
            if (inheritedFields.Any(f => string.Equals(f.Name, templateFieldWriter.TemplateField.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var item = templateFieldWriter.Item;

            var isNew = item == null;
            if (isNew)
            {
                var id = new ID(templateFieldWriter.TemplateField.Id);
                item = templateSectionWriter.Item.Database.AddFromTemplateSynchronized(templateFieldWriter.TemplateField.Name, new TemplateID(TemplateIDs.TemplateField), templateSectionWriter.Item, id);
                if (item == null)
                {
                    throw new EmitException("Could not create template field", templateFieldWriter.TemplateField.Name);
                }

                templateFieldWriter.Item = item;
            }

            if (templateSectionWriter.Item != null && item.ParentID != templateSectionWriter.Item.ID)
            {
                item.MoveTo(templateSectionWriter.Item);
            }

            using (new EditContext(item))
            {
                if (!string.IsNullOrEmpty(templateFieldWriter.TemplateField.Name))
                {
                    item.Name = templateFieldWriter.TemplateField.Name;
                }

                if (!string.IsNullOrEmpty(templateFieldWriter.TemplateField.Type))
                {
                    item["Type"] = templateFieldWriter.TemplateField.Type;
                }

                item["Shared"] = templateFieldWriter.TemplateField.IsShared ? "1" : string.Empty;
                item["Unversioned"] = templateFieldWriter.TemplateField.IsUnversioned ? "1" : string.Empty;

                if (!string.IsNullOrEmpty(templateFieldWriter.TemplateField.Source))
                {
                    item["Source"] = templateFieldWriter.TemplateField.Source;
                }

                if (!string.IsNullOrEmpty(templateFieldWriter.TemplateField.ShortHelp))
                {
                    item.Help.ToolTip = templateFieldWriter.TemplateField.ShortHelp;
                }

                if (!string.IsNullOrEmpty(templateFieldWriter.TemplateField.LongHelp))
                {
                    item.Help.Text = templateFieldWriter.TemplateField.LongHelp;
                }

                item.Appearance.Sortorder = templateFieldWriter.TemplateField.Sortorder;
            }
        }

        protected virtual void WriteNewTemplate([NotNull] IEnumerable<Data.Templates.TemplateField> inheritedFields, [NotNull] string baseTemplates)
        {
            var database = Factory.GetDatabase(Template.Database);
            if (database == null)
            {
                return;
            }

            var parentItem = GetParentItem(database);
            if (parentItem == null)
            {
                throw new RetryableEmitException("Failed to create template");
            }

            var item = parentItem.Database.AddFromTemplateSynchronized(Template.Name, new TemplateID(TemplateIDs.Template), parentItem, new ID(Template.Id));
            if (item == null)
            {
                throw new EmitException("Failed to add new template");
            }

            Item = item;
            using (new EditContext(item))
            {
                if (!string.IsNullOrEmpty(baseTemplates))
                {
                    item[FieldIDs.BaseTemplate] = baseTemplates;
                }

                if (!string.IsNullOrEmpty(Template.Icon))
                {
                    item.Appearance.Icon = Template.Icon;
                }

                if (!string.IsNullOrEmpty(Template.ShortHelp))
                {
                    item.Help.ToolTip = Template.ShortHelp;
                }

                if (!string.IsNullOrEmpty(Template.LongHelp))
                {
                    item.Help.Text = Template.LongHelp;
                }

                item[FieldIDs.StandardValues] = Template.StandardValuesItemId;
            }

            foreach (var section in Sections)
            {
                WriteSection(section, inheritedFields);
            }
        }

        protected virtual void WriteSection([NotNull] TemplateSectionWriter templateSectionWriter, [NotNull] IEnumerable<Data.Templates.TemplateField> inheritedFields)
        {
            if (Item == null)
            {
                return;
            }

            var isNew = templateSectionWriter.Item == null;
            if (isNew)
            {
                var id = new ID(templateSectionWriter.TemplateSection.Id);
                templateSectionWriter.Item = Item.Database.AddFromTemplateSynchronized(templateSectionWriter.TemplateSection.Name, new TemplateID(TemplateIDs.TemplateSection), Item, id);
                if (templateSectionWriter.Item == null)
                {
                    throw new EmitException("Could not create section item");
                }
            }

            if (Item != null && templateSectionWriter.Item.ParentID != Item.ID)
            {
                templateSectionWriter.Item.MoveTo(Item);
            }

            using (new EditContext(templateSectionWriter.Item))
            {
                if (templateSectionWriter.Item.Name != templateSectionWriter.TemplateSection.Name)
                {
                    templateSectionWriter.Item.Name = templateSectionWriter.TemplateSection.Name;
                }

                if (!string.IsNullOrEmpty(templateSectionWriter.TemplateSection.Icon))
                {
                    templateSectionWriter.Item.Appearance.Icon = templateSectionWriter.TemplateSection.Icon;
                }
            }

            foreach (var fieldWriter in templateSectionWriter.Fields)
            {
                WriteField(templateSectionWriter, fieldWriter, inheritedFields);
            }
        }

        protected virtual void WriteTemplate([NotNull] IEnumerable<Data.Templates.TemplateField> inheritedFields, [NotNull] string baseTemplates)
        {
            var item = Item;
            if (item == null)
            {
                return;
            }

            // move
            if (!string.Equals(item.Paths.Path, Template.Path, StringComparison.OrdinalIgnoreCase) && !string.Equals(item.ID.ToString(), Template.Path, StringComparison.OrdinalIgnoreCase))
            {
                var parentItemPath = GetItemParentPath(Template.Path);

                var parentItem = item.Database.GetItem(parentItemPath);
                if (parentItem == null)
                {
                    parentItem = item.Database.CreateItemPathSynchronized(parentItemPath);
                    if (parentItem == null)
                    {
                        throw new RetryableEmitException("Could not create item", parentItemPath);
                    }
                }

                item.MoveTo(parentItem);
            }

            // rename item and update fields
            using (new EditContext(item))
            {
                item.Name = Template.Name;
                item[FieldIDs.BaseTemplate] = baseTemplates;
                item.Appearance.Icon = Template.Icon;
                item.Help.ToolTip = Template.ShortHelp;
                item.Help.Text = Template.LongHelp;
                item[FieldIDs.StandardValues] = Template.StandardValuesItemId;
            }

            foreach (var templateSectionWriter in Sections)
            {
                WriteSection(templateSectionWriter, inheritedFields);
            }
        }

        protected virtual string GetItemParentPath(string itemPath)
        {
            var n = itemPath.LastIndexOf('/');
            return n >= 0 ? itemPath.Left(n) : itemPath;
        }
    }
}
