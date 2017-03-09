// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
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

        public virtual void AddCommandLine([NotNull] IConfigurationSourceRoot configurationSourceRoot, [NotNull, ItemNotNull] IEnumerable<string> commandLineArgs)
        {
            var args = configurationSourceRoot.GetCommandLine(commandLineArgs);

            configurationSourceRoot.AddCommandLine(args.ToArray());
        }

        public virtual void Load(ConfigurationOptions options, string toolsDirectory, string projectDirectory, string systemConfigFileName, string[] commandLine)
        {
            var configurationFileNames = new List<string>();

            GetConfigurationFileNames(configurationFileNames, options, toolsDirectory, projectDirectory, systemConfigFileName);

            BuildConfiguration(Configuration, configurationFileNames, options, toolsDirectory, projectDirectory, systemConfigFileName, commandLine);
        }

        protected virtual void BuildConfiguration([NotNull] IConfiguration configuration, [NotNull, ItemNotNull] List<string> configurationFileNames, ConfigurationOptions options, [NotNull] string toolsDirectory, [NotNull] string projectDirectory, [NotNull] string systemConfigFileName, [NotNull, ItemNotNull] string[] commandLine)
        {
            var configurationSourceRoot = configuration as IConfigurationSourceRoot;
            if (configurationSourceRoot == null)
            {
                throw new ConfigurationException(Texts.Configuration_failed_spectacularly);
            }

            configurationSourceRoot.Set(Constants.Configuration.ToolsDirectory, toolsDirectory);
            configurationSourceRoot.Set(Constants.Configuration.ProjectDirectory, projectDirectory);

            // check if there are project configuration files
            if (!configurationFileNames.All(f => f.StartsWith(toolsDirectory, StringComparison.OrdinalIgnoreCase)))
            {
                configurationSourceRoot.Set(Constants.Configuration.IsProjectConfigured, "True");
            }

            // add environment variables
            if ((options & ConfigurationOptions.IncludeEnvironment) == ConfigurationOptions.IncludeEnvironment)
            {
                configurationSourceRoot.AddEnvironmentVariables();
            }

            // add configuration files (distinctly)
            foreach (var configurationFileName in configurationFileNames.Distinct())
            {
                configurationSourceRoot.AddFile(configurationFileName);
            }

            // add config file specified on the command line: /config myconfig.xml
            if ((options & ConfigurationOptions.IncludeCommandLineConfig) == ConfigurationOptions.IncludeCommandLineConfig)
            {
                var configName = configurationSourceRoot.GetString(Constants.Configuration.CommandLineConfig);

                if (!string.IsNullOrEmpty(configName))
                {
                    var configFileName = PathHelper.Combine(projectDirectory, configName);
                    if (File.Exists(configFileName))
                    {
                        configurationSourceRoot.AddFile(configFileName);
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
                configurationSourceRoot.AddCommandLine(commandLine);
            }
        }

        protected virtual void GetConfigurationFileNames([NotNull, ItemNotNull] List<string> configurationFileNames, ConfigurationOptions options, [NotNull] string toolsDirectory, [NotNull] string projectDirectory, [NotNull] string systemConfigFileName)
        {
            // add system config
            systemConfigFileName = Path.Combine(toolsDirectory, systemConfigFileName);
            if (!File.Exists(systemConfigFileName))
            {
                throw new ConfigurationException(Texts.System_configuration_file_not_found, systemConfigFileName);
            }

            configurationFileNames.Add(systemConfigFileName);

            // add system user config file located next to the system config file - scconfig.json.user
            if ((options & ConfigurationOptions.IncludeUserConfig) == ConfigurationOptions.IncludeUserConfig)
            {
                var userConfigFileName = systemConfigFileName + ".user";
                if (File.Exists(userConfigFileName))
                {
                    configurationFileNames.Add(userConfigFileName);
                }
            }

            // get project configuration files
            if ((options & ConfigurationOptions.Recursive) == ConfigurationOptions.Recursive)
            {
                GetProjectConfigurationFilesRecursive(configurationFileNames, options, toolsDirectory, projectDirectory);
            }
            else
            {
                GetProjectConfigurationFiles(configurationFileNames, options, toolsDirectory, projectDirectory);
            }
        }

        protected virtual void GetProjectConfigurationFiles([NotNull, ItemNotNull] List<string> configurationFileNames, ConfigurationOptions options, [NotNull] string toolsDirectory, [NotNull] string directory)
        {
            var projectConfigFileName = Path.Combine(directory, "scconfig.json");
            var machineConfigFileName = Path.GetFileNameWithoutExtension(projectConfigFileName) + "." + Environment.MachineName + ".json";

            // add project config file - scconfig.json
            if (!string.IsNullOrEmpty(projectConfigFileName) && File.Exists(projectConfigFileName))
            {
                configurationFileNames.Add(projectConfigFileName);
            }

            // add module configs (ignore machine config - it will be added last) - scconfig.[module].json 
            if ((options & ConfigurationOptions.IncludeModuleConfig) == ConfigurationOptions.IncludeModuleConfig)
            {
                foreach (var moduleFileName in Directory.GetFiles(directory, "scconfig.*.json").OrderBy(f => f))
                {
                    if (!string.Equals(moduleFileName, machineConfigFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        configurationFileNames.Add(moduleFileName);
                    }
                }
            }

            // add machine level config file - scconfig.[machine name].json
            if ((options & ConfigurationOptions.IncludeMachineConfig) == ConfigurationOptions.IncludeMachineConfig)
            {
                if (File.Exists(machineConfigFileName))
                {
                    configurationFileNames.Add(machineConfigFileName);
                }
            }

            // add user config file - scconfig.json.user
            if ((options & ConfigurationOptions.IncludeUserConfig) == ConfigurationOptions.IncludeUserConfig)
            {
                var userConfigFileName = projectConfigFileName + ".user";
                if (File.Exists(userConfigFileName))
                {
                    configurationFileNames.Add(userConfigFileName);
                }
            }
        }

        protected virtual void GetProjectConfigurationFilesRecursive([NotNull, ItemNotNull] List<string> configurationFileNames, ConfigurationOptions options, [NotNull] string toolsDirectory, [NotNull] string directory)
        {
            var parentDirectory = Path.GetDirectoryName(directory);
            if (parentDirectory != null)
            {
                GetProjectConfigurationFilesRecursive(configurationFileNames, options, toolsDirectory, parentDirectory);
            }

            GetProjectConfigurationFiles(configurationFileNames, options, toolsDirectory, directory);
        }
    }
}
