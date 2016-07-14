// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration
{
    [Flags]
    public enum ConfigurationOptions
    {
        None = 0,

        IncludeCommandLine = 1,

        IncludeEnvironment = 2,

        IncludeMachineConfig = 4,

        IncludeUserConfig = 8,

        IncludeCommandLineConfig = 16,

        IncludeModuleConfig = 32,

        DoNotLoadConfig = 64,

        Recursive = 128,

        Interactive = IncludeCommandLine | IncludeEnvironment | IncludeMachineConfig | IncludeUserConfig | IncludeCommandLineConfig | IncludeModuleConfig | Recursive,

        Noninteractive = IncludeCommandLine | IncludeEnvironment | IncludeMachineConfig | IncludeModuleConfig
    }

    public interface IConfigurationService
    {
        [NotNull]
        IConfiguration Configuration { get; }

        void Load(ConfigurationOptions options, [NotNull] string toolsDirectory, [NotNull] string projectDirectory, [NotNull] string systemConfigFileName, [NotNull, ItemNotNull] string[] commandLine);
    }
}
