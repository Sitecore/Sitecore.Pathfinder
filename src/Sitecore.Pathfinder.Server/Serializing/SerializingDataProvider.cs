// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Serializing
{
    public class SerializingDataProvider : DataProvider
    {
        private static int _disabled;

        public static bool Disabled
        {
            get { return _disabled != 0; }
            set
            {
                if (value)
                {
                    _disabled++;
                }
                else
                {
                    _disabled--;
                    if (_disabled < 0)
                    {
                        throw new InvalidOperationException("Calls to Disabled are not balanced");
                    }
                }
            }
        }

        public override int AddVersion([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] VersionUri baseVersion, [Diagnostics.NotNull] CallContext context)
        {
            SerializeItem(itemDefinition.ID);
            return base.AddVersion(itemDefinition, baseVersion, context);
        }

        public override bool ChangeTemplate([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] TemplateChangeList changes, [Diagnostics.NotNull] CallContext context)
        {
            SerializeItem(itemDefinition.ID);
            return base.ChangeTemplate(itemDefinition, changes, context);
        }

        public override bool CopyItem([Diagnostics.NotNull] ItemDefinition source, [Diagnostics.NotNull] ItemDefinition destination, [Diagnostics.NotNull] string copyName, [Diagnostics.NotNull] Data.ID copyID, [Diagnostics.NotNull] CallContext context)
        {
            SerializeItem(copyID);
            return base.CopyItem(source, destination, copyName, copyID, context);
        }

        public override bool CreateItem([Diagnostics.NotNull] Data.ID itemID, [Diagnostics.NotNull] string itemName, [Diagnostics.NotNull] Data.ID templateID, [Diagnostics.NotNull] ItemDefinition parent, [Diagnostics.NotNull] CallContext context)
        {
            SerializeItem(itemID);
            return base.CreateItem(itemID, itemName, templateID, parent, context);
        }

        public override bool DeleteItem([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] CallContext context)
        {
            RemoveItem(itemDefinition.ID);
            return base.DeleteItem(itemDefinition, context);
        }

        [Diagnostics.CanBeNull, ItemNotNull]
        public override LanguageCollection GetLanguages([Diagnostics.NotNull] CallContext context)
        {
            // avoid returning default languages
            return null;
        }

        public override bool MoveItem([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] ItemDefinition destination, [Diagnostics.NotNull] CallContext context)
        {
            RemoveItem(itemDefinition.ID);
            SerializeItem(itemDefinition.ID, destination.ID);
            return base.MoveItem(itemDefinition, destination, context);
        }

        public override bool RemoveVersion([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] VersionUri version, [Diagnostics.NotNull] CallContext context)
        {
            SerializeItem(itemDefinition.ID);
            return base.RemoveVersion(itemDefinition, version, context);
        }

        public override bool SaveItem([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] ItemChanges changes, [Diagnostics.NotNull] CallContext context)
        {
            if (changes.Renamed)
            {
                PropertyChange nameChange;
                if (changes.Properties.TryGetValue("name", out nameChange))
                {
                    var oldItemName = nameChange.OriginalValue as string ?? string.Empty;
                    if (!string.IsNullOrEmpty(oldItemName))
                    {
                        RemoveItem(itemDefinition.ID, oldItemName);
                    }
                }
            }

            var hasChanges = changes.Renamed || changes.HasPropertiesChanged || changes.FieldChanges.OfType<FieldChange>().Any(fieldChange => fieldChange.FieldID != FieldIDs.Revision && fieldChange.FieldID != FieldIDs.Updated && fieldChange.FieldID != FieldIDs.UpdatedBy && fieldChange.FieldID != FieldIDs.Created && fieldChange.FieldID != FieldIDs.CreatedBy && fieldChange.FieldID != FieldIDs.Originator);
            if (hasChanges)
            {
                SerializeItem(itemDefinition.ID);
            }

            return base.SaveItem(itemDefinition, changes, context);
        }

        protected virtual void RemoveItem([Diagnostics.NotNull] Data.ID itemID)
        {
            if (Disabled)
            {
                return;
            }

            foreach (var project in ProjectHost.Projects)
            {
                try
                {
                    project.WebsiteSerializer.RemoveItem(Database.Name, itemID);
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to remove item", ex, typeof(SerializingDataProvider));
                }
            }
        }

        protected virtual void RemoveItem([Diagnostics.NotNull] Data.ID itemID, [Diagnostics.NotNull] string oldItemName)
        {
            if (Disabled)
            {
                return;
            }

            foreach (var project in ProjectHost.Projects)
            {
                try
                {
                    project.WebsiteSerializer.RemoveItem(Database.Name, itemID, oldItemName);
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to remove item", ex, typeof(SerializingDataProvider));
                }
            }
        }

        protected virtual void SerializeItem([Diagnostics.NotNull] Data.ID itemID)
        {
            if (Disabled)
            {
                return;
            }

            foreach (var project in ProjectHost.Projects)
            {
                try
                {
                    project.WebsiteSerializer.SerializeItem(Database.Name, itemID);
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to serialize item", ex, typeof(SerializingDataProvider));
                }
            }
        }

        protected virtual void SerializeItem([Diagnostics.NotNull] Data.ID itemID, [Diagnostics.NotNull] Data.ID newParentId)
        {
            if (Disabled)
            {
                return;
            }

            foreach (var project in ProjectHost.Projects)
            {
                try
                {
                    project.WebsiteSerializer.SerializeItem(Database.Name, itemID, newParentId);
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to serialize item", ex, typeof(SerializingDataProvider));
                }
            }
        }
    }
}
