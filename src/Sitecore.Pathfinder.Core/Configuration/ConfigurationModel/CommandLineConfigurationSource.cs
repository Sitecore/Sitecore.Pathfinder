// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public class CommandLineConfigurationSource : ConfigurationSource
    {
        private readonly Dictionary<string, string> _switchMappings;

        public CommandLineConfigurationSource(IEnumerable<string> args, IDictionary<string, string> switchMappings = null)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            Args = args;
            if (switchMappings == null)
            {
                return;
            }
            _switchMappings = GetValidatedSwitchMappingsCopy(switchMappings);
        }

        protected IEnumerable<string> Args { get; private set; }

        public override void Load()
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var enumerator = Args.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key1 = enumerator.Current;
                var startIndex = 0;
                if (key1.StartsWith("--"))
                {
                    startIndex = 2;
                }
                else if (key1.StartsWith("-"))
                {
                    startIndex = 1;
                }
                else if (key1.StartsWith("/"))
                {
                    key1 = string.Format("--{0}", key1.Substring(1));
                    startIndex = 2;
                }
                var length = key1.IndexOf('=');
                string index;
                string str;
                if (length < 0)
                {
                    if (startIndex == 0)
                    {
                        throw new FormatException(Resources.FormatError_UnrecognizedArgumentFormat(key1));
                    }
                    if (_switchMappings != null && _switchMappings.ContainsKey(key1))
                    {
                        index = _switchMappings[key1];
                    }
                    else
                    {
                        if (startIndex == 1)
                        {
                            throw new FormatException(Resources.FormatError_ShortSwitchNotDefined(key1));
                        }
                        index = key1.Substring(startIndex);
                    }
                    var current = enumerator.Current;
                    if (!enumerator.MoveNext())
                    {
                        throw new FormatException(Resources.FormatError_ValueIsMissing(current));
                    }
                    str = enumerator.Current;
                }
                else
                {
                    var key2 = key1.Substring(0, length);
                    if (_switchMappings != null && _switchMappings.ContainsKey(key2))
                    {
                        index = _switchMappings[key2];
                    }
                    else
                    {
                        if (startIndex == 1)
                        {
                            throw new FormatException(Resources.FormatError_ShortSwitchNotDefined(key1));
                        }
                        index = key1.Substring(startIndex, length - startIndex);
                    }
                    str = key1.Substring(length + 1);
                }
                dictionary[index] = str;
            }
            Data = dictionary;
        }

        private Dictionary<string, string> GetValidatedSwitchMappingsCopy(IDictionary<string, string> switchMappings)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var switchMapping in switchMappings)
            {
                if (!switchMapping.Key.StartsWith("-") && !switchMapping.Key.StartsWith("--"))
                {
                    throw new ArgumentException(Resources.FormatError_InvalidSwitchMapping(switchMapping.Key), "switchMappings");
                }
                if (dictionary.ContainsKey(switchMapping.Key))
                {
                    throw new ArgumentException(Resources.FormatError_DuplicatedKeyInSwitchMappings(switchMapping.Key), "switchMappings");
                }
                dictionary.Add(switchMapping.Key, switchMapping.Value);
            }
            return dictionary;
        }
    }
}
