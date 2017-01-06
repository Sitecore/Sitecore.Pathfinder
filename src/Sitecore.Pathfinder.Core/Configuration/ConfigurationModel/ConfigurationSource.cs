// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public abstract class ConfigurationSource : IConfigurationSource
    {
        protected ConfigurationSource()
        {
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        [NotNull]
        protected IDictionary<string, string> Data { get; set; }

        public virtual void Load()
        {
        }

        public virtual IEnumerable<string> ProduceSubKeys(IEnumerable<string> earlierKeys, string prefix, string delimiter)
        {
            return Data.Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).Select(kv => Segment(kv.Key, prefix, delimiter)).Concat(earlierKeys);
        }

        public virtual void Set(string key, string value)
        {
            Data[key] = value;
        }

        public virtual bool TryGet(string key, out string value)
        {
            return Data.TryGetValue(key, out value);
        }

        [NotNull]
        private static string Segment([NotNull] string key, [NotNull] string prefix, [NotNull] string delimiter)
        {
            var n = key.IndexOf(delimiter, prefix.Length, StringComparison.OrdinalIgnoreCase);
            if (n >= 0)
            {
                return key.Substring(prefix.Length, n - prefix.Length);
            }

            return key.Substring(prefix.Length);
        }
    }
}
