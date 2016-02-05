// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public abstract class FieldCompilerBase : IFieldCompiler
    {
        protected FieldCompilerBase(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; }

        public virtual bool IsExclusive => false;

        public abstract bool CanCompile(IFieldCompileContext context, Field field);

        public abstract string Compile(IFieldCompileContext context, Field field);
    }
}
