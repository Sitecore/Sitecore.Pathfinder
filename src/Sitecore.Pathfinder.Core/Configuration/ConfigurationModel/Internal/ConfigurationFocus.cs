// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel.Internal
{
    public class ConfigurationFocus : IConfiguration
    {
        private readonly string _prefix;

        private readonly IConfiguration _root;

        public ConfigurationFocus(IConfiguration root, string prefix)
        {
            _prefix = prefix;
            _root = root;
        }

        public string this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public string Get(string key)
        {
            if (key == null)
            {
                return _root.Get(_prefix.Substring(0, _prefix.Length - 1));
            }
            return _root.Get(_prefix + key);
        }

        public IConfiguration GetSubKey(string key)
        {
            return _root.GetSubKey(_prefix + key);
        }

        public IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys()
        {
            return _root.GetSubKeys(_prefix.Substring(0, _prefix.Length - 1));
        }

        public IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys(string key)
        {
            return _root.GetSubKeys(_prefix + key);
        }

        public void Set(string key, string value)
        {
            _root.Set(_prefix + key, value);
        }

        public bool TryGet(string key, out string value)
        {
            if (key == null)
            {
                return _root.TryGet(_prefix.Substring(0, _prefix.Length - 1), out value);
            }
            return _root.TryGet(_prefix + key, out value);
        }
    }
}
