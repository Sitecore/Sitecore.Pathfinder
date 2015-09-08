// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects.Items.FieldResolvers;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProject
    {
        [NotNull]
        ICollection<Diagnostic> Diagnostics { get; }

        long Ducats { get; set; }

        [NotNull]
        IEnumerable<IFieldResolver> FieldResolvers { get; }

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

        void Add([NotNull] string sourceFileName);

        T AddOrMerge<T>([NotNull] IParseContext context, [NotNull] T projectItem) where T : IProjectItem;

        void Compile();

        [CanBeNull]
        IProjectItem FindQualifiedItem([NotNull] string qualifiedName);

        [NotNull]
        IProject Load([NotNull] ProjectOptions projectOptions, [NotNull] IEnumerable<string> sourceFileNames);

        void Remove([NotNull] IProjectItem projectItem);

        void Remove([NotNull] string sourceFileName);

        void SaveChanges();
    }
}
