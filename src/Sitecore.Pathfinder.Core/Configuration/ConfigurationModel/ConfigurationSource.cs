// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public abstract class ConfigurationSource : IConfigurationSource
    {
        protected ConfigurationSource()
        {
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

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

        private static string Segment(string key, string prefix, string delimiter)
        {
            var num = key.IndexOf(delimiter, prefix.Length, StringComparison.OrdinalIgnoreCase);
            if (num >= 0)
            {
                return key.Substring(prefix.Length, num - prefix.Length);
            }
            return key.Substring(prefix.Length);
        }
    }
}
