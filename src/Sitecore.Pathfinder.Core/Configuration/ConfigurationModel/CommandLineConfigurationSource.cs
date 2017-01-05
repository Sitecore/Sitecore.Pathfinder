// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.CommandLineConfigurationSource
// Assembly: Microsoft.Framework.ConfigurationModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF6551BA-D3EF-49B9-9DB1-FD9EE239F6F6
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Framework.ConfigurationModel
{
  public class CommandLineConfigurationSource : ConfigurationSource
  {
    private readonly Dictionary<string, string> _switchMappings;

    protected IEnumerable<string> Args { get; private set; }

    public CommandLineConfigurationSource(IEnumerable<string> args, IDictionary<string, string> switchMappings = null)
    {
      if (args == null)
        throw new ArgumentNullException("args");
      this.Args = args;
      if (switchMappings == null)
        return;
      this._switchMappings = this.GetValidatedSwitchMappingsCopy(switchMappings);
    }

    public override void Load()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IEnumerator<string> enumerator = this.Args.GetEnumerator();
      while (enumerator.MoveNext())
      {
        string key1 = enumerator.Current;
        int startIndex = 0;
        if (key1.StartsWith("--"))
          startIndex = 2;
        else if (key1.StartsWith("-"))
          startIndex = 1;
        else if (key1.StartsWith("/"))
        {
          key1 = string.Format("--{0}", (object) key1.Substring(1));
          startIndex = 2;
        }
        int length = key1.IndexOf('=');
        string index;
        string str;
        if (length < 0)
        {
          if (startIndex == 0)
            throw new FormatException(Resources.FormatError_UnrecognizedArgumentFormat((object) key1));
          if (this._switchMappings != null && this._switchMappings.ContainsKey(key1))
          {
            index = this._switchMappings[key1];
          }
          else
          {
            if (startIndex == 1)
              throw new FormatException(Resources.FormatError_ShortSwitchNotDefined((object) key1));
            index = key1.Substring(startIndex);
          }
          string current = enumerator.Current;
          if (!enumerator.MoveNext())
            throw new FormatException(Resources.FormatError_ValueIsMissing((object) current));
          str = enumerator.Current;
        }
        else
        {
          string key2 = key1.Substring(0, length);
          if (this._switchMappings != null && this._switchMappings.ContainsKey(key2))
          {
            index = this._switchMappings[key2];
          }
          else
          {
            if (startIndex == 1)
              throw new FormatException(Resources.FormatError_ShortSwitchNotDefined((object) key1));
            index = key1.Substring(startIndex, length - startIndex);
          }
          str = key1.Substring(length + 1);
        }
        dictionary[index] = str;
      }
      this.Data = (IDictionary<string, string>) dictionary;
    }

    private Dictionary<string, string> GetValidatedSwitchMappingsCopy(IDictionary<string, string> switchMappings)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, string> switchMapping in (IEnumerable<KeyValuePair<string, string>>) switchMappings)
      {
        if (!switchMapping.Key.StartsWith("-") && !switchMapping.Key.StartsWith("--"))
          throw new ArgumentException(Resources.FormatError_InvalidSwitchMapping((object) switchMapping.Key), "switchMappings");
        if (dictionary.ContainsKey(switchMapping.Key))
          throw new ArgumentException(Resources.FormatError_DuplicatedKeyInSwitchMappings((object) switchMapping.Key), "switchMappings");
        dictionary.Add(switchMapping.Key, switchMapping.Value);
      }
      return dictionary;
    }
  }
}
