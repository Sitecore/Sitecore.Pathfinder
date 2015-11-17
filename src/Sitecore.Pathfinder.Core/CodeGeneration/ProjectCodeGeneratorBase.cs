using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.CodeGeneration
{
    public abstract class ProjectCodeGeneratorBase : IProjectCodeGenerator
    {
        public abstract void Generate(IBuildContext context, IProject project);
    }
}