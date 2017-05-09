// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.CodeGeneration
{
    public interface IProjectItemCodeGenerator
    {
        bool CanGenerate([NotNull] object instance);

        void Generate([NotNull] string baseFileName, [NotNull] object instance);
    }
}
