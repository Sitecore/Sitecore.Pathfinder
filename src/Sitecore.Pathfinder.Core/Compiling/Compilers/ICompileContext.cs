// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    public interface ICompileContext
    {
        [NotNull, ItemNotNull]
        IEnumerable<ICompiler> Compilers { get; }

        [NotNull]
        IFactory Factory { get; }

        [NotNull]
        IProject Project { get; }

        [NotNull]
        ICompileContext With([NotNull] IProject project);
    }
}
