namespace Sitecore.Pathfinder.Configuration
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(IConfigurationService))]
  public class ConfigurationService : IConfigurationService
  {
    [ImportingConstructor]
    public ConfigurationService([NotNull] IConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    [NotNull]
    public IConfiguration Configuration { get; }

    public virtual void Load(bool includeCommandLineArgs)
    {
      var configurationSourceRoot = this.Configuration as IConfigurationSourceRoot;
      if (configurationSourceRoot == null)
      {
        throw new ConfigurationException(Texts.Text3000);
      }

      var toolsDirectory = configurationSourceRoot.Get(Pathfinder.Constants.ToolsDirectory);

      // add system config
      var fileName = Path.Combine(toolsDirectory, configurationSourceRoot.Get(Pathfinder.Constants.ConfigFileName));
      if (!File.Exists(fileName))
      {
        throw new ConfigurationException(Texts.Text3002, fileName);
      }

      configurationSourceRoot.AddJsonFile(fileName);

      if (includeCommandLineArgs)
      {
        // add command line
        // cut off executable name
        var commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
        configurationSourceRoot.AddCommandLine(commandLineArgs);
      }

      // set solution directory
      var solutionDirectory = PathHelper.Combine(toolsDirectory, configurationSourceRoot.Get(Pathfinder.Constants.SolutionDirectory) ?? string.Empty);
      configurationSourceRoot.Set(Pathfinder.Constants.SolutionDirectory, solutionDirectory);

      // add config
      var websiteConfigFileName = PathHelper.Combine(solutionDirectory, configurationSourceRoot.Get(Pathfinder.Constants.ConfigFileName));
      if (File.Exists(websiteConfigFileName))
      {
        configurationSourceRoot.AddFile(websiteConfigFileName);
      }

      // set project directory
      var projectDirectory = PathHelper.NormalizeFilePath(configurationSourceRoot.Get(Pathfinder.Constants.ProjectDirectory) ?? string.Empty).TrimStart('\\');
      configurationSourceRoot.Set(Pathfinder.Constants.ProjectDirectory, projectDirectory);
    }
  }
}
