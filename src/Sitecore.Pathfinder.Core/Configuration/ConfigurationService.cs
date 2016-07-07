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

        protected virtual bool FileExists([NotNull] string fileName)
        {
            return File.Exists(fileName);
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
                    args.Add(arg.Trim());

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
                if (n >= commandLineArgs.Count())
                {
                    args.Add("true");
                    continue;
                }

                arg = commandLineArgs.ElementAt(n);
                if (arg.StartsWith("-") || arg.StartsWith("/"))
                {
                    args.Add("true");
                    n--;
                    continue;
                }

                args.Add(commandLineArgs.ElementAt(n).Trim());
            }

            configurationSourceRoot.AddCommandLine(args.ToArray());
        }

        public virtual void Load(ConfigurationOptions options, string projectDirectory = null)
        {
            var configurationSourceRoot = Configuration as IConfigurationSourceRoot;
            if (configurationSourceRoot == null)
            {
                throw new ConfigurationException(Texts.Configuration_failed_spectacularly);
            }

            var toolsDirectory = configurationSourceRoot.GetToolsDirectory();

            // add system config
            var systemConfigFileName = Path.Combine(toolsDirectory, configurationSourceRoot.GetString(Constants.Configuration.SystemConfigFileName));
            if (!FileExists(systemConfigFileName))
            {
                throw new ConfigurationException(Texts.System_configuration_file_not_found, systemConfigFileName);
            }

            configurationSourceRoot.AddJsonFile(systemConfigFileName);

            // add system user config file located next to the system config file - scconfig.json.user
            if ((options & ConfigurationOptions.IncludeUserConfig) == ConfigurationOptions.IncludeUserConfig)
            {
                var userConfigFileName = systemConfigFileName + ".user";
                if (FileExists(userConfigFileName))
                {
                    configurationSourceRoot.AddFile(userConfigFileName, ".json");
                }
            }

            // add environment variables
            if ((options & ConfigurationOptions.IncludeEnvironment) == ConfigurationOptions.IncludeEnvironment)
            {
                configurationSourceRoot.AddEnvironmentVariables();
            }

            // set project directory
            if (projectDirectory != null)
            {
                configurationSourceRoot.Set(Constants.Configuration.ProjectDirectory, projectDirectory);
            }
            else
            {
                projectDirectory = configurationSourceRoot.GetString(Constants.Configuration.ProjectDirectory);
            }

            // add project config file - scconfig.json
            var projectConfigFileName = string.Empty;

            var configurationProjectConfigFileName = configurationSourceRoot.GetString(Constants.Configuration.ProjectConfigFileName);
            if (!string.IsNullOrEmpty(configurationProjectConfigFileName))
            {
                projectConfigFileName = PathHelper.Combine(projectDirectory, configurationProjectConfigFileName);
            }

            if (!string.IsNullOrEmpty(projectConfigFileName) && FileExists(projectConfigFileName))
            {
                configurationSourceRoot.AddFile(projectConfigFileName);
                configurationSourceRoot.Set(Constants.Configuration.IsProjectConfigured, "True");
            }
            else if (Directory.GetFiles(projectDirectory).Any() || Directory.GetDirectories(projectDirectory).Any())
            {
                // no config file, but project directory has files, so let's try the default project config file
                projectConfigFileName = PathHelper.Combine(toolsDirectory, "files\\project.noconfig\\scconfig.json");
                if (FileExists(projectConfigFileName))
                {
                    configurationSourceRoot.AddFile(projectConfigFileName);
                }
            }

            // add project role configs 
            var projectRoles = configurationSourceRoot.GetStringList(Constants.Configuration.ProjectRole);
            foreach (var projectRole in projectRoles)
            {
                foreach (var pair in configurationSourceRoot.GetSubKeys("project-role-conventions:" + projectRole))
                {
                    var conventionsFileName = configurationSourceRoot.GetString("project-role-conventions:" + projectRole + ":" + pair.Key);
                    if (string.IsNullOrEmpty(conventionsFileName))
                    {
                        continue;
                    }

                    if (conventionsFileName.StartsWith("$tools/", StringComparison.OrdinalIgnoreCase))
                    {
                        conventionsFileName = Path.Combine(configurationSourceRoot.GetToolsDirectory(), PathHelper.NormalizeFilePath(conventionsFileName.Mid(7)));
                    }

                    if (conventionsFileName.StartsWith("~/", StringComparison.OrdinalIgnoreCase))
                    {
                        conventionsFileName = Path.Combine(configurationSourceRoot.GetString(Constants.Configuration.ProjectDirectory), PathHelper.NormalizeFilePath(conventionsFileName.Mid(2)));
                    }

                    // filename might be a json config file or the name of a class in an extension
                    if (FileExists(conventionsFileName))
                    {
                        configurationSourceRoot.AddFile(conventionsFileName);
                    }
                }
            }

            var machineConfigFileName = Path.GetFileNameWithoutExtension(projectConfigFileName) + "." + Environment.MachineName + ".json";

            // add module configs (ignore machine config - it will be added last) - scconfig.[module].json 
            if ((options & ConfigurationOptions.IncludeModuleConfig) == ConfigurationOptions.IncludeModuleConfig)
            {
                foreach (var moduleFileName in Directory.GetFiles(projectDirectory, "scconfig.*.json").OrderBy(f => f))
                {
                    if (!string.Equals(moduleFileName, machineConfigFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        configurationSourceRoot.AddFile(moduleFileName);
                    }
                }
            }

            // add machine level config file - scconfig.[machine name].json
            if ((options & ConfigurationOptions.IncludeMachineConfig) == ConfigurationOptions.IncludeMachineConfig)
            {
                if (FileExists(machineConfigFileName))
                {
                    configurationSourceRoot.AddFile(machineConfigFileName);
                }
            }

            // add user config file - scconfig.json.user
            if ((options & ConfigurationOptions.IncludeUserConfig) == ConfigurationOptions.IncludeUserConfig)
            {
                var userConfigFileName = projectConfigFileName + ".user";
                if (FileExists(userConfigFileName))
                {
                    configurationSourceRoot.AddFile(userConfigFileName, ".json");
                }
            }

            // add config file specified on the command line: /config myconfig.xml
            if ((options & ConfigurationOptions.IncludeCommandLineConfig) == ConfigurationOptions.IncludeCommandLineConfig)
            {
                var configName = configurationSourceRoot.GetString(Constants.Configuration.CommandLineConfig);

                if (!string.IsNullOrEmpty(configName))
                {
                    var configFileName = PathHelper.Combine(projectDirectory, configName);
                    if (FileExists(configFileName))
                    {
                        configurationSourceRoot.AddFile(configFileName);
                        configurationSourceRoot.Set(Constants.Configuration.IsProjectConfigured, "True");
                    }
                    else
                    {
                        throw new ConfigurationException(Texts.Config_file_not_found__ + configFileName);
                    }
                }
            }

            // add command line
            if ((options & ConfigurationOptions.IncludeCommandLine) == ConfigurationOptions.IncludeCommandLine)
            {
                AddCommandLine(configurationSourceRoot);
            }
        }
    }
}
