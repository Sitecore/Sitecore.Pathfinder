// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.CodeGeneration
{
    public abstract class ProjectCodeGeneratorBase : IProjectCodeGenerator
    {
        public abstract void Generate(IBuildContext context, IProjectBase project);
    }
}
