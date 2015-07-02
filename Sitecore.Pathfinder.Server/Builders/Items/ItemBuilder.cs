// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Builders.Items
{
    public class ItemBuilder
    {
        [Diagnostics.NotNull]
        public string DatabaseName { get; set; } = string.Empty;

        [Diagnostics.NotNull]
        public ICollection<FieldBuilder> Fields { get; } = new List<FieldBuilder>();

        public Guid Guid { get; set; } = Guid.Empty;

        [Diagnostics.NotNull]
        public string ItemIdOrPath { get; set; } = string.Empty;

        [Diagnostics.NotNull]
        public string ItemName { get; set; } = string.Empty;

        [Diagnostics.NotNull]
        public ISnapshot Snapshot { get; set; }

        [Diagnostics.NotNull]
        public string TemplateIdOrPath { get; set; } = string.Empty;

        public virtual void Build([Diagnostics.NotNull] IEmitContext context)
        {
            var database = context.DataService.GetDatabase(DatabaseName);
            if (database == null)
            {
                throw new EmitException(Texts.Database_not_found, Snapshot, DatabaseName);
            }

            var item = database.GetItem(new ID(Guid));
            if (item != null)
            {
                ItemIdOrPath = item.Paths.Path;
            }

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
                context.RegisterAddedItem(item);
            }
            else
            {
                context.RegisterUpdatedItem(item);
            }

            UpdateItem(context, item, templateItem);
        }

        [Diagnostics.NotNull]
        protected virtual Item CreateNewItem([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] Item templateItem)
        {
            var parentItemPath = PathHelper.GetItemParentPath(ItemIdOrPath);

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

            if (string.Compare(field.Type, "layout", StringComparison.OrdinalIgnoreCase) != 0)
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
            if (string.Compare(item.Paths.Path, ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(item.ID.ToString(), ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0)
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
                    item.ChangeTemplate(new TemplateItem(templateItem));
                }

                foreach (var field in Fields.Where(i => string.IsNullOrEmpty(i.Language) && i.Version == 0))
                {
                    SetFieldValue(item, field.FieldName.Value, field.Value);
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
                    versionedItem = item.Database.GetItem(item.ID, language, new Sitecore.Data.Version(field.Version));
                    if (versionedItem == null)
                    {
                        // todo: validate this before updating the item
                        context.Trace.TraceError(Texts.Item_not_found, field.FieldName.Source ?? TextNode.Empty, $"language: {field.Language}, version; {field.Version}");
                        continue;
                    }

                    versionedItem.Editing.BeginEdit();
                    versionedItems.Add(versionedItem);
                    versionedItem.Fields.ReadAll();
                }

                SetFieldValue(versionedItem, field.FieldName.Value, field.Value);
            }

            foreach (var i in versionedItems)
            {
                i.Editing.EndEdit();
            }
        }
    }
}
