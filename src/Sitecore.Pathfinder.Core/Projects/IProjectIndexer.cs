// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProjectIndexer
    {
        [NotNull, ItemNotNull]
        ICollection<Item> Items { get; }

        [NotNull, ItemNotNull]
        ICollection<Template> Templates { get; }

        void Add([NotNull] IProjectItem projectItem);

        [CanBeNull]
        T FindQualifiedItem<T>(Guid guid) where T : class, IProjectItem;

        [CanBeNull]
        T FindQualifiedItem<T>([NotNull] string qualifiedName) where T : class, IProjectItem;

        [CanBeNull]
        T FindQualifiedItem<T>([NotNull] Database database, [NotNull] string qualifiedName) where T : DatabaseProjectItem;

        [CanBeNull]
        T FindQualifiedItem<T>([NotNull] IProjectItemUri uri) where T : class, IProjectItem;

        [NotNull, ItemNotNull]
        IEnumerable<IReference> GetUsages([NotNull] string qualifiedName);

        [CanBeNull]
        T FirstOrDefault<T>([NotNull] Database database, Guid guid) where T : DatabaseProjectItem;

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

        void Remove([NotNull] IProjectItem projectItem);

        [NotNull, ItemNotNull]
        IEnumerable<T> Where<T>([NotNull] ISourceFile sourceFile) where T : class, IProjectItem;
    }
}
