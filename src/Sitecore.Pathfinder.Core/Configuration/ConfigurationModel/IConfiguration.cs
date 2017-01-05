// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.IConfiguration
// Assembly: Microsoft.Framework.ConfigurationModel.Interfaces, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB4573E8-9F09-49F3-B2EF-6350D9165C25
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.Interfaces.dll

using System.Collections.Generic;

namespace Microsoft.Framework.ConfigurationModel
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

    bool TryGet(string key, out string value);

    IConfiguration GetSubKey(string key);

    IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys();

    IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys(string key);

    void Set(string key, string value);
  }
}
