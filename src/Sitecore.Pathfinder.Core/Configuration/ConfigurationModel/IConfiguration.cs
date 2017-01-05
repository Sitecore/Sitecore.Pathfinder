// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public interface IConfiguration
    {
        string this[string key] { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">A case insensitive name.</param>
        /// <returns>The value associated with the given key, or null if none is found.</returns>
        string Get(string key);

        IConfiguration GetSubKey(string key);

        IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys();

        IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys(string key);

        void Set(string key, string value);

        bool TryGet(string key, out string value);
    }
}
