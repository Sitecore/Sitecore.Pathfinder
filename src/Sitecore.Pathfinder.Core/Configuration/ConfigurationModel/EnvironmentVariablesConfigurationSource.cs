// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.EnvironmentVariablesConfigurationSource
// Assembly: Microsoft.Framework.ConfigurationModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF6551BA-D3EF-49B9-9DB1-FD9EE239F6F6
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Framework.ConfigurationModel
{
  public class EnvironmentVariablesConfigurationSource : ConfigurationSource
  {
    private const string MySqlServerPrefix = "MYSQLCONNSTR_";
    private const string SqlAzureServerPrefix = "SQLAZURECONNSTR_";
    private const string SqlServerPrefix = "SQLCONNSTR_";
    private const string CustomPrefix = "CUSTOMCONNSTR_";
    private const string ConnStrKeyFormat = "Data:{0}:ConnectionString";
    private const string ProviderKeyFormat = "Data:{0}:ProviderName";
    private readonly string _prefix;

    public EnvironmentVariablesConfigurationSource(string prefix)
    {
      this._prefix = prefix;
    }

    public EnvironmentVariablesConfigurationSource()
    {
      this._prefix = string.Empty;
    }

    public override void Load()
    {
      this.Load(Environment.GetEnvironmentVariables());
    }

    internal void Load(IDictionary envVariables)
    {
      IEnumerable<DictionaryEntry> source = envVariables.Cast<DictionaryEntry>().SelectMany<DictionaryEntry, DictionaryEntry>(new Func<DictionaryEntry, IEnumerable<DictionaryEntry>>(EnvironmentVariablesConfigurationSource.AzureEnvToAppEnv)).Where<DictionaryEntry>((Func<DictionaryEntry, bool>) (entry => ((string) entry.Key).StartsWith(this._prefix, StringComparison.OrdinalIgnoreCase)));
      Func<DictionaryEntry, string> keySelector = (Func<DictionaryEntry, string>) (entry => ((string) entry.Key).Substring(this._prefix.Length));
      StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
      this.Data = (IDictionary<string, string>) source.ToDictionary<DictionaryEntry, string, string>(keySelector, (Func<DictionaryEntry, string>) (entry => (string) entry.Value), (IEqualityComparer<string>) ordinalIgnoreCase);
    }

    private static IEnumerable<DictionaryEntry> AzureEnvToAppEnv(DictionaryEntry entry)
    {
      string key = (string) entry.Key;
      string prefix = string.Empty;
      string provider = string.Empty;
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
      yield return new DictionaryEntry((object) string.Format("Data:{0}:ConnectionString", (object) key.Substring(prefix.Length)), entry.Value);
      if (!string.IsNullOrEmpty(provider))
        yield return new DictionaryEntry((object) string.Format("Data:{0}:ProviderName", (object) key.Substring(prefix.Length)), (object) provider);
    }
  }
}
