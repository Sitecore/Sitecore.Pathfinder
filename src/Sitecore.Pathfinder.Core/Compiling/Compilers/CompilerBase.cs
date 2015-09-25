// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    public abstract class CompilerBase : ICompiler
    {
        public abstract bool CanCompile(ICompileContext context, IProjectItem projectItem);

        public abstract void Compile(ICompileContext context, IProjectItem projectItem);
    }
}
