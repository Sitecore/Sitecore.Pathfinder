// © 2015-2017 by Jakob Christensen. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Projects
{
    public class FilteredLockableList<T> : IList<T> where T : class
    {
        [NotNull]
        private readonly object _sync = new object();

        public FilteredLockableList([NotNull] ILockable owner)
        {
            Owner = owner;
        }

        public int Count => FilteredList.Count();

        public bool IsReadOnly => Owner.Locking == Locking.ReadOnly;

        [NotNull]
        public T this[int index]
        {
            get
            {
                lock (_sync)
                {
                    return FilteredList.ElementAt(index);
                }
            }
            set => throw new InvalidOperationException("Cannot assign to FilteredLockableList using index");
        }

        [ItemNotNull, NotNull]
        protected virtual IEnumerable<T> FilteredList => List;

        [NotNull, ItemNotNull]
        protected List<T> List { get; } = new List<T>();

        [NotNull]
        private ILockable Owner { get; }

        public void Add([NotNull] T item)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("List is locked");
            }

            lock (_sync)
            {
                List.Add(item);
            }
        }

        public void Clear()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("List is locked");
            }

            lock (_sync)
            {
                List.Clear();
            }
        }

        public bool Contains([NotNull] T item)
        {
            lock (_sync)
            {
                return FilteredList.Contains(item);
            }
        }

        public void CopyTo([ItemNotNull] T[] array, int arrayIndex)
        {
            throw new InvalidOperationException("Cannot copy from FilteredLockableList using index");
        }

        [NotNull]
        public IEnumerator<T> GetEnumerator()
        {
            return FilteredList.GetEnumerator();
        }

        public int IndexOf([NotNull] T item) => FilteredList.IndexOf(item);

        public void Insert(int index, [NotNull] T item)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("List is locked");
            }

            lock (_sync)
            {
                List.Insert(index, item);
            }
        }

        public bool Remove([NotNull] T item)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("List is locked");
            }

            lock (_sync)
            {
                return List.Remove(item);
            }
        }

        public void RemoveAt(int index) => throw new InvalidOperationException("Cannot remove from FilteredLockableList using index");

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)FilteredList).GetEnumerator();
        }
    }
}
