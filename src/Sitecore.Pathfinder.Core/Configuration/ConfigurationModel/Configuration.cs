// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel.Internal;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public class Configuration : IConfigurationSourceRoot
    {
        [ItemNotNull, NotNull]
        private readonly IList<IConfigurationSource> _sources = new List<IConfigurationSource>();

        public Configuration([ItemNotNull, CanBeNull] params IConfigurationSource[] sources)
        {
            if (sources == null)
            {
                return;
            }

            foreach (var source in sources)
            {
                Add(source);
            }
        }

        public string this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public IEnumerable<IConfigurationSource> Sources => _sources;

        public IConfigurationSourceRoot Add(IConfigurationSource configurationSource)
        {
            configurationSource.Load();
            return AddLoadedSource(configurationSource);
        }

        public string Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            string s;
            return !TryGet(key, out s) ? null : s;
        }

        [NotNull]
        public IConfiguration GetSubKey(string key)
        {
            return new ConfigurationFocus(this, key + Constants.KeyDelimiter);
        }

        public IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys()
        {
            return GetSubKeysImplementation(string.Empty);
        }

        public IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return GetSubKeysImplementation(key + Constants.KeyDelimiter);
        }

        public void Reload()
        {
            foreach (var source in _sources)
            {
                source.Load();
            }
        }

        public void Set(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            foreach (var source in _sources)
            {
                source.Set(key, value);
            }
        }

        public bool TryGet(string key, out string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            foreach (var configurationSource in _sources.Reverse())
            {
                if (configurationSource.TryGet(key, out value))
                {
                    return true;
                }
            }

            value = null;
            return false;
        }

        [NotNull]
        internal IConfigurationSourceRoot AddLoadedSource([NotNull] IConfigurationSource configurationSource)
        {
            _sources.Add(configurationSource);
            return this;
        }

        private KeyValuePair<string, IConfiguration> CreateConfigurationFocus([NotNull] string prefix, [NotNull] string segment)
        {
            return new KeyValuePair<string, IConfiguration>(segment, new ConfigurationFocus(this, prefix + segment + Constants.KeyDelimiter));
        }

        [NotNull]
        private IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeysImplementation([NotNull] string prefix)
        {
            return _sources.Aggregate(Enumerable.Empty<string>(), (seed, source) => source.ProduceSubKeys(seed, prefix, Constants.KeyDelimiter)).Distinct().Select(segment => CreateConfigurationFocus(prefix, segment));
        }
    }
}
