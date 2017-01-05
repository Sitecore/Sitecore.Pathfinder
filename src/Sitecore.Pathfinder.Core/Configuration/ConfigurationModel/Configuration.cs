// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.Configuration
// Assembly: Microsoft.Framework.ConfigurationModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF6551BA-D3EF-49B9-9DB1-FD9EE239F6F6
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.dll

using Microsoft.Framework.ConfigurationModel.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Framework.ConfigurationModel
{
  public class Configuration : IConfiguration, IConfigurationSourceRoot
  {
    private readonly IList<IConfigurationSource> _sources = (IList<IConfigurationSource>) new List<IConfigurationSource>();

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

    public IEnumerable<IConfigurationSource> Sources
    {
      get
      {
        return (IEnumerable<IConfigurationSource>) this._sources;
      }
    }

    public Configuration(params IConfigurationSource[] sources)
    {
      if (sources == null)
        return;
      foreach (IConfigurationSource source in sources)
        this.Add(source);
    }

    public string Get(string key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      string str;
      if (!this.TryGet(key, out str))
        return (string) null;
      return str;
    }

    public bool TryGet(string key, out string value)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      foreach (IConfigurationSource configurationSource in this._sources.Reverse<IConfigurationSource>())
      {
        if (configurationSource.TryGet(key, out value))
          return true;
      }
      value = (string) null;
      return false;
    }

    public void Set(string key, string value)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      if (value == null)
        throw new ArgumentNullException("value");
      foreach (IConfigurationSource source in (IEnumerable<IConfigurationSource>) this._sources)
        source.Set(key, value);
    }

    public void Reload()
    {
      foreach (IConfigurationSource source in (IEnumerable<IConfigurationSource>) this._sources)
        source.Load();
    }

    public IConfiguration GetSubKey(string key)
    {
      return (IConfiguration) new ConfigurationFocus((IConfiguration) this, key + Constants.KeyDelimiter);
    }

    public IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys()
    {
      return this.GetSubKeysImplementation(string.Empty);
    }

    public IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeys(string key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      return this.GetSubKeysImplementation(key + Constants.KeyDelimiter);
    }

    private IEnumerable<KeyValuePair<string, IConfiguration>> GetSubKeysImplementation(string prefix)
    {
      return this._sources.Aggregate<IConfigurationSource, IEnumerable<string>>(Enumerable.Empty<string>(), (Func<IEnumerable<string>, IConfigurationSource, IEnumerable<string>>) ((seed, source) => source.ProduceSubKeys(seed, prefix, Constants.KeyDelimiter))).Distinct<string>().Select<string, KeyValuePair<string, IConfiguration>>((Func<string, KeyValuePair<string, IConfiguration>>) (segment => this.CreateConfigurationFocus(prefix, segment)));
    }

    private KeyValuePair<string, IConfiguration> CreateConfigurationFocus(string prefix, string segment)
    {
      return new KeyValuePair<string, IConfiguration>(segment, (IConfiguration) new ConfigurationFocus((IConfiguration) this, prefix + segment + Constants.KeyDelimiter));
    }

    public IConfigurationSourceRoot Add(IConfigurationSource configurationSource)
    {
      configurationSource.Load();
      return this.AddLoadedSource(configurationSource);
    }

    internal IConfigurationSourceRoot AddLoadedSource(IConfigurationSource configurationSource)
    {
      this._sources.Add(configurationSource);
      return (IConfigurationSourceRoot) this;
    }
  }
}
