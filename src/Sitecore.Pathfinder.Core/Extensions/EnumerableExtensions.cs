// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>([NotNull, ItemCanBeNull] this IEnumerable<T> source, [NotNull] Action<T> action)
        {
            foreach (var t in source)
            {
                action(t);
            }
        }

        public static void ForEach<T>([NotNull, ItemCanBeNull] this IEnumerable<T> source, [NotNull] Action<T> action, [NotNull] Func<T, bool> until)
        {
            foreach (var t in source)
            {
                action(t);
                if (until(t))
                {
                    break;
                }
            }
        }

        public static int IndexOf<TSource>([NotNull, ItemCanBeNull] this IEnumerable<TSource> source, [CanBeNull] TSource item) where TSource : class
        {
            var index = 0;
            foreach (var i in source)
            {
                if (i == item)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }
    }
}
