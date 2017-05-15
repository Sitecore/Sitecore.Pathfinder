// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Extensions.StringExtensions;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Resources.Media;

namespace Sitecore.Pathfinder.Emitting.Writers
{
    public class ItemWriter
    {
        [NotNull]
        private static readonly object SyncRoot = new object();

        public ItemWriter([NotNull] Parsing.Item item)
        {
            Item = item;
        }

        [NotNull]
        public Parsing.Item Item { get; }

        [NotNull]
        public virtual Item Write()
        {
            var database = Factory.GetDatabase(Item.Database);
            if (database == null)
            {
                throw new EmitException("Database not found", Item.Database);
            }

            var existingItem = database.GetItem(Item.Path);

            Item item;
            Item templateItem;

            // make sure items are only create once
            // todo: hackish!
            lock (SyncRoot)
            {
                item = database.GetItem(new ID(Item.Id));
                templateItem = database.GetItem(Item.Template);
                if (templateItem == null && item != null)
                {
                    templateItem = item.Template;
                }

                if (templateItem == null)
                {
                    throw new RetryableEmitException("Template missing", Item.Template);
                }

                if (item == null)
                {
                    item = CreateNewItem(database, templateItem);
                }
            }

            UpdateItem(item, templateItem);

            if (existingItem != null && existingItem.ID != item.ID)
            {
                foreach (Item child in existingItem.Children)
                {
                    child.MoveTo(item);
                }

                existingItem.Recycle();
            }

            return item;
        }

        [NotNull]
        protected virtual Item CreateNewItem([NotNull] Database database, [NotNull] Item templateItem)
        {
            var parentItemPath = GetItemParentPath(Item.Path);
            if (string.IsNullOrEmpty(parentItemPath))
            {
                throw new EmitException("Parent not found", Item.Path);
            }

            var parentItem = database.CreateItemPathSynchronized(parentItemPath);
            if (parentItem == null)
            {
                throw new RetryableEmitException("Failed to create Item Path", parentItemPath);
            }

            var item = database.AddFromTemplateSynchronized(Item.Name, templateItem.ID, parentItem, new ID(Item.Id));
            if (item == null)
            {
                throw new RetryableEmitException("Failed to create item path", Item.Path);
            }

            Item.Path = item.ID.ToString();

            return item;
        }

        protected virtual string GetItemParentPath(string itemPath)
        {
            var n = itemPath.LastIndexOf('/');
            return n >= 0 ? itemPath.Left(n) : itemPath;
        }

        protected virtual void SetFieldValue([NotNull] Item item, [NotNull] Parsing.Field field, [NotNull] string fieldName)
        {
            var fieldValue = field.Value.Trim();
            var f = item.Fields[fieldName];

            if (!item.Editing.IsEditing)
            {
                item.Editing.BeginEdit();
            }

            if (!string.Equals(f.Type, "layout", StringComparison.OrdinalIgnoreCase))
            {
                f.Value = fieldValue;
            }
            else
            {
                // support layout deltas - may throw a MissingMethod exception on older Sitecore systems
                try
                {
                    SetLayoutFieldValue(f, fieldValue);
                }
                catch
                {
                    f.Value = fieldValue;
                }
            }
        }

        protected virtual void SetLayoutFieldValue([NotNull] Field field, [NotNull] string value)
        {
            // handle layout deltas
            var layoutField = new LayoutField(field);
            layoutField.Value = value;
        }

        protected virtual void UpdateItem([NotNull] Item item, [NotNull] Item templateItem)
        {
            // move
            if (!string.Equals(item.Paths.Path, Item.Path, StringComparison.OrdinalIgnoreCase) && !string.Equals(item.ID.ToString(), Item.Path, StringComparison.OrdinalIgnoreCase))
            {
                var parentItemPath = GetItemParentPath(Item.Path);

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

            // rename and update fields
            using (new EditContext(item))
            {
                if (item.Name != Item.Name)
                {
                    item.Name = Item.Name;
                }

                if (item.TemplateID != templateItem.ID)
                {
                    try
                    {
                        item.ChangeTemplate(new TemplateItem(templateItem));
                    }
                    catch (Exception ex)
                    {
                        throw new RetryableEmitException("Failed to change template of the item", ex.Message);
                    }
                }

                if (item.Appearance.Sortorder != Item.Sortorder)
                {
                    item.Appearance.Sortorder = Item.Sortorder;
                }

                foreach (var field in Item.Fields.Where(i => string.IsNullOrEmpty(i.Language) && i.Version == 0))
                {
                    var fieldName = field.Name;

                    SetFieldValue(item, field, fieldName);
                }
            }

            var versionedItems = new List<Item>();

            foreach (var field in Item.Fields.Where(i => !string.IsNullOrEmpty(i.Language) || i.Version != 0))
            {
                // language has already been validated
                var language = LanguageManager.GetLanguage(field.Language, item.Database);

                var versionedItem = versionedItems.FirstOrDefault(i => i.Language == language && i.Version.Number == field.Version);
                if (versionedItem == null)
                {
                    versionedItem = item.Database.GetItem(item.ID, language, Data.Version.Parse(field.Version));
                    if (versionedItem == null)
                    {
                        // todo: validate this before updating the item
                        throw new RetryableEmitException("Item not found", $"language: {field.Language}, version; {field.Version}");
                    }

                    versionedItems.Add(versionedItem);
                }

                var fieldName = field.Name;

                SetFieldValue(versionedItem, field, fieldName);

                if (!string.IsNullOrEmpty(field.Blob))
                {
                    using (var memoryStream = new MemoryStream(System.Convert.FromBase64String(field.Blob)))
                    {
                        var media = MediaManager.GetMedia(new MediaItem(item));
                        media.MediaData.DataFieldName = field.Name;
                        media.SetStream(memoryStream, field.BlobExtension);
                    }
                }
            }

            foreach (var i in versionedItems)
            {
                if (i.Editing.IsEditing)
                {
                    i.Editing.EndEdit();
                }
            }
        }
    }
}
