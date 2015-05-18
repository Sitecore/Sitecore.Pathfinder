namespace Sitecore.Pathfinder.Extensions.ConfigurationExtensions
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;

  public static class ConfigurationExtensions
  {
    [NotNull]
    public static IConfigurationSourceRoot AddFile([NotNull] this IConfigurationSourceRoot configuration, [NotNull] string path)
    {
      if (!File.Exists(path))
      {
        return configuration;
      }

      var extension = Path.GetExtension(path).ToLowerInvariant();
      switch (extension)
      {
        case ".ini":
          configuration.AddIniFile(path);
          break;
        case ".json":
        case ".js":
          configuration.AddJsonFile(path);
          break;

        // case ".xml":
        // configuration.AddXmlFile(path);
        // break;
      }

      return configuration;
    }

    public static bool GetBool([NotNull] this IConfiguration configuration, [NotNull] string key, [NotNull] bool defaultValue = false)
    {
      string value;
      return configuration.TryGet(key, out value) ? string.Compare(value, "true", StringComparison.OrdinalIgnoreCase) == 0 : defaultValue;
    }

    [NotNull]
    public static IEnumerable<string> GetList([NotNull] this IConfiguration configuration, [NotNull] string key)
    {
      var value = configuration.Get(key) ?? string.Empty;
      var parts = value.Split(Pathfinder.Constants.Space, StringSplitOptions.RemoveEmptyEntries);
      return parts;
    }

    [NotNull]
    public static string GetString([NotNull] this IConfiguration configuration, [NotNull] string key, [NotNull] string defaultValue = "")
    {
      return configuration.Get(key) ?? defaultValue;
    }
  }
}
