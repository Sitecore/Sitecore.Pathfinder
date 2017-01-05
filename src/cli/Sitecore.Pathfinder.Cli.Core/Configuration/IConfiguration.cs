// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;

namespace Sitecore.Pathfinder.Configuration
{
    public interface IConfiguration
    {
        string this[string key] { get; set; }

        string Get(string key);

        bool TryGet(string key, out string value);

        IConfiguration GetSubKey(string key);

        IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys();

        IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys(string key);

        void Set(string key, string value);
    }
}