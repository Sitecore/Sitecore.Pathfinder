// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.ProjectTrees;
using Sitecore.Pathfinder.Serializing;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProjectContext
    {
        [Diagnostics.NotNull]
        ICompositionService CompositionService { get; }

        [Diagnostics.NotNull]
        IConfiguration Configuration { get; }

        [Diagnostics.NotNull]
        string Name { get; }

        [Diagnostics.NotNull]
        IProjectTree ProjectTree { get; }

        [Diagnostics.NotNull]
        WebsiteSerializer WebsiteSerializer { get; }
    }
}
