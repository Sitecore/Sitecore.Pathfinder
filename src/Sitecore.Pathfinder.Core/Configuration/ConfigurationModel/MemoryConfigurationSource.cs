// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections;
using System.Collections.Generic;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public class MemoryConfigurationSource : ConfigurationSource, IEnumerable<KeyValuePair<string, string>>, IEnumerable
    {
        public MemoryConfigurationSource()
        {
        }

        public MemoryConfigurationSource(IEnumerable<KeyValuePair<string, string>> initialData)
        {
            foreach (var keyValuePair in initialData)
            {
                Data.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public void Add(string key, string value)
        {
            Data.Add(key, value);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
