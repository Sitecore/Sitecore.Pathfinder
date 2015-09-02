// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Builders.Templates
{
    public class TemplateBuilder
    {
        [Diagnostics.CanBeNull]
        private IEnumerable<TemplateSectionBuilder> _sectionBuilders;

        public TemplateBuilder([Diagnostics.NotNull] Template template)
        {
            Template = template;
        }

        [Diagnostics.CanBeNull]
        public Item Item { get; set; }

        [Diagnostics.NotNull]
        public IEnumerable<TemplateSectionBuilder> Sections
        {
            get
            {
                return _sectionBuilders ?? (_sectionBuilders = Template.Sections.Select(s => new TemplateSectionBuilder(s)).ToList());
            }
        }

        [Diagnostics.NotNull]
        public Template Template { get; }

        [Diagnostics.CanBeNull]
        public Item Build([Diagnostics.NotNull] IEmitContext context)
        {
            var inheritedFields = GetInheritedFields(Template);

            if (Item == null)
            {
                ResolveItem(context);
            }

            if (Item == null)
            {
                CreateNewTemplate(context, inheritedFields);
                if (Item == null)
                {
                    return null;
                }

                context.RegisterAddedItem(Item);
            }
            else
            {
                UpdateTemplate(context, inheritedFields);
                DeleteSections(context);
            }

            SortSections(context, this);

            return Item;
        }

        protected virtual void CreateNewTemplate([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] IEnumerable<Sitecore.Data.Templates.TemplateField> inheritedFields)
        {
            var database = context.DataService.GetDatabase(Template.DatabaseName);
            if (database == null)
            {
                return;
            }

            var parentItem = GetParentItem(context, database);
            if (parentItem == null)
            {
                throw new RetryableEmitException(Texts.Failed_to_create_template, Template.Snapshots.First());
            }

            var item = ItemManager.AddFromTemplate(Template.ItemName, new TemplateID(TemplateIDs.Template), parentItem, new ID(Template.Guid));
            if (item == null)
            {
                throw new EmitException(Texts.Failed_to_add_new_template, Template.Snapshots.First());
            }

            Item = item;
            using (new EditContext(item))
            {
                if (!string.IsNullOrEmpty(Template.BaseTemplates))
                {
                    item[FieldIDs.BaseTemplate] = Template.BaseTemplates;
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

                item[FieldIDs.StandardValues] = Template.StandardValuesItem?.Guid.Format() ?? string.Empty;
            }

            foreach (var section in Sections)
            {
                UpdateSection(context, section, inheritedFields);
            }
        }

        protected virtual void DeleteFields([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] TemplateSectionBuilder templateSectionBuilder)
        {
            if (templateSectionBuilder.Item == null)
            {
                return;
            }

            foreach (Item child in templateSectionBuilder.Item.Children)
            {
                if (child.TemplateID != TemplateIDs.TemplateField)
                {
                    continue;
                }

                var found = false;

                foreach (var field in templateSectionBuilder.Fields)
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
                    context.RegisterDeletedItem(child);
                    child.Recycle();
                }
            }
        }

        protected virtual void DeleteSections([Diagnostics.NotNull] IEmitContext context)
        {
            if (Item == null)
            {
                return;
            }

            foreach (Item child in Item.Children)
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
                        DeleteFields(context, section);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    context.RegisterDeletedItem(child);
                    child.Recycle();
                }
            }
        }

        [Diagnostics.NotNull]
        protected IEnumerable<Sitecore.Data.Templates.TemplateField> GetInheritedFields([Diagnostics.NotNull] Template template)
        {
            var fields = new List<Sitecore.Data.Templates.TemplateField>();

            var database = Factory.GetDatabase(template.DatabaseName);
            var baseTemplates = new List<Item>();

            var templates = template.BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);
            foreach (var templateId in templates)
            {
                // resolve possible item paths
                var baseTemplateItem = database.GetItem(templateId);
                if (baseTemplateItem == null)
                {
                    throw new RetryableEmitException(Texts.Base_Template_missing, template.Snapshots.First(), templateId);
                }

                baseTemplates.Add(baseTemplateItem);

                var t = TemplateManager.GetTemplate(baseTemplateItem.ID, database);
                if (t == null)
                {
                    throw new RetryableEmitException(Texts.Template_missing, template.Snapshots.First(), templateId);
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

            // todo: hmm
            template.BaseTemplates = string.Join("|", baseTemplates.Select(t => t.ID.ToString()));

            return fields;
        }

        [Diagnostics.CanBeNull]
        protected virtual Item GetParentItem([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] Database database)
        {
            var parentPath = PathHelper.GetItemParentPath(Template.ItemIdOrPath);
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

        protected virtual void ResolveItem([Diagnostics.NotNull] IEmitContext context)
        {
            var database = context.DataService.GetDatabase(Template.DatabaseName);
            if (database == null)
            {
                return;
            }

            Item = database.GetItem(new ID(Template.Guid));
            if (Item == null)
            {
                return;
            }

            foreach (var section in Sections)
            {
                section.ResolveItem(context, Item);
            }
        }

        protected virtual void SortFields([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] TemplateSectionBuilder templateSectionBuilder)
        {
            var lastSortorder = 0;

            var fields = templateSectionBuilder.Fields.ToList();
            for (var index = 0; index < fields.Count(); index++)
            {
                var field = fields.ElementAt(index);
                if (field.Item == null)
                {
                    continue;
                }

                var sortorder = field.Item.Appearance.Sortorder;

                if (sortorder <= lastSortorder)
                {
                    var nextSortorder = lastSortorder + 200;

                    if (index < fields.Count() - 1)
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

                    sortorder = lastSortorder + ((nextSortorder - lastSortorder) / 2);

                    context.RegisterUpdatedItem(field.Item);

                    using (new EditContext(field.Item))
                    {
                        field.Item.Editing.EndEdit();
                    }
                }

                lastSortorder = sortorder;
            }
        }

        protected virtual void SortSections([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] TemplateBuilder templateBuilder)
        {
            var lastSortorder = 0;

            var sections = templateBuilder.Sections.ToList();
            for (var index = 0; index < sections.Count(); index++)
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

                    if (index < sections.Count() - 1)
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

                    sortorder = lastSortorder + ((nextSortorder - lastSortorder) / 2);

                    context.RegisterUpdatedItem(section.Item);

                    using (new EditContext(section.Item))
                    {
                        section.Item.Appearance.Sortorder = sortorder;
                    }
                }

                SortFields(context, section);

                lastSortorder = sortorder;
            }
        }

        protected virtual void UpdateField([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] TemplateSectionBuilder templateSectionBuilder, [Diagnostics.NotNull] TemplateFieldBuilder templateFieldBuilder, [Diagnostics.NotNull] IEnumerable<Sitecore.Data.Templates.TemplateField> inheritedFields)
        {
            if (inheritedFields.Any(f => string.Compare(f.Name, templateFieldBuilder.TemplateField.FieldName, StringComparison.OrdinalIgnoreCase) == 0))
            {
                return;
            }

            var item = templateFieldBuilder.Item;

            var isNew = item == null;
            if (isNew)
            {
                item = ItemManager.AddFromTemplate(templateFieldBuilder.TemplateField.FieldName, new TemplateID(TemplateIDs.TemplateField), templateSectionBuilder.Item);
                if (item == null)
                {
                    throw new EmitException(Texts.Could_not_create_template_field, templateFieldBuilder.TemplateField.FieldNameProperty.SourceTextNode ?? TextNode.Empty, templateFieldBuilder.TemplateField.FieldName);
                }

                templateFieldBuilder.Item = item;
            }
            else
            {
                context.RegisterUpdatedItem(item);
            }

            if (templateSectionBuilder.Item != null && item.ParentID != templateSectionBuilder.Item.ID)
            {
                item.MoveTo(templateSectionBuilder.Item);
            }

            using (new EditContext(item))
            {
                if (!string.IsNullOrEmpty(templateFieldBuilder.TemplateField.FieldName))
                {
                    item.Name = templateFieldBuilder.TemplateField.FieldName;
                }

                if (!string.IsNullOrEmpty(templateFieldBuilder.TemplateField.Type))
                {
                    item["Type"] = templateFieldBuilder.TemplateField.Type;
                }

                item["Shared"] = templateFieldBuilder.TemplateField.Shared ? "1" : string.Empty;
                item["Unversioned"] = templateFieldBuilder.TemplateField.Unversioned ? "1" : string.Empty;

                if (!string.IsNullOrEmpty(templateFieldBuilder.TemplateField.Source))
                {
                    item["Source"] = templateFieldBuilder.TemplateField.Source;
                }

                if (!string.IsNullOrEmpty(templateFieldBuilder.TemplateField.ShortHelp))
                {
                    item.Help.ToolTip = templateFieldBuilder.TemplateField.ShortHelp;
                }

                if (!string.IsNullOrEmpty(templateFieldBuilder.TemplateField.LongHelp))
                {
                    item.Help.Text = templateFieldBuilder.TemplateField.LongHelp;
                }

                item.Appearance.Sortorder = templateFieldBuilder.TemplateField.SortOrder;
            }

            if (isNew)
            {
                context.RegisterAddedItem(item);
            }
        }

        protected virtual void UpdateSection([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] TemplateSectionBuilder templateSectionBuilder, [Diagnostics.NotNull] IEnumerable<Sitecore.Data.Templates.TemplateField> inheritedFields)
        {
            if (Item == null)
            {
                return;
            }

            var isNew = templateSectionBuilder.Item == null;
            if (isNew)
            {
                templateSectionBuilder.Item = ItemManager.AddFromTemplate(templateSectionBuilder.TemplateSection.SectionName, new TemplateID(TemplateIDs.TemplateSection), Item);
                if (templateSectionBuilder.Item == null)
                {
                    throw new EmitException(Texts.Could_not_create_section_item, Template.ItemNameProperty.SourceTextNode ?? TextNode.Empty);
                }
            }
            else
            {
                context.RegisterUpdatedItem(templateSectionBuilder.Item);
            }

            if (Item != null && templateSectionBuilder.Item.ParentID != Item.ID)
            {
                templateSectionBuilder.Item.MoveTo(Item);
            }

            using (new EditContext(templateSectionBuilder.Item))
            {
                if (templateSectionBuilder.Item.Name != templateSectionBuilder.TemplateSection.SectionName)
                {
                    templateSectionBuilder.Item.Name = templateSectionBuilder.TemplateSection.SectionName;
                }

                if (!string.IsNullOrEmpty(templateSectionBuilder.TemplateSection.Icon))
                {
                    templateSectionBuilder.Item.Appearance.Icon = templateSectionBuilder.TemplateSection.Icon;
                }
            }

            foreach (var field in templateSectionBuilder.Fields)
            {
                UpdateField(context, templateSectionBuilder, field, inheritedFields);
            }

            if (isNew)
            {
                context.RegisterAddedItem(templateSectionBuilder.Item);
            }
        }

        protected virtual void UpdateTemplate([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] IEnumerable<Sitecore.Data.Templates.TemplateField> inheritedFields)
        {
            var item = Item;
            if (item == null)
            {
                return;
            }

            context.RegisterUpdatedItem(item);

            // move
            if (string.Compare(item.Paths.Path, Template.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(item.ID.ToString(), Template.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0)
            {
                var parentItemPath = PathHelper.GetItemParentPath(Template.ItemIdOrPath);

                var parentItem = item.Database.GetItem(parentItemPath);
                if (parentItem == null)
                {
                    parentItem = item.Database.CreateItemPath(parentItemPath);
                    if (parentItem == null)
                    {
                        throw new RetryableEmitException(Texts.Could_not_create_item, Template.Snapshots.First(), parentItemPath);
                    }
                }

                item.MoveTo(parentItem);
            }

            // rename item and update fields
            using (new EditContext(item))
            {
                item.Name = Template.ItemName;
                item[FieldIDs.BaseTemplate] = Template.BaseTemplates;
                item.Appearance.Icon = Template.Icon;
                item.Help.ToolTip = Template.ShortHelp;
                item.Help.Text = Template.LongHelp;
                item[FieldIDs.StandardValues] = Template.StandardValuesItem?.Guid.Format() ?? string.Empty;
            }

            foreach (var templateSectionBuilder in Sections)
            {
                UpdateSection(context, templateSectionBuilder, inheritedFields);
            }
        }
    }
}
