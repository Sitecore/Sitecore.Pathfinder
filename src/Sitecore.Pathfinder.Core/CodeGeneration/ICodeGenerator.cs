// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.CodeGeneration
{
    [InheritedExport]
    public interface ICodeGenerator
    {
        bool CanGenerate([NotNull] object instance);

        void Generate([NotNull] string baseFileName, [NotNull] object instance);
    }
}
