// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public class SynchronizedList<T> : IList<T>
    {
        [NotNull]
        private readonly object _sync = new object();

        public int Count => List.Count;

        public bool IsReadOnly => false;

        [NotNull]
        public T this[int index]
        {
            get
            {
                lock (_sync)
                {
                    return List[index];
                }
            }
            set
            {
                lock (_sync)
                {
                    List[index] = value;
                }
            }
        }

        [NotNull, ItemNotNull]
        private List<T> List { get; } = new List<T>();

        public void Add([NotNull] T item)
        {
            lock (_sync)
            {
                List.Add(item);
            }
        }

        public void Clear()
        {
            lock (_sync)
            {
                List.Clear();
            }
        }

        public bool Contains([NotNull] T item)
        {
            lock (_sync)
            {
                return List.Contains(item);
            }
        }

        public void CopyTo([ItemNotNull] T[] array, int arrayIndex)
        {
            lock (_sync)
            {
                List.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public int IndexOf([NotNull] T item) => List.IndexOf(item);

        public void Insert(int index, [NotNull] T item)
        {
            lock (_sync)
            {
                List.Insert(index, item);
            }
        }

        public bool Remove([NotNull] T item)
        {
            lock (_sync)
            {
                return List.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_sync)
            {
                List.RemoveAt(index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)List).GetEnumerator();
        }
    }
}
