// Decompiled with JetBrains decompiler
// Type: Microsoft.Framework.ConfigurationModel.ConfigurationExtensions
// Assembly: Microsoft.Framework.ConfigurationModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF6551BA-D3EF-49B9-9DB1-FD9EE239F6F6
// Assembly location: E:\Sitecore\Sitecore.Pathfinder\code\bin\Microsoft.Framework.ConfigurationModel.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Framework.ConfigurationModel
{
  public static class ConfigurationExtensions
  {
    public static T Get<T>(this IConfiguration configuration, string key)
    {
      return (T) Convert.ChangeType((object) configuration.Get(key), typeof (T));
    }

    public static IConfigurationSourceRoot AddIniFile(this IConfigurationSourceRoot configuration, string path)
    {
      return configuration.AddIniFile(path, false);
    }

    public static IConfigurationSourceRoot AddIniFile(this IConfigurationSourceRoot configuration, string path, bool optional)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(Resources.Error_InvalidFilePath, "path");
      string str = PathResolver.ResolveAppRelativePath(path);
      if (!optional && !File.Exists(str))
        throw new FileNotFoundException(Resources.Error_FileNotFound, str);
      configuration.Add((IConfigurationSource) new IniFileConfigurationSource(path, optional));
      return configuration;
    }

    public static IConfigurationSourceRoot AddCommandLine(this IConfigurationSourceRoot configuration, string[] args)
    {
      configuration.Add((IConfigurationSource) new CommandLineConfigurationSource((IEnumerable<string>) args, (IDictionary<string, string>) null));
      return configuration;
    }

    public static IConfigurationSourceRoot AddCommandLine(this IConfigurationSourceRoot configuration, string[] args, IDictionary<string, string> switchMappings)
    {
      configuration.Add((IConfigurationSource) new CommandLineConfigurationSource((IEnumerable<string>) args, switchMappings));
      return configuration;
    }

    public static IConfigurationSourceRoot AddEnvironmentVariables(this IConfigurationSourceRoot configuration)
    {
      configuration.Add((IConfigurationSource) new EnvironmentVariablesConfigurationSource());
      return configuration;
    }

    public static IConfigurationSourceRoot AddEnvironmentVariables(this IConfigurationSourceRoot configuration, string prefix)
    {
      configuration.Add((IConfigurationSource) new EnvironmentVariablesConfigurationSource(prefix));
      return configuration;
    }
  }
}
