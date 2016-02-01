// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;

namespace Sitecore.Data.Items
{
    public class ItemChanges
    {
        [NotNull]
        public FieldChangeList FieldChanges
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasPropertiesChanged
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public Dictionary<string, PropertyChange> Properties
        {
            get { throw new NotImplementedException(); }
        }

        public bool Renamed
        {
            get { throw new NotImplementedException(); }
        }
    }
}
