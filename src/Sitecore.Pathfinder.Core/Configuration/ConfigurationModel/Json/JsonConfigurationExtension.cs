// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.JsonConfigurationExtension
// Assembly: Microsoft.Framework.ConfigurationModel.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 29E3F8BD-4D3C-4C9D-8840-A11A97E69911
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.Json.dll

using System;
using System.IO;

namespace Microsoft.Framework.ConfigurationModel
{
  public static class JsonConfigurationExtension
  {
    public static IConfigurationSourceRoot AddJsonFile(this IConfigurationSourceRoot configuration, string path)
    {
      return configuration.AddJsonFile(path, false);
    }

    public static IConfigurationSourceRoot AddJsonFile(this IConfigurationSourceRoot configuration, string path, bool optional)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(Microsoft.Framework.ConfigurationModel.Json.Resources.Error_InvalidFilePath, "path");
      string str = JsonPathResolver.ResolveAppRelativePath(path);
      if (!optional && !File.Exists(str))
        throw new FileNotFoundException(Microsoft.Framework.ConfigurationModel.Json.Resources.Error_FileNotFound, str);
      configuration.Add((IConfigurationSource) new JsonConfigurationSource(path, optional));
      return configuration;
    }
  }
}
