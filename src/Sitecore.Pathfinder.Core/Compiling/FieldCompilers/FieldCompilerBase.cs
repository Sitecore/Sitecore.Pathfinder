// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public abstract class FieldCompilerBase : IFieldCompiler
    {
        protected FieldCompilerBase(double priority)
        {
            Priority = priority;
        }

        public virtual bool IsExclusive => false;

        public double Priority { get; }

        public abstract bool CanCompile(IFieldCompileContext context, Field field);

        public abstract string Compile(IFieldCompileContext context, Field field);
    }
}
