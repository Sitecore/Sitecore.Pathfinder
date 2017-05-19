// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.ProjectIndexes
{
    public class Index<TV> : Dictionary<string, List<TV>> where TV : class, IProjectItem
    {
        public Index([NotNull] Func<TV, string> getKey)
        {
            GetKey = key => getKey(key).ToUpperInvariant();
        }

        public Index()
        {
        }

        [NotNull]
        protected Func<TV, string> GetKey { get; }

        public void Add([NotNull] TV projectItem)
        {
            Add(GetKey(projectItem), projectItem);
        }

        public void Add([NotNull] string key, [NotNull] TV projectItem)
        {
            key = key.ToUpperInvariant();
            if (!TryGetValue(key, out List<TV> projectItemList))
            {
                projectItemList = new List<TV>();
                this[key] = projectItemList;
            }

            projectItemList.Add(projectItem);
        }

        [CanBeNull]
        public T FirstOrDefault<T>([NotNull] string key) where T : class, TV
        {
            return TryGetValue(key.ToUpperInvariant(), out List<TV> projectItemList) ? projectItemList.OfType<T>().FirstOrDefault() : null;
        }

        public void Remove([NotNull] TV projectItem)
        {
            Remove(GetKey(projectItem), projectItem);
        }

        public void Remove([NotNull] string key, [NotNull] TV projectItem)
        {
            key = key.ToUpperInvariant();
            if (!TryGetValue(key, out List<TV> projectItemList))
            {
                return;
            }

            projectItemList.Remove(projectItem);
            if (!projectItemList.Any())
            {
                Remove(key);
            }
        }

        [NotNull, ItemNotNull]
        public IEnumerable<T> Where<T>([NotNull] string key) where T : class, TV
        {
            return TryGetValue(key.ToUpperInvariant(), out List<TV> projectItemList) ? projectItemList.OfType<T>() : Enumerable.Empty<T>();
        }
    }
}
