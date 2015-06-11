// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProject
    {
        [NotNull]
        ICollection<Diagnostic> Diagnostics { get; }

        [NotNull]
        IFileSystemService FileSystem { get; }

        bool HasErrors { get; }

        [NotNull]
        IEnumerable<IProjectItem> Items { get; }

        [NotNull]
        ProjectOptions Options { get; }

        [NotNull]
        string ProjectUniqueId { get; }

        [NotNull]
        ICollection<ISourceFile> SourceFiles { get; }

        long Ducats { get; set; }

        void Add([NotNull] string sourceFileName);

        T AddOrMerge<T>([NotNull] T projectItem) where T : IProjectItem;

        void Compile();

        [NotNull]
        IProject Load([NotNull] ProjectOptions projectOptions, [NotNull] IEnumerable<string> sourceFileNames);

        void Remove([NotNull] IProjectItem projectItem);

        void Remove([NotNull] string sourceFileName);

        void SaveChanges();
    }
}
