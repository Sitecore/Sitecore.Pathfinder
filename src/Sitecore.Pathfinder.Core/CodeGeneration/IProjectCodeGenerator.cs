// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.CodeGeneration
{
    public interface IProjectCodeGenerator
    {
        void Generate([NotNull] IBuildContext context, [NotNull] IProjectBase project);
    }
}
