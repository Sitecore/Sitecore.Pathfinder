// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Items;

namespace Sitecore.Data.Links
{
    public class ItemLink
    {
        [NotNull]
        public ID SourceFieldID
        {
            get { throw new NotImplementedException(); }
        }

        [CanBeNull]
        public Item GetSourceItem()
        {
            throw new NotImplementedException();
        }
    }
}
