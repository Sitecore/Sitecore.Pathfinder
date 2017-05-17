// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.CodeGeneration
{
    public interface IProjectCodeGenerator
    {
        void Generate([NotNull] IBuildContext context, [NotNull] ITextTemplatingEngine textTemplatingEngine, [NotNull] IProjectBase project);
    }
}
