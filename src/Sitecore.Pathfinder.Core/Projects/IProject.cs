// © 2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProject : IProjectBase
    {
        [NotNull, ItemNotNull]
        IEnumerable<Diagnostic> Diagnostics { get; }

        long Ducats { get; set; }

        [NotNull]
        IProjectIndexer Index { get; }

        [NotNull]
        IDictionary<string, ISourceFile> SourceFiles { get; }

        [NotNull]
        IProjectBase Add([NotNull] string absoluteFileName);

        [NotNull]
        IProjectBase Add([NotNull, ItemNotNull] IEnumerable<string> sourceFileNames);

        [NotNull]
        T AddOrMerge<T>([NotNull] T projectItem) where T : class, IProjectItem;

        [NotNull]
        IProjectBase Check();

        [NotNull]
        IProjectBase Compile();

        void Lock(Locking locking);

        event ProjectChangedEventHandler ProjectChanged;

        void Remove([NotNull] IProjectItem projectItem);

        void Remove([NotNull] string absoluteSourceFileName);

        [NotNull]
        IProject With([NotNull] ProjectOptions projectOptions, [NotNull, ItemNotNull] IEnumerable<string> sourceFileNames);
    }
}
