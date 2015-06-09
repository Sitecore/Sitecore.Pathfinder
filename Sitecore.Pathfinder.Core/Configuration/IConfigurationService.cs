// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration
{
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
