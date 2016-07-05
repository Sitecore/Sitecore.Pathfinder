using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.CodeGeneration
{
    [InheritedExport(typeof(IProjectCodeGenerator))]
    public abstract class ProjectCodeGeneratorBase : IProjectCodeGenerator
    {
        public abstract void Generate(IBuildContext context, IProjectBase project);
    }
}