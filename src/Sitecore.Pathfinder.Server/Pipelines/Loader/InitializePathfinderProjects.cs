// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Projects;
using Sitecore.Pipelines;

namespace Sitecore.Pathfinder.Pipelines.Loader
{
    public class InitializePathfinderProjects
    {
        public void Process([NotNull] PipelineArgs args)
        {
            ProjectHost.Initialize();
        }
    }
}
