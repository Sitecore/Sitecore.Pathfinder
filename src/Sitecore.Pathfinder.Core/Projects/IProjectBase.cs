// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProjectBase : ILockable
    {
        [NotNull, ItemNotNull]
        IEnumerable<File> Files { get; }

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

        [NotNull, ItemNotNull]
        IEnumerable<Template> Templates { get; }

        [CanBeNull]
        T FindQualifiedItem<T>([NotNull] IProjectItemUri uri) where T : class, IProjectItem;

        [CanBeNull]
        T FindQualifiedItem<T>([NotNull] string qualifiedName) where T : class, IProjectItem;

        [CanBeNull]
        T FindQualifiedItem<T>([NotNull] Database database, [NotNull] string qualifiedName) where T : DatabaseProjectItem;

        [NotNull, ItemNotNull]
        IEnumerable<T> GetByFileName<T>([NotNull] string fileName) where T : File;

        [NotNull, ItemNotNull]
        IEnumerable<T> GetByQualifiedName<T>([NotNull] string qualifiedName) where T : class, IProjectItem;

        [NotNull, ItemNotNull]
        IEnumerable<T> GetByQualifiedName<T>([NotNull] Database database, [NotNull] string qualifiedName) where T : DatabaseProjectItem;

        [NotNull, ItemNotNull]
        IEnumerable<T> GetByShortName<T>([NotNull] string shortName) where T : class, IProjectItem;

        [NotNull, ItemNotNull]
        IEnumerable<T> GetByShortName<T>([NotNull] Database database, [NotNull] string shortName) where T : DatabaseProjectItem;

        [NotNull, ItemNotNull]
        IEnumerable<Item> GetChildren([NotNull] Item item);

        [NotNull]
        Database GetDatabase([NotNull] string databaseName);

        [NotNull]
        Language GetLanguage([NotNull] string languageName);

        [NotNull, ItemNotNull]
        IEnumerable<IProjectItem> GetUsages([NotNull] string qualifiedName);
    }
}
