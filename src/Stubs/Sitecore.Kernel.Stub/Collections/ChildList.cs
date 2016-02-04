// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.Collections
{
    public class ChildList : IEnumerable<Item>, ICollection
    {
        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        [CanBeNull]
        public Item this[[NotNull] string itemName]
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator<Item> IEnumerable<Item>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        [NotNull]               public IEnumerator GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
