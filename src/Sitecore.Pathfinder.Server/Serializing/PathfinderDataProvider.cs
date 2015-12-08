// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Globalization;

namespace Sitecore.Pathfinder.Serializing
{
    public class PathfinderDataProvider : DataProvider
    {
        public override bool CreateItem([Diagnostics.NotNull] ID itemID, [Diagnostics.NotNull] string itemName, [Diagnostics.NotNull] ID templateID, [Diagnostics.NotNull] ItemDefinition parent, [Diagnostics.NotNull] CallContext context)
        {
            Sync(itemID);
            return base.CreateItem(itemID, itemName, templateID, parent, context);
        }

        private void Sync(ID itemID)
        {
        }

        public override int AddVersion([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] VersionUri baseVersion, [Diagnostics.NotNull] CallContext context)
        {
            Sync(itemDefinition.ID);
            return base.AddVersion(itemDefinition, baseVersion, context);
        }

        public override bool SaveItem([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] ItemChanges changes, [Diagnostics.NotNull] CallContext context)
        {
            Sync(itemDefinition.ID);
            return base.SaveItem(itemDefinition, changes, context);
        }

        public override bool DeleteItem([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] CallContext context)
        {
            Sync(itemDefinition.ID);
            return base.DeleteItem(itemDefinition, context);
        }

        public override bool RemoveVersion([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] VersionUri version, [Diagnostics.NotNull] CallContext context)
        {
            Sync(itemDefinition.ID);
            return base.RemoveVersion(itemDefinition, version, context);
        }

        public override bool RemoveVersions([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] Language language, [Diagnostics.NotNull] CallContext context)
        {
            Sync(itemDefinition.ID);
            return base.RemoveVersions(itemDefinition, language, context);
        }

        public override bool RemoveVersions([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] Language language, bool removeSharedData, [Diagnostics.NotNull] CallContext context)
        {
            Sync(itemDefinition.ID);
            return base.RemoveVersions(itemDefinition, language, removeSharedData, context);
        }

        public override bool CopyItem([Diagnostics.NotNull] ItemDefinition source, [Diagnostics.NotNull] ItemDefinition destination, [Diagnostics.NotNull] string copyName, [Diagnostics.NotNull] ID copyID, [Diagnostics.NotNull] CallContext context)
        {
            Sync(copyID);
            return base.CopyItem(source, destination, copyName, copyID, context);
        }

        public override bool MoveItem([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] ItemDefinition destination, [Diagnostics.NotNull] CallContext context)
        {
            Sync(itemDefinition.ID);
            return base.MoveItem(itemDefinition, destination, context);
        }

        public override bool ChangeFieldSharing([Diagnostics.NotNull] TemplateField fieldDefinition, TemplateFieldSharing sharing, [Diagnostics.NotNull] CallContext context)
        {
            return base.ChangeFieldSharing(fieldDefinition, sharing, context);
        }

        public override bool ChangeTemplate([Diagnostics.NotNull] ItemDefinition itemDefinition, [Diagnostics.NotNull] TemplateChangeList changes, [Diagnostics.NotNull] CallContext context)
        {
            Sync(itemDefinition.ID);
            return base.ChangeTemplate(itemDefinition, changes, context);
        }
    }
}