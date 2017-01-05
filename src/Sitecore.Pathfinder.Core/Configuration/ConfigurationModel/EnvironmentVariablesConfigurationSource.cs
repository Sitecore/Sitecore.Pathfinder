// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public class EnvironmentVariablesConfigurationSource : ConfigurationSource
    {
        private const string ConnStrKeyFormat = "Data:{0}:ConnectionString";

        private const string CustomPrefix = "CUSTOMCONNSTR_";

        private const string MySqlServerPrefix = "MYSQLCONNSTR_";

        private const string ProviderKeyFormat = "Data:{0}:ProviderName";

        private const string SqlAzureServerPrefix = "SQLAZURECONNSTR_";

        private const string SqlServerPrefix = "SQLCONNSTR_";

        private readonly string _prefix;

        public EnvironmentVariablesConfigurationSource(string prefix)
        {
            _prefix = prefix;
        }

        public EnvironmentVariablesConfigurationSource()
        {
            _prefix = string.Empty;
        }

        public override void Load()
        {
            Load(Environment.GetEnvironmentVariables());
        }

        internal void Load(IDictionary envVariables)
        {
            var source = envVariables.Cast<DictionaryEntry>().SelectMany(AzureEnvToAppEnv).Where(entry => ((string)entry.Key).StartsWith(_prefix, StringComparison.OrdinalIgnoreCase));
            var keySelector = (Func<DictionaryEntry, string>)(entry => ((string)entry.Key).Substring(_prefix.Length));
            var ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
            Data = source.ToDictionary(keySelector, entry => (string)entry.Value, ordinalIgnoreCase);
        }

        private static IEnumerable<DictionaryEntry> AzureEnvToAppEnv(DictionaryEntry entry)
        {
            var key = (string)entry.Key;
            var prefix = string.Empty;
            var provider = string.Empty;
            if (key.StartsWith("MYSQLCONNSTR_", StringComparison.OrdinalIgnoreCase))
            {
                prefix = "MYSQLCONNSTR_";
                provider = "MySql.Data.MySqlClient";
            }
            else if (key.StartsWith("SQLAZURECONNSTR_", StringComparison.OrdinalIgnoreCase))
            {
                prefix = "SQLAZURECONNSTR_";
                provider = "System.Data.SqlClient";
            }
            else if (key.StartsWith("SQLCONNSTR_", StringComparison.OrdinalIgnoreCase))
            {
                prefix = "SQLCONNSTR_";
                provider = "System.Data.SqlClient";
            }
            else if (key.StartsWith("CUSTOMCONNSTR_", StringComparison.OrdinalIgnoreCase))
            {
                prefix = "CUSTOMCONNSTR_";
            }
            else
            {
                yield return entry;
                yield break;
            }
            yield return new DictionaryEntry(string.Format("Data:{0}:ConnectionString", key.Substring(prefix.Length)), entry.Value);
            if (!string.IsNullOrEmpty(provider))
            {
                yield return new DictionaryEntry(string.Format("Data:{0}:ProviderName", key.Substring(prefix.Length)), provider);
            }
        }
    }
}
