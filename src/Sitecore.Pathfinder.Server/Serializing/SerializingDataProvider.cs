// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Serializing
{
    public class SerializingDataProvider : DataProvider
    {
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

        public override bool CopyItem([Diagnostics.NotNull] ItemDefinition source, [Diagnostics.NotNull] ItemDefinition destination, [Diagnostics.NotNull] string copyName, [Diagnostics.NotNull] ID copyID, [Diagnostics.NotNull] CallContext context)
        {
            SerializeItem(copyID);
            return base.CopyItem(source, destination, copyName, copyID, context);
        }

        public override bool CreateItem([Diagnostics.NotNull] ID itemID, [Diagnostics.NotNull] string itemName, [Diagnostics.NotNull] ID templateID, [Diagnostics.NotNull] ItemDefinition parent, [Diagnostics.NotNull] CallContext context)
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

        protected virtual void RemoveItem([Diagnostics.NotNull] ID itemID)
        {
            SerializingDataProviderService.RemoveItem(Database.Name, itemID);
        }

        protected virtual void RemoveItem([Diagnostics.NotNull] ID itemID, [Diagnostics.NotNull] string oldItemName)
        {
            SerializingDataProviderService.RemoveItem(Database.Name, itemID, oldItemName);
        }

        protected virtual void SerializeItem([Diagnostics.NotNull] ID itemID)
        {
            SerializingDataProviderService.SerializeItem(Database.Name, itemID);
        }

        protected virtual void SerializeItem([Diagnostics.NotNull] ID itemID, [Diagnostics.NotNull] ID newParentId)
        {
            SerializingDataProviderService.SerializeItem(Database.Name, itemID, newParentId);
        }
    }
}
