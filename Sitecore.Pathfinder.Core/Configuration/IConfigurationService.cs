namespace Sitecore.Pathfinder.Configuration
{
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;

  public interface IConfigurationService
  {
    [NotNull]
    IConfiguration Configuration { get; }

    void Load(bool includeCommandLineArgs);
  }
}
