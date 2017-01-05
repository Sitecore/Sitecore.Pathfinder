// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Configuration
{
    public static class StartupExtensions
    {
        [CanBeNull]
        public static IConfigurationSourceRoot RegisterConfiguration([NotNull] this Startup startup, ConfigurationOptions options, [NotNull] string toolsDirectory, [NotNull] string projectDirectory, [NotNull] string systemConfigFileName, [CanBeNull, ItemNotNull] string[] commandLine)
        {
            var configuration = new ConfigurationModel.Configuration();
            configuration.Add(new MemoryConfigurationSource());

            if ((options & ConfigurationOptions.DoNotLoadConfig) == ConfigurationOptions.DoNotLoadConfig)
            {
                configuration.Set(Constants.Configuration.ToolsDirectory, toolsDirectory);
                configuration.Set(Constants.Configuration.ProjectDirectory, projectDirectory);
                configuration.Set(Constants.Configuration.SystemConfigFileName, systemConfigFileName);

                return configuration;
            }

            if (commandLine == null)
            {
                commandLine = (options & ConfigurationOptions.Interactive) == ConfigurationOptions.Interactive ? configuration.GetCommandLine() : new string[0];
            }

            var configurationService = new ConfigurationService(configuration);
            try
            {
                configurationService.Load(options, toolsDirectory, projectDirectory, systemConfigFileName, commandLine);
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
