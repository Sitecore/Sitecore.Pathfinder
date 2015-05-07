namespace Sitecore.Pathfinder.Extensions.ConfigurationExtensions
{
  using System.IO;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;

  public static class ConfigurationExtensions
  {
    [NotNull]
    public static IConfigurationSourceContainer AddFile([NotNull] this IConfigurationSourceContainer configuration, [NotNull] string path)
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
  }
}
