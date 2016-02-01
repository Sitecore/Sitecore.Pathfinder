// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
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
            throw new NotImplementedException();
        }

        public virtual bool ChangeTemplate(ItemDefinition itemDefinition, TemplateChangeList changes, CallContext context)
        {
            throw new NotImplementedException();
        }

        public virtual bool CopyItem(ItemDefinition source, ItemDefinition destination, string copyName, ID copyID, CallContext context)
        {
            throw new NotImplementedException();
        }

        public virtual bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, CallContext context)
        {
            throw new NotImplementedException();
        }

        public virtual bool DeleteItem(ItemDefinition itemDefinition, CallContext context)
        {
            throw new NotImplementedException();
        }

        public virtual LanguageCollection GetLanguages(CallContext context)
        {
            throw new NotImplementedException();
        }

        public virtual bool MoveItem(ItemDefinition itemDefinition, ItemDefinition destination, CallContext context)
        {
            throw new NotImplementedException();
        }

        public virtual bool RemoveVersion(ItemDefinition itemDefinition, VersionUri version, CallContext context)
        {
            throw new NotImplementedException();
        }

        public virtual bool SaveItem(ItemDefinition itemDefinition, ItemChanges changes, CallContext context)
        {
            throw new NotImplementedException();
        }
    }
}