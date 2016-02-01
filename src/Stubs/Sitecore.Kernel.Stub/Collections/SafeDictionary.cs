// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sitecore.Collections
{
    public class SafeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        [NotNull, ItemNotNull]
        public IEnumerable<TValue> Values
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
