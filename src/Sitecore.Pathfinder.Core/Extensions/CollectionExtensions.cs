// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

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

        public static void Merge([NotNull, ItemNotNull] this IList<ISnapshot> list1, [NotNull] ISnapshot first1, [NotNull, ItemNotNull] IEnumerable<ISnapshot> list2, [NotNull] ISnapshot first2, bool overwrite)
        {
            foreach (var snapshot in list2)
            {
                if (!list1.Contains(snapshot))
                {
                    list1.Add(snapshot);
                }
            }

            var first = Snapshot.Empty;
            if (first1 != Snapshot.Empty && first2 != Snapshot.Empty)
            {
                first = overwrite ? first2 : first1;
            }
            else if (first1 != Snapshot.Empty)
            {
                first = first1;
            }
            else if (first2 != Snapshot.Empty)
            {
                first = first2;
            }

            if (first == Snapshot.Empty)
            {
                return;
            }

            list1.Remove(first);
            list1.Insert(0, first);
        }

        public static void Merge([NotNull, ItemNotNull] this IList<ITextNode> list1, [NotNull] ITextNode first1, [NotNull, ItemNotNull] IEnumerable<ITextNode> list2, [NotNull] ITextNode first2, bool overwrite)
        {
            foreach (var textNode in list2)
            {
                if (!list1.Contains(textNode))
                {
                    list1.Add(textNode);
                }
            }

            var first = TextNode.Empty;
            if (first1 != TextNode.Empty && first2 != TextNode.Empty)
            {
                first = overwrite ? first2 : first1;
            }
            else if (first1 != TextNode.Empty)
            {
                first = first1;
            }
            else if (first2 != TextNode.Empty)
            {
                first = first2;
            }

            if (first == TextNode.Empty)
            {
                return;
            }

            list1.Remove(first);
            list1.Insert(0, first);
        }

        public static void RemoveAll<T>([NotNull, ItemNotNull] this ICollection<T> collection, [NotNull] Func<T, bool> match)
        {
            foreach (var item in collection.ToList())
            {
                if (match(item))
                {
                    collection.Remove(item);
                }
            }
        }
    }
}
