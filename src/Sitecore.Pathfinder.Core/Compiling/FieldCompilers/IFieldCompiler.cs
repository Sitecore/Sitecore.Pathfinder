// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public interface IFieldCompiler
    {
        bool IsExclusive { get; }

        double Priority { get; }

        bool CanCompile([NotNull] IFieldCompileContext context, [NotNull] Field field);

        [NotNull]
        string Compile([NotNull] IFieldCompileContext context, [NotNull] Field field);
    }
}
