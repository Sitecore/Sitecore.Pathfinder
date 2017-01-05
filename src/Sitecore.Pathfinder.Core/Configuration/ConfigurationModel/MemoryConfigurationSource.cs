// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.MemoryConfigurationSource
// Assembly: Microsoft.Framework.ConfigurationModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF6551BA-D3EF-49B9-9DB1-FD9EE239F6F6
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Framework.ConfigurationModel
{
  public class MemoryConfigurationSource : ConfigurationSource, IEnumerable<KeyValuePair<string, string>>, IEnumerable
  {
    public MemoryConfigurationSource()
    {
    }

    public MemoryConfigurationSource(IEnumerable<KeyValuePair<string, string>> initialData)
    {
      foreach (KeyValuePair<string, string> keyValuePair in initialData)
        this.Data.Add(keyValuePair.Key, keyValuePair.Value);
    }

    public void Add(string key, string value)
    {
      this.Data.Add(key, value);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
      return this.Data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
