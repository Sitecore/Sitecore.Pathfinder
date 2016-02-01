// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Data.Items
{
    public abstract class CustomBaseItem
    {
        protected CustomBaseItem([NotNull] Item innerItem)
        {
            InnerItem = innerItem;
        }

        [NotNull]
        public Database Database
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public ID ID
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public Item InnerItem
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Name
        {
            get { throw new NotImplementedException(); }
        }
    }
}
