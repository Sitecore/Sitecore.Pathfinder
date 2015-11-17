// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.CodeGeneration
{
    [InheritedExport]
    public interface IProjectCodeGenerator
    {
        void Generate([NotNull] IBuildContext context, [NotNull] IProject project);
    }
}
