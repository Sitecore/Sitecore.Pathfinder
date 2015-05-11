namespace Sitecore.Pathfinder.Configuration
{
  using System;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;

  [Flags]
  public enum LoadConfigurationOptions
  {
    None = 0, 

    IncludeCommandLine = 1
  }

  public interface IConfigurationService
  {
    [NotNull]
    IConfiguration Configuration { get; }

    void Load(LoadConfigurationOptions options);
  }
}
