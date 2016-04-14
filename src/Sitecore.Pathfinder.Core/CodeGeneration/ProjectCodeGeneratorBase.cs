using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.CodeGeneration
{
    public abstract class ProjectCodeGeneratorBase : IProjectCodeGenerator
    {
        public abstract void Generate(IBuildContext context, IProject project);
    }
}