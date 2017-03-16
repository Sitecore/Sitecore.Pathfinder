// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public class LockableList<T> : IList<T>
    {
        [NotNull]
        private readonly object _syncObject = new object();

        public LockableList([NotNull] ILockable owner)
        {
            Owner = owner;
        }

        public int Count => List.Count;

        public bool IsReadOnly => Owner.Locking == Locking.ReadOnly;

        [NotNull]
        public T this[int index]
        {
            get { return List[index]; }
            set
            {
                switch (Owner.Locking)
                {
                    case Locking.ReadWrite:
                        List[index] = value;
                        break;

                    case Locking.ReadOnly:
                        throw new InvalidOperationException("List is locked");

                    case Locking.CopyOnWrite:
                        lock (_syncObject)
                        {
                            List = new ConcurrentList<T>(List)
                            {
                                [index] = value
                            };
                        }
                        break;
                }
            }
        }

        [NotNull, ItemNotNull]
        private ConcurrentList<T> List { get; set; } = new ConcurrentList<T>();

        [NotNull]
        private ILockable Owner { get; }

        public void Add([NotNull] T item)
        {
            switch (Owner.Locking)
            {
                case Locking.ReadWrite:
                    List.Add(item);
                    break;

                case Locking.ReadOnly:
                    throw new InvalidOperationException("List is locked");

                case Locking.CopyOnWrite:
                    lock (_syncObject)
                    {
                        List = new ConcurrentList<T>(List)
                        {
                            item
                        };
                    }
                    break;
            }
        }

        public void Clear()
        {
            switch (Owner.Locking)
            {
                case Locking.ReadWrite:
                    List.Clear();
                    break;

                case Locking.ReadOnly:
                    throw new InvalidOperationException("List is locked");

                case Locking.CopyOnWrite:
                    lock (_syncObject)
                    {
                        List = new ConcurrentList<T>();
                    }
                    break;
            }
        }

        public bool Contains([NotNull] T item)
        {
            return List.Contains(item);
        }

        public void CopyTo([ItemNotNull] T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public int IndexOf([NotNull] T item) => List.IndexOf(item);

        public void Insert(int index, [NotNull] T item)
        {
            switch (Owner.Locking)
            {
                case Locking.ReadWrite:
                    List.Insert(index, item);
                    break;

                case Locking.ReadOnly:
                    throw new InvalidOperationException("List is locked");

                case Locking.CopyOnWrite:
                    lock (_syncObject)
                    {
                        List = new ConcurrentList<T>(List);
                        List.Insert(index, item);
                    }
                    break;
            }
        }

        public bool Remove([NotNull] T item)
        {
            switch (Owner.Locking)
            {
                case Locking.ReadWrite:
                    return List.Remove(item);

                case Locking.ReadOnly:
                    throw new InvalidOperationException("List is locked");

                case Locking.CopyOnWrite:
                    lock (_syncObject)
                    {
                        List = new ConcurrentList<T>(List);
                        return List.Remove(item);
                    }
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            switch (Owner.Locking)
            {
                case Locking.ReadWrite:
                    List.RemoveAt(index);
                    break;

                case Locking.ReadOnly:
                    throw new InvalidOperationException("List is locked");

                case Locking.CopyOnWrite:
                    lock (_syncObject)
                    {
                        List = new ConcurrentList<T>(List);
                        List.RemoveAt(index);
                    }
                    break;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)List).GetEnumerator();
        }
    }
}
