// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder
{
    public interface IAppService
    {
        [NotNull]
        ICompositionService CompositionService { get; }

        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        string ProjectDirectory { get; }

        [NotNull]
        string ToolsDirectory { get; }
    }
}
