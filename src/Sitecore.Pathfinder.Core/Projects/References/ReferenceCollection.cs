// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Projects.References
{
    public class ReferenceCollection : ICollection<IReference>
    {
        [NotNull, ItemNotNull]
        private readonly ICollection<IReference> _references;

        public ReferenceCollection([NotNull] ProjectItem projectItem)
        {
            ProjectItem = projectItem;

            _references = new LockableList<IReference>(projectItem);
        }

        public int Count => _references.Count;

        public bool IsReadOnly => false;

        [NotNull]
        public ProjectItem ProjectItem { get; }

        public void Add([NotNull] IReference item)
        {
            _references.Add(item);
        }

        public void AddRange([NotNull, ItemNotNull] IEnumerable<IReference> items)
        {
            _references.AddRange(items);
        }

        public void Clear()
        {
            _references.Clear();
        }

        public bool Contains([NotNull] IReference item)
        {
            return _references.Contains(item);
        }

        public void CopyTo([ItemNotNull] IReference[] array, int arrayIndex)
        {
            _references.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IReference> GetEnumerator()
        {
            return _references.GetEnumerator();
        }

        public bool Remove([NotNull] IReference item)
        {
            return _references.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
