// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    public interface ICompiler
    {
        double Priority { get; }

        bool CanCompile([NotNull] ICompileContext context, [NotNull] IProjectItem projectItem);

        void Compile([NotNull] ICompileContext context, [NotNull] IProjectItem projectItem);
    }
}
