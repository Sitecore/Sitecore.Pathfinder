// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    public abstract class CompilerBase : ICompiler
    {
        protected CompilerBase(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; }

        public abstract bool CanCompile(ICompileContext context, IProjectItem projectItem);

        public abstract void Compile(ICompileContext context, IProjectItem projectItem);

        protected virtual void RetryCompilation([NotNull] IProjectItem projectItem)
        {
            projectItem.State = ProjectItemState.CompilationPending;
        }
    }
}
