// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public class CommandLineConfigurationSource : ConfigurationSource
    {
        [CanBeNull]
        private readonly Dictionary<string, string> _switchMappings;

        public CommandLineConfigurationSource(IEnumerable<string> args, [CanBeNull] IDictionary<string, string> switchMappings = null)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Args = args;
            if (switchMappings == null)
            {
                return;
            }

            _switchMappings = GetValidatedSwitchMappingsCopy(switchMappings);
        }

        protected IEnumerable<string> Args { get; }

        public override void Load()
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var enumerator = Args.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key = enumerator.Current;
                var startIndex = 0;
                if (key.StartsWith("--"))
                {
                    startIndex = 2;
                }
                else if (key.StartsWith("-"))
                {
                    startIndex = 1;
                }
                else if (key.StartsWith("/"))
                {
                    key = $"--{key.Substring(1)}";
                    startIndex = 2;
                }

                var length = key.IndexOf('=');
                string index;
                string s;
                if (length < 0)
                {
                    if (startIndex == 0)
                    {
                        throw new FormatException(Resources.FormatError_UnrecognizedArgumentFormat(key));
                    }

                    if (_switchMappings != null && _switchMappings.ContainsKey(key))
                    {
                        index = _switchMappings[key];
                    }
                    else
                    {
                        if (startIndex == 1)
                        {
                            throw new FormatException(Resources.FormatError_ShortSwitchNotDefined(key));
                        }

                        index = key.Substring(startIndex);
                    }

                    var current = enumerator.Current;
                    if (!enumerator.MoveNext())
                    {
                        throw new FormatException(Resources.FormatError_ValueIsMissing(current));
                    }

                    s = enumerator.Current;
                }
                else
                {
                    var key2 = key.Substring(0, length);
                    if (_switchMappings != null && _switchMappings.ContainsKey(key2))
                    {
                        index = _switchMappings[key2];
                    }
                    else
                    {
                        if (startIndex == 1)
                        {
                            throw new FormatException(Resources.FormatError_ShortSwitchNotDefined(key));
                        }

                        index = key.Substring(startIndex, length - startIndex);
                    }

                    s = key.Substring(length + 1);
                }

                dictionary[index] = s;
            }

            Data = dictionary;
        }

        [NotNull]
        private Dictionary<string, string> GetValidatedSwitchMappingsCopy([NotNull] IDictionary<string, string> switchMappings)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var switchMapping in switchMappings)
            {
                if (!switchMapping.Key.StartsWith("-") && !switchMapping.Key.StartsWith("--"))
                {
                    throw new ArgumentException(Resources.FormatError_InvalidSwitchMapping(switchMapping.Key), nameof(switchMappings));
                }

                if (dictionary.ContainsKey(switchMapping.Key))
                {
                    throw new ArgumentException(Resources.FormatError_DuplicatedKeyInSwitchMappings(switchMapping.Key), nameof(switchMappings));
                }

                dictionary.Add(switchMapping.Key, switchMapping.Value);
            }

            return dictionary;
        }
    }
}
