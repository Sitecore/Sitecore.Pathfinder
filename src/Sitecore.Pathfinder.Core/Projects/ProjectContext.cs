// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Projects
{
    public class ProjectContext
    {
        [FactoryConstructor]
        public ProjectContext()
        {
        }

        [NotNull]
        public Language Language { get; set; } = Language.Empty;
    }
}
