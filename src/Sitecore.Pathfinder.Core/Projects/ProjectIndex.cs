// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public class ProjectIndex<TV> : Dictionary<string, List<TV>> where TV : class, IProjectItem
    {
        [NotNull]
        private readonly Func<TV, string> _getKey;

        public ProjectIndex([NotNull] Func<TV, string> getKey)
        {
            _getKey = getKey;
        }

        public ProjectIndex()
        {
        }

        public void Add([NotNull] TV projectItem)
        {
            Add(_getKey(projectItem), projectItem);
        }

        public void Add([NotNull] string key, [NotNull] TV projectItem)
        {
            List<TV> projectItemList;

            if (!TryGetValue(key, out projectItemList))
            {
                projectItemList = new List<TV>();
                this[key] = projectItemList;
            }

            projectItemList.Add(projectItem);
        }

        [CanBeNull]
        public T FirstOrDefault<T>([NotNull] string key) where T : class, TV
        {
            List<TV> projectItemList;
            return TryGetValue(key, out projectItemList) ? projectItemList.OfType<T>().FirstOrDefault() : null;
        }

        public void Remove([NotNull] TV projectItem)
        {
            var key = _getKey(projectItem);
            Remove(key, projectItem);
        }

        public void Remove([NotNull] string key, [NotNull] TV projectItem)
        {
            List<TV> projectItemList;

            if (!TryGetValue(key, out projectItemList))
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
            List<TV> projectItemList;
            return TryGetValue(key, out projectItemList) ? projectItemList.OfType<T>() : Enumerable.Empty<T>();
        }
    }
}
