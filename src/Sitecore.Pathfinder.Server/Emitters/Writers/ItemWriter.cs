// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Emitters.Writers
{
    public class ItemWriter
    {
        [Diagnostics.NotNull]
        public string DatabaseName { get; set; } = string.Empty;

        [Diagnostics.NotNull]
        [ItemNotNull]
        public ICollection<FieldWriter> Fields { get; } = new List<FieldWriter>();

        public Guid Guid { get; set; } = Guid.Empty;

        [Diagnostics.NotNull]
        public string ItemIdOrPath { get; set; } = string.Empty;

        [Diagnostics.NotNull]
        public string ItemName { get; set; } = string.Empty;

        [Diagnostics.NotNull]
        public ISnapshot Snapshot { get; set; }

        [Diagnostics.NotNull]
        public string TemplateIdOrPath { get; set; } = string.Empty;

        public virtual void Write([Diagnostics.NotNull] IEmitContext context)
        {
            var database = Factory.GetDatabase(DatabaseName);
            if (database == null)
            {
                throw new EmitException(Texts.Database_not_found, Snapshot, DatabaseName);
            }

            var item = database.GetItem(new ID(Guid));
            var existingItem = database.GetItem(ItemIdOrPath);

            var templateItem = database.GetItem(TemplateIdOrPath);
            if (templateItem == null && item != null)
            {
                templateItem = item.Template;
            }

            if (templateItem == null)
            {
                throw new RetryableEmitException(Texts.Template_missing, Snapshot, TemplateIdOrPath);
            }

            if (item == null)
            {
                item = CreateNewItem(context, database, templateItem);
            }

            UpdateItem(context, item, templateItem);

            if (existingItem != null && existingItem.ID != item.ID)
            {
                foreach (Item child in existingItem.Children)
                {
                    child.MoveTo(item);
                }

                existingItem.Recycle();
            }
        }

        [Diagnostics.NotNull]
        protected virtual Item CreateNewItem([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] Item templateItem)
        {
            var parentItemPath = PathHelper.GetItemParentPath(ItemIdOrPath);
            if (string.IsNullOrEmpty(parentItemPath))
            {
                throw new EmitException("Parent not found", Snapshot, ItemIdOrPath);
            }

            var parentItem = database.CreateItemPath(parentItemPath);
            if (parentItem == null)
            {
                throw new RetryableEmitException(Texts.Failed_to_create_item_path, Snapshot, parentItemPath);
            }

            var item = ItemManager.AddFromTemplate(ItemName, templateItem.ID, parentItem, new ID(Guid));
            if (item == null)
            {
                throw new RetryableEmitException(Texts.Failed_to_create_item_path, Snapshot, ItemIdOrPath);
            }

            ItemIdOrPath = item.ID.ToString();

            return item;
        }

        protected virtual void SetFieldValue([Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string fieldName, [Diagnostics.NotNull] string fieldValue)
        {
            var field = item.Fields[fieldName];

            if (!string.Equals(field.Type, "layout", StringComparison.OrdinalIgnoreCase))
            {
                field.Value = fieldValue.Trim();
                return;
            }

            // support layout deltas - may throw a MissingMethod exception on older Sitecore systems
            try
            {
                SetLayoutFieldValue(field, fieldValue);
            }
            catch
            {
                field.Value = fieldValue.Trim();
            }
        }

        protected virtual void SetLayoutFieldValue([Diagnostics.NotNull] Field field, [Diagnostics.NotNull] string value)
        {
            // handle layout deltas
            var layoutField = new LayoutField(field);
            layoutField.Value = value;
        }

        protected virtual void UpdateItem([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] Item templateItem)
        {
            // move
            if (!string.Equals(item.Paths.Path, ItemIdOrPath, StringComparison.OrdinalIgnoreCase) && !string.Equals(item.ID.ToString(), ItemIdOrPath, StringComparison.OrdinalIgnoreCase))
            {
                var parentItemPath = PathHelper.GetItemParentPath(ItemIdOrPath);

                var parentItem = item.Database.GetItem(parentItemPath);
                if (parentItem == null)
                {
                    parentItem = item.Database.CreateItemPath(parentItemPath);
                    if (parentItem == null)
                    {
                        throw new RetryableEmitException(Texts.Could_not_create_item, Snapshot, parentItemPath);
                    }
                }

                item.MoveTo(parentItem);
            }

            // rename and update fields
            using (new EditContext(item))
            {
                item.Fields.ReadAll();

                if (item.Name != ItemName)
                {
                    item.Name = ItemName;
                }

                if (item.TemplateID != templateItem.ID)
                {
                    try
                    {
                        item.ChangeTemplate(new TemplateItem(templateItem));
                    }
                    catch (Exception ex)
                    {
                        throw new RetryableEmitException(Texts.Failed_to_change_template_of_the_item, Snapshot, ex.Message);
                    }
                }

                foreach (var field in Fields.Where(i => string.IsNullOrEmpty(i.Language) && i.Version == 0))
                {
                    var fieldName = field.FieldName;
                    if (string.IsNullOrEmpty(fieldName))
                    {
                        fieldName = field.FieldId.Format();
                    }

                    SetFieldValue(item, fieldName, field.DatabaseValue);
                }
            }

            var versionedItems = new List<Item>();

            foreach (var field in Fields.Where(i => !string.IsNullOrEmpty(i.Language) || i.Version != 0))
            {
                // language has already been validated
                var language = LanguageManager.GetLanguage(field.Language, item.Database);

                var versionedItem = versionedItems.FirstOrDefault(i => i.Language == language && i.Version.Number == field.Version);
                if (versionedItem == null)
                {
                    versionedItem = item.Database.GetItem(item.ID, language, new Data.Version(field.Version));
                    if (versionedItem == null)
                    {
                        // todo: validate this before updating the item
                        context.Trace.TraceError(Msg.E1006, Texts.Item_not_found, TraceHelper.GetTextNode(field.FieldNameProperty), $"language: {field.Language}, version; {field.Version}");
                        continue;
                    }

                    versionedItem.Editing.BeginEdit();
                    versionedItems.Add(versionedItem);
                    versionedItem.Fields.ReadAll();
                }

                var fieldName = field.FieldName;
                if (string.IsNullOrEmpty(fieldName))
                {
                    fieldName = field.FieldId.Format();
                }
                                                      
                SetFieldValue(versionedItem, fieldName, field.DatabaseValue);
            }

            foreach (var i in versionedItems)
            {
                i.Editing.EndEdit();
            }
        }
    }
}
