// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;

namespace Sitecore.Data.DataProviders
{
    public class DataProvider
    {
        public Database Database { get; private set; }

        public virtual int AddVersion(ItemDefinition itemDefinition, VersionUri baseVersion, CallContext context)
        {
            return 0;
        }

        public virtual bool ChangeTemplate(ItemDefinition itemDefinition, TemplateChangeList changes, CallContext context)
        {
            return false;
        }

        public virtual bool CopyItem(ItemDefinition source, ItemDefinition destination, string copyName, ID copyID, CallContext context)
        {
            return false;
        }

        public virtual bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, CallContext context)
        {
            return false;
        }

        public virtual bool DeleteItem(ItemDefinition itemDefinition, CallContext context)
        {
            return false;
        }

        public virtual LanguageCollection GetLanguages(CallContext context)
        {
            return null;
        }

        public virtual bool MoveItem(ItemDefinition itemDefinition, ItemDefinition destination, CallContext context)
        {
            return false;
        }

        public virtual bool RemoveVersion(ItemDefinition itemDefinition, VersionUri version, CallContext context)
        {
            return false;
        }

        public virtual bool SaveItem(ItemDefinition itemDefinition, ItemChanges changes, CallContext context)
        {
            return false;
        }
    }
}