// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

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
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Emitters.Writers
{
    public class ItemWriter
    {
        [Diagnostics.NotNull]
        public string DatabaseName { get; set; } = string.Empty;

        [Diagnostics.NotNull, ItemNotNull]
        public ICollection<FieldWriter> Fields { get; } = new List<FieldWriter>();

        public Guid Guid { get; set; } = Guid.Empty;

        [Diagnostics.NotNull]
        public string ItemIdOrPath { get; set; } = string.Empty;

        [Diagnostics.NotNull]
        public string ItemName { get; set; } = string.Empty;

        [Diagnostics.NotNull]
        public ISnapshot Snapshot { get; set; }

        public int Sortorder { get; set; }

        [Diagnostics.NotNull]
        public string TemplateIdOrPath { get; set; } = string.Empty;

        [NotNull]
        private static readonly object SyncRoot = new object();

        [Diagnostics.NotNull]
        public virtual Data.Items.Item Write([Diagnostics.NotNull] IEmitContext context)
        {
            var database = Factory.GetDatabase(DatabaseName);
            if (database == null)
            {
                throw new EmitException(Msg.E1023, Texts.Database_not_found, Snapshot, DatabaseName);
            }

            var existingItem = database.GetItem(ItemIdOrPath);

            Data.Items.Item item;
            Data.Items.Item templateItem;

            // make sure items are only create once
            // todo: hackish!
            lock (SyncRoot)
            {
                item = database.GetItem(new ID(Guid));
                templateItem = database.GetItem(TemplateIdOrPath);
                if (templateItem == null && item != null)
                {
                    templateItem = item.Template;
                }

                if (templateItem == null)
                {
                    throw new RetryableEmitException(Msg.E1024, Texts.Template_missing, Snapshot, TemplateIdOrPath);
                }

                if (item == null)
                {
                    item = CreateNewItem(context, database, templateItem);
                }
            }

            UpdateItem(context, item, templateItem);

            if (existingItem != null && existingItem.ID != item.ID)
            {
                foreach (Data.Items.Item child in existingItem.Children)
                {
                    child.MoveTo(item);
                }

                existingItem.Recycle();
            }

            return item;
        }

        [Diagnostics.NotNull]
        protected virtual Data.Items.Item CreateNewItem([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] Data.Items.Item templateItem)
        {
            var parentItemPath = PathHelper.GetItemParentPath(ItemIdOrPath);
            if (string.IsNullOrEmpty(parentItemPath))
            {
                throw new EmitException(Msg.E1025, Texts.Parent_not_found, Snapshot, ItemIdOrPath);
            }

            var parentItem = database.CreateItemPathSynchronized(parentItemPath);
            if (parentItem == null)
            {
                throw new RetryableEmitException(Msg.E1026, Texts.Failed_to_create_item_path, Snapshot, parentItemPath);
            }

            var item = database.AddFromTemplateSynchronized(ItemName, templateItem.ID, parentItem, new ID(Guid));
            if (item == null)
            {
                throw new RetryableEmitException(Msg.E1027, Texts.Failed_to_create_item_path, Snapshot, ItemIdOrPath);
            }

            ItemIdOrPath = item.ID.ToString();

            return item;
        }

        protected virtual void SetFieldValue([NotNull] IEmitContext context, [Diagnostics.NotNull] Data.Items.Item item, [Diagnostics.NotNull] FieldWriter fieldWriter, [NotNull] string fieldName)
        {
            var fieldValue = fieldWriter.DatabaseValue.Trim();
            var field = item.Fields[fieldName];

            var trackingProjectEmitter = context.ProjectEmitter as ITrackingProjectEmitter;
            if (trackingProjectEmitter != null)
            {
                if (!trackingProjectEmitter.CanSetFieldValue(item, fieldWriter, fieldValue))
                {
                    return;
                }
            }

            if (!item.Editing.IsEditing)
            {
                item.Editing.BeginEdit();
            }

            if (!string.Equals(field.Type, "layout", StringComparison.OrdinalIgnoreCase))
            {
                field.Value = fieldValue;
            }
            else
            {
                // support layout deltas - may throw a MissingMethod exception on older Sitecore systems
                try
                {
                    SetLayoutFieldValue(field, fieldValue);
                }
                catch
                {
                    field.Value = fieldValue;
                }
            }
        }

        protected virtual void SetLayoutFieldValue([Diagnostics.NotNull] Data.Fields.Field field, [Diagnostics.NotNull] string value)
        {
            // handle layout deltas
            var layoutField = new LayoutField(field);
            layoutField.Value = value;
        }

        protected virtual void UpdateItem([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] Data.Items.Item item, [Diagnostics.NotNull] Data.Items.Item templateItem)
        {
            // move
            if (!string.Equals(item.Paths.Path, ItemIdOrPath, StringComparison.OrdinalIgnoreCase) && !string.Equals(item.ID.ToString(), ItemIdOrPath, StringComparison.OrdinalIgnoreCase))
            {
                var parentItemPath = PathHelper.GetItemParentPath(ItemIdOrPath);

                var parentItem = item.Database.GetItem(parentItemPath);
                if (parentItem == null)
                {
                    parentItem = item.Database.CreateItemPathSynchronized(parentItemPath);
                    if (parentItem == null)
                    {
                        throw new RetryableEmitException(Msg.E1028, Texts.Could_not_create_item, Snapshot, parentItemPath);
                    }
                }

                item.MoveTo(parentItem);
            }

            // rename and update fields
            using (new EditContext(item))
            {
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
                        throw new RetryableEmitException(Msg.E1029, Texts.Failed_to_change_template_of_the_item, Snapshot, ex.Message);
                    }
                }

                if (item.Appearance.Sortorder!= Sortorder)
                {
                    item.Appearance.Sortorder = Sortorder;
                }

                foreach (var fieldWriter in Fields.Where(i => i.Language == Language.Undefined && i.Version == Projects.Items.Version.Undefined))
                {
                    var fieldName = fieldWriter.FieldName;
                    if (string.IsNullOrEmpty(fieldName))
                    {
                        fieldName = fieldWriter.FieldId.Format();
                    }

                    SetFieldValue(context, item, fieldWriter, fieldName);
                }

                item.UpdateProjectUniqueIds(context);
            }

            var versionedItems = new List<Data.Items.Item>();

            foreach (var field in Fields.Where(i => i.Language != Language.Undefined || i.Version != Projects.Items.Version.Undefined))
            {
                // language has already been validated
                var language = LanguageManager.GetLanguage(field.Language.LanguageName, item.Database);

                var versionedItem = versionedItems.FirstOrDefault(i => i.Language == language && i.Version.Number == field.Version.Number);
                if (versionedItem == null)
                {
                    versionedItem = item.Database.GetItem(item.ID, language, Data.Version.Parse(field.Version.Number));
                    if (versionedItem == null)
                    {
                        // todo: validate this before updating the item
                        context.Trace.TraceError(Msg.E1006, Texts.Item_not_found, TraceHelper.GetTextNode(field.FieldNameProperty), $"language: {field.Language}, version; {field.Version}");
                        continue;
                    }

                    versionedItems.Add(versionedItem);
                }

                var fieldName = field.FieldName;
                if (string.IsNullOrEmpty(fieldName))
                {
                    fieldName = field.FieldId.Format();
                }

                SetFieldValue(context, versionedItem, field, fieldName);
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
