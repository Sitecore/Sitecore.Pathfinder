// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Configuration
{
    [Export(typeof(IConfigurationService))]
    public class ConfigurationService : IConfigurationService
    {
        [ImportingConstructor]
        public ConfigurationService([NotNull] IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void AddCommandLine([NotNull] IConfigurationSourceRoot configurationSourceRoot)
        {
            // cut off executable name
            var commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
            AddCommandLine(configurationSourceRoot, commandLineArgs);
        }

        public virtual void AddCommandLine([NotNull] IConfigurationSourceRoot configurationSourceRoot, [NotNull][ItemNotNull] IEnumerable<string> commandLineArgs)
        {
            var args = new List<string>();

            var positionalArg = 0;
            for (var n = 0; n < commandLineArgs.Count(); n++)
            {
                var arg = commandLineArgs.ElementAt(n);

                // if the arg is not a switch, add it to the list of position args
                if (!arg.StartsWith("-") && !arg.StartsWith("/"))
                {
                    args.Add("/arg" + positionalArg);
                    args.Add(arg);

                    positionalArg++;

                    continue;
                }

                // if the arg is a switch, add it to the list of args to pass to the commandline configuration
                args.Add(arg);
                if (arg.IndexOf('=') >= 0)
                {
                    continue;
                }

                n++;
                if (n < commandLineArgs.Count())
                {
                    args.Add(commandLineArgs.ElementAt(n));
                }
            }

            configurationSourceRoot.AddCommandLine(args.ToArray());
        }

        public virtual void Load(LoadConfigurationOptions options, string solutionDirectory = null)
        {
            var configurationSourceRoot = Configuration as IConfigurationSourceRoot;
            if (configurationSourceRoot == null)
            {
                throw new ConfigurationException(Texts.Configuration_failed_spectacularly);
            }

            var toolsDirectory = configurationSourceRoot.Get(Constants.Configuration.ToolsDirectory);

            // add system config
            var fileName = Path.Combine(toolsDirectory, configurationSourceRoot.Get(Constants.Configuration.SystemConfigFileName));
            if (!File.Exists(fileName))
            {
                throw new ConfigurationException(Texts.System_configuration_file_not_found, fileName);
            }

            configurationSourceRoot.AddJsonFile(fileName);

            // add command line
            if ((options & LoadConfigurationOptions.IncludeCommandLine) == LoadConfigurationOptions.IncludeCommandLine)
            {
                AddCommandLine(configurationSourceRoot);
            }

            // set solution directory
            if (solutionDirectory == null)
            {
                solutionDirectory = PathHelper.Combine(toolsDirectory, configurationSourceRoot.GetString(Constants.Configuration.SolutionDirectory));
            }

            configurationSourceRoot.Set(Constants.Configuration.SolutionDirectory, solutionDirectory);

            // add project config file
            var projectConfigFileName = PathHelper.Combine(solutionDirectory, configurationSourceRoot.Get(Constants.Configuration.ProjectConfigFileName));
            if (File.Exists(projectConfigFileName))
            {
                configurationSourceRoot.AddFile(projectConfigFileName);
            }

            // add user config file
            if ((options & LoadConfigurationOptions.IncludeUserConfig) == LoadConfigurationOptions.IncludeUserConfig)
            {
                var userConfigFileName = projectConfigFileName + ".user";
                if (File.Exists(userConfigFileName))
                {
                    configurationSourceRoot.AddFile(userConfigFileName);
                }
            }

            // set project directory
            var projectDirectory = PathHelper.NormalizeFilePath(configurationSourceRoot.GetString(Constants.Configuration.ProjectDirectory)).TrimStart('\\');
            configurationSourceRoot.Set(Constants.Configuration.ProjectDirectory, projectDirectory);
        }
    }
}
