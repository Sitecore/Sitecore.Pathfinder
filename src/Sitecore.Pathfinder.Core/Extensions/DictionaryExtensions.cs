// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class DictionaryExtensions
    {
        [NotNull]
        public static IDictionary<TK, TV> AddRange<TK, TV>([NotNull] this IDictionary<TK, TV> dictionary, [NotNull] IDictionary<TK, TV> items)
        {
            foreach (var item in items)
            {
                dictionary[item.Key] = item.Value;
            }

            return dictionary;
        }

        [NotNull]
        public static string GetString([NotNull] this IDictionary<string, string> dictionary, [NotNull] string key)
        {
            string value;
            return dictionary.TryGetValue(key, out value) ? value : string.Empty;
        }
    }
}
