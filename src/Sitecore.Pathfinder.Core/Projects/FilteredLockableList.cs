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
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("List is locked");
                }

                lock (_sync)
                {
                    var i = List.IndexOf(this[index]);
                    if (i < 0)
                    {
                        return;
                    }

                    List[i] = value;
                }

                Changed();
            }
        }

        [ItemNotNull, NotNull]
        protected virtual IEnumerable<T> FilteredList => List;

        protected virtual void Changed()
        {
        }

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

            Changed();
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

            Changed();
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
            lock (_sync)
            {
                FilteredList.ToList().CopyTo(array, arrayIndex);
            }
        }

        [NotNull]
        public IEnumerator<T> GetEnumerator() => FilteredList.GetEnumerator();

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

            Changed();
        }

        public bool Remove([NotNull] T item)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("List is locked");
            }

            bool isRemoved;

            lock (_sync)
            {
                isRemoved = List.Remove(item);
            }

            Changed();

            return isRemoved;
        }

        public void RemoveAt(int index)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("List is locked");
            }

            lock (_sync)
            {
                var i = List.IndexOf(this[index]);
                if (i < 0)
                {
                    return;
                }

                List.RemoveAt(i);
            }

            Changed();
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)FilteredList).GetEnumerator();
    }
}
