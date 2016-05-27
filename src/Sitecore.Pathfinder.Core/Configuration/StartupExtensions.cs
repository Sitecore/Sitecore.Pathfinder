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
        public static IConfigurationSourceRoot RegisterConfiguration([NotNull] this Startup startup, [NotNull] string projectDirectory, [NotNull] string systemConfigFileName, ConfigurationOptions options)
        {
            var toolsDirectory = Path.Combine(projectDirectory, "sitecore.tools");

            return RegisterConfiguration(startup, toolsDirectory, projectDirectory, systemConfigFileName, options);
        }

        [CanBeNull]
        public static IConfigurationSourceRoot RegisterConfiguration([NotNull] this Startup startup, [NotNull] string toolsDirectory, [NotNull] string projectDirectory, [NotNull] string systemConfigFileName, ConfigurationOptions options)
        {
            var configuration = new Microsoft.Framework.ConfigurationModel.Configuration();
            configuration.Add(new MemoryConfigurationSource());

            configuration.Set(Constants.Configuration.ToolsDirectory, toolsDirectory);
            configuration.Set(Constants.Configuration.ProjectDirectory, projectDirectory);
            configuration.Set(Constants.Configuration.SystemConfigFileName, systemConfigFileName);

            var configurationService = new ConfigurationService(configuration);

            if ((options & ConfigurationOptions.DoNotLoadConfig) != ConfigurationOptions.DoNotLoadConfig)
            {
                try
                {                       
                    configurationService.Load(options);
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
            }

            return configuration;
        }
    }
}
