namespace Sitecore.Pathfinder.Configuration
{
  using System;
  using System.Collections.Generic;
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

    public IConfiguration Configuration { get; }

    public virtual void Load(LoadConfigurationOptions options)
    {
      var configurationSourceRoot = this.Configuration as IConfigurationSourceRoot;
      if (configurationSourceRoot == null)
      {
        throw new ConfigurationException(Texts.Text3000);
      }

      var toolsDirectory = configurationSourceRoot.Get(Pathfinder.Constants.Configuration.ToolsDirectory);

      // add system config
      var fileName = Path.Combine(toolsDirectory, configurationSourceRoot.Get(Pathfinder.Constants.Configuration.ConfigFileName));
      if (!File.Exists(fileName))
      {
        throw new ConfigurationException(Texts.Text3002, fileName);
      }

      configurationSourceRoot.AddJsonFile(fileName);

      if ((options & LoadConfigurationOptions.IncludeCommandLine) == LoadConfigurationOptions.IncludeCommandLine)
      {
        this.AddCommandLine(configurationSourceRoot);
      }

      // set solution directory
      var solutionDirectory = PathHelper.Combine(toolsDirectory, configurationSourceRoot.Get(Pathfinder.Constants.Configuration.SolutionDirectory) ?? string.Empty);
      configurationSourceRoot.Set(Pathfinder.Constants.Configuration.SolutionDirectory, solutionDirectory);

      // add config
      var websiteConfigFileName = PathHelper.Combine(solutionDirectory, configurationSourceRoot.Get(Pathfinder.Constants.Configuration.ConfigFileName));
      if (File.Exists(websiteConfigFileName))
      {
        configurationSourceRoot.AddFile(websiteConfigFileName);
      }

      // set project directory
      var projectDirectory = PathHelper.NormalizeFilePath(configurationSourceRoot.Get(Pathfinder.Constants.Configuration.ProjectDirectory) ?? string.Empty).TrimStart('\\');
      configurationSourceRoot.Set(Pathfinder.Constants.Configuration.ProjectDirectory, projectDirectory);
    }

    private void AddCommandLine([NotNull] IConfigurationSourceRoot configurationSourceRoot)
    {
      // cut off executable name
      var commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
      Console.WriteLine(string.Join(", ", commandLineArgs));

      for (var n = 0; n < commandLineArgs.Count(); n++)
      {
        var arg = commandLineArgs[n];
        if (arg.StartsWith("-") || arg.StartsWith("/"))
        {
          n++;
          continue;
        }

        commandLineArgs = commandLineArgs.Take(n).Concat(commandLineArgs.Skip(n + 1)).ToList();
        Console.WriteLine(string.Join(", ", commandLineArgs));

        // ignore "build", it is default
        if (string.Compare(arg, "Build", StringComparison.OrdinalIgnoreCase) != 0)
        {
          configurationSourceRoot.Set("run", arg);
        }

        break;
      }

      var switchMappings = new Dictionary<string, string>()
      {
        ["-r"] = "run",
        ["--r"] = "run"
      };

      configurationSourceRoot.AddCommandLine(commandLineArgs.ToArray(), switchMappings);
    }
  }
}
