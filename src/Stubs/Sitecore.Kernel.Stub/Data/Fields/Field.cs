// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Sitecore.Data.Fields
{
    public class Field
    {
        public bool ContainsStandardValue
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public ID ID
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public Item Item
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [NotNull]
        public Language Language
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public bool Shared
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Type
        {
            get { throw new NotImplementedException(); }
        }

        public bool Unversioned
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Value
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
