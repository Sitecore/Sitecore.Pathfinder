// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.ConfigurationSource
// Assembly: Microsoft.Framework.ConfigurationModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF6551BA-D3EF-49B9-9DB1-FD9EE239F6F6
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Framework.ConfigurationModel
{
  public abstract class ConfigurationSource : IConfigurationSource
  {
    protected IDictionary<string, string> Data { get; set; }

    protected ConfigurationSource()
    {
      this.Data = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public virtual bool TryGet(string key, out string value)
    {
      return this.Data.TryGetValue(key, out value);
    }

    public virtual void Set(string key, string value)
    {
      this.Data[key] = value;
    }

    public virtual void Load()
    {
    }

    public virtual IEnumerable<string> ProduceSubKeys(IEnumerable<string> earlierKeys, string prefix, string delimiter)
    {
      return this.Data.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kv => ConfigurationSource.Segment(kv.Key, prefix, delimiter))).Concat<string>(earlierKeys);
    }

    private static string Segment(string key, string prefix, string delimiter)
    {
      int num = key.IndexOf(delimiter, prefix.Length, StringComparison.OrdinalIgnoreCase);
      if (num >= 0)
        return key.Substring(prefix.Length, num - prefix.Length);
      return key.Substring(prefix.Length);
    }
  }
}
