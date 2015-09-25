// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProject
    {
        [NotNull]
        [ItemNotNull]
        ICollection<Diagnostic> Diagnostics { get; }

        long Ducats { get; set; }

        [NotNull]
        IFileSystemService FileSystem { get; }

        bool HasErrors { get; }

        [NotNull]
        [ItemNotNull]
        IEnumerable<IProjectItem> Items { get; }

        [NotNull]
        ProjectOptions Options { get; }

        [NotNull]
        string ProjectUniqueId { get; }

        [NotNull]
        [ItemNotNull]
        ICollection<ISourceFile> SourceFiles { get; }

        [NotNull]
        IProject Add([NotNull] string sourceFileName);

        [NotNull]
        T AddOrMerge<T>([NotNull] T projectItem) where T : IProjectItem;

        [NotNull]
        IProject Compile();

        [CanBeNull]
        IProjectItem FindQualifiedItem([NotNull] string qualifiedName);

        [CanBeNull]
        IProjectItem FindQualifiedItem([NotNull] string databaseName, [NotNull] string qualifiedName);

        [CanBeNull]
        IProjectItem FindQualifiedItem([NotNull] ProjectItemUri uri);

        [NotNull]
        IProject Load([NotNull] ProjectOptions projectOptions, [NotNull] [ItemNotNull] IEnumerable<string> sourceFileNames);

        void Remove([NotNull] IProjectItem projectItem);

        void Remove([NotNull] string sourceFileName);

        [NotNull]
        IProject SaveChanges();
    }
}
