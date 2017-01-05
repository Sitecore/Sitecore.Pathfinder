// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.Internal.ConfigurationFocus
// Assembly: Microsoft.Framework.ConfigurationModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF6551BA-D3EF-49B9-9DB1-FD9EE239F6F6
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.dll

using System.Collections.Generic;

namespace Microsoft.Framework.ConfigurationModel.Internal
{
  public class ConfigurationFocus : IConfiguration
  {
    private readonly string _prefix;
    private readonly IConfiguration _root;

    public string this[string key]
    {
      get
      {
        return this.Get(key);
      }
      set
      {
        this.Set(key, value);
      }
    }

    public ConfigurationFocus(IConfiguration root, string prefix)
    {
      this._prefix = prefix;
      this._root = root;
    }

    public string Get(string key)
    {
      if (key == null)
        return this._root.Get(this._prefix.Substring(0, this._prefix.Length - 1));
      return this._root.Get(this._prefix + key);
    }

    public bool TryGet(string key, out string value)
    {
      if (key == null)
        return this._root.TryGet(this._prefix.Substring(0, this._prefix.Length - 1), out value);
      return this._root.TryGet(this._prefix + key, out value);
    }

    public IConfiguration GetSubKey(string key)
    {
      return this._root.GetSubKey(this._prefix + key);
    }

    public void Set(string key, string value)
    {
      this._root.Set(this._prefix + key, value);
    }

    public IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys()
    {
      return this._root.GetSubKeys(this._prefix.Substring(0, this._prefix.Length - 1));
    }

    public IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys(string key)
    {
      return this._root.GetSubKeys(this._prefix + key);
    }
  }
}
