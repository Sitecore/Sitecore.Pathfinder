// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Configuration
{
    public static class StartupExtensions
    {
        [CanBeNull]
        public static IConfigurationSourceRoot RegisterConfiguration([NotNull] this Startup startup, ConfigurationOptions options, [NotNull] string projectDirectory, [NotNull] string systemConfigFileName)
        {
            var toolsDirectory = Path.Combine(projectDirectory, "sitecore.tools");

            return RegisterConfiguration(startup, options, toolsDirectory, projectDirectory, systemConfigFileName);
        }

        [CanBeNull]
        public static IConfigurationSourceRoot RegisterConfiguration([NotNull] this Startup startup, ConfigurationOptions options, [NotNull] string toolsDirectory, [NotNull] string projectDirectory, [NotNull] string systemConfigFileName)
        {
            var configuration = new Microsoft.Framework.ConfigurationModel.Configuration();
            configuration.Add(new MemoryConfigurationSource());

            if ((options & ConfigurationOptions.DoNotLoadConfig) == ConfigurationOptions.DoNotLoadConfig)
            {
                configuration.Set(Constants.Configuration.ToolsDirectory, toolsDirectory);
                configuration.Set(Constants.Configuration.ProjectDirectory, projectDirectory);
                configuration.Set(Constants.Configuration.SystemConfigFileName, systemConfigFileName);

                return configuration;
            }

            var configurationService = new ConfigurationService(configuration);
            try
            {                       
                configurationService.Load(options, toolsDirectory, projectDirectory, systemConfigFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (configuration.GetBool(Constants.Configuration.System.ShowStackTrace))
                {
                    Console.WriteLine(ex.StackTrace);
                }

                return null;
            }

            return configuration;
        }
    }
}
