// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections;
using System.Collections.Generic;

namespace Sitecore.Data.Validators
{
    public class ValidatorCollection : IList<BaseValidator>
    {
        IEnumerator<BaseValidator> IEnumerable<BaseValidator>.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public void Add(BaseValidator item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(BaseValidator item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(BaseValidator[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(BaseValidator item)
        {
            throw new System.NotImplementedException();
        }

        public int Count
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new System.NotImplementedException(); }
        }

        public int IndexOf(BaseValidator item)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(int index, BaseValidator item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public BaseValidator this[int index]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}