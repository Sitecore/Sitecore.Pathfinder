// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProject
    {
        [NotNull, ItemNotNull]
        IEnumerable<Diagnostic> Diagnostics { get; }

        long Ducats { get; set; }

        [NotNull, ItemNotNull]
        IEnumerable<File> Files { get; }

        bool HasErrors { get; }

        [NotNull]
        IProjectIndexer Index { get; }

        [NotNull, ItemNotNull]
        IEnumerable<Item> Items { get; }

        [NotNull]
        ProjectOptions Options { get; }

        [NotNull]
        string ProjectDirectory { get; }

        [NotNull, ItemNotNull]
        IEnumerable<IProjectItem> ProjectItems { get; }

        [NotNull]
        string ProjectUniqueId { get; }

        [NotNull]
        IDictionary<string, ISourceFile> SourceFiles { get; }

        [NotNull, ItemNotNull]
        IEnumerable<Template> Templates { get; }

        [NotNull]
        IProject Add([NotNull] string absoluteFileName);

        [NotNull]
        IProject Add([NotNull, ItemNotNull] IEnumerable<string> sourceFileNames);

        [NotNull]
        T AddOrMerge<T>([NotNull] T projectItem) where T : class, IProjectItem;

        [NotNull]
        IProject Check();

        [NotNull]
        IProject Compile();

        [NotNull, ItemNotNull]
        IEnumerable<T> FindFiles<T>([NotNull] string fileName) where T : File;

        [CanBeNull]
        T FindQualifiedItem<T>([NotNull] string qualifiedName) where T : class, IProjectItem;

        [CanBeNull]
        T FindQualifiedItem<T>([NotNull] Database database, [NotNull] string qualifiedName) where T : DatabaseProjectItem;

        [CanBeNull]
        T FindQualifiedItem<T>([NotNull] ProjectItemUri uri) where T : class, IProjectItem;

        [NotNull]
        Database GetDatabase([NotNull] string databaseName);

        [NotNull, ItemNotNull]
        IEnumerable<Item> GetItems([NotNull] Database database);

        event ProjectChangedEventHandler ProjectChanged;

        void Remove([NotNull] IProjectItem projectItem);

        void Remove([NotNull] string absoluteSourceFileName);

        [NotNull]
        IProject SaveChanges();

        [NotNull]
        IProject With([NotNull] ProjectOptions projectOptions, [NotNull, ItemNotNull] IEnumerable<string> sourceFileNames);
    }
}
