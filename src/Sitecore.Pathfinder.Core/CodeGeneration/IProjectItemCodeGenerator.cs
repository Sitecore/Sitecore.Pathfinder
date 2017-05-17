// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.CodeGeneration
{
    public interface IProjectItemCodeGenerator
    {
        bool CanGenerate([NotNull] IProjectItem projectItem);

        void Generate([NotNull] IBuildContext context, [NotNull] ITextTemplatingEngine textTemplatingEngine, [NotNull] IProjectItem projectItem, [NotNull] string templateFileName);
    }
}
