// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProject : IProjectBase
    {
        [NotNull, ItemNotNull]
        IEnumerable<Diagnostic> Diagnostics { get; }

        long Ducats { get; set; }

        [NotNull]
        T AddOrMerge<T>([NotNull] T projectItem) where T : class, IProjectItem;

        [NotNull]
        IProjectBase Check();

        [NotNull]
        IProjectBase Compile();

        void Lock(Locking locking);

        [NotNull]
        IProject With([NotNull] ProjectOptions projectOptions, [NotNull, ItemNotNull] IEnumerable<string> sourceFileNames);
    }
}
