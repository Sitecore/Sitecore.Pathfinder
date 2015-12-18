// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>([NotNull, ItemNotNull] this ICollection<T> collection, [NotNull, ItemNotNull] IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
