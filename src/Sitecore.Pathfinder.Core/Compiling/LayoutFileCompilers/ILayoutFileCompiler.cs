// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.LayoutFileCompilers
{
    public interface ILayoutFileCompiler
    {
        bool CanCompile([NotNull] ICompileContext context, [NotNull] IProjectItem projectItem, [NotNull] SourceProperty<string> property);

        void Compile([NotNull] ICompileContext context, [NotNull] IProjectItem projectItem, [NotNull] SourceProperty<string> property);
    }
}
