// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.IniFileConfigurationSource
// Assembly: Microsoft.Framework.ConfigurationModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF6551BA-D3EF-49B9-9DB1-FD9EE239F6F6
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Framework.ConfigurationModel
{
  public class IniFileConfigurationSource : ConfigurationSource
  {
    public bool Optional { get; private set; }

    public string Path { get; private set; }

    /// <summary>
    /// Files are simple line structures
    /// [Section:Header]
    /// key1=value1
    /// key2 = " value2 "
    /// ; comment
    /// # comment
    /// / comment
    /// </summary>
    /// <param name="path">The path and file name to load.</param>
    public IniFileConfigurationSource(string path)
      : this(path, false)
    {
    }

    /// <summary>
    /// Files are simple line structures
    /// [Section:Header]
    /// key1=value1
    /// key2 = " value2 "
    /// ; comment
    /// # comment
    /// / comment
    /// </summary>
    /// <param name="path">The path and file name to load.</param>
    public IniFileConfigurationSource(string path, bool optional)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(Resources.Error_InvalidFilePath, "path");
      this.Optional = optional;
      this.Path = PathResolver.ResolveAppRelativePath(path);
    }

    public override void Load()
    {
      if (!File.Exists(this.Path))
      {
        if (!this.Optional)
          throw new FileNotFoundException(string.Format(Resources.Error_FileNotFound, (object) this.Path), this.Path);
        this.Data = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
      else
      {
        using (FileStream fileStream = new FileStream(this.Path, FileMode.Open, FileAccess.Read))
          this.Load((Stream) fileStream);
      }
    }

    internal void Load(Stream stream)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      using (StreamReader streamReader = new StreamReader(stream))
      {
        string str1 = string.Empty;
        while (streamReader.Peek() != -1)
        {
          string str2 = streamReader.ReadLine();
          string str3 = str2.Trim();
          if (!string.IsNullOrWhiteSpace(str3) && (int) str3[0] != 59 && ((int) str3[0] != 35 && (int) str3[0] != 47))
          {
            if ((int) str3[0] == 91)
            {
              string str4 = str3;
              int index = str4.Length - 1;
              if ((int) str4[index] == 93)
              {
                str1 = str3.Substring(1, str3.Length - 2) + ":";
                continue;
              }
            }
            int length = str3.IndexOf('=');
            if (length < 0)
              throw new FormatException(Resources.FormatError_UnrecognizedLineFormat((object) str2));
            string key = str1 + str3.Substring(0, length).Trim();
            string str5 = str3.Substring(length + 1).Trim();
            if (str5.Length > 1 && (int) str5[0] == 34)
            {
              string str4 = str5;
              int index = str4.Length - 1;
              if ((int) str4[index] == 34)
                str5 = str5.Substring(1, str5.Length - 2);
            }
            if (dictionary.ContainsKey(key))
              throw new FormatException(Resources.FormatError_KeyIsDuplicated((object) key));
            dictionary[key] = str5;
          }
        }
      }
      this.Data = (IDictionary<string, string>) dictionary;
    }
  }
}
