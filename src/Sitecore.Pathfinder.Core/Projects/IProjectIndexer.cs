// © 2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
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
        T FirstOrDefault<T>(Guid guid) where T : class, IProjectItem;

        [CanBeNull]
        T FirstOrDefault<T>([NotNull] Database database, Guid guid) where T : DatabaseProjectItem;

        [CanBeNull]
        T FirstOrDefault<T>([NotNull] string qualifiedName) where T : class, IProjectItem;

        [CanBeNull]
        T FirstOrDefault<T>([NotNull] Database database, [NotNull] string qualifiedName) where T : DatabaseProjectItem;

        [CanBeNull]
        T FirstOrDefault<T>([NotNull] ProjectItemUri uri) where T : class, IProjectItem;

        void Remove([NotNull] IProjectItem projectItem);

        [NotNull, ItemNotNull]
        IEnumerable<T> Where<T>([NotNull] ISourceFile sourceFile) where T : class, IProjectItem;

        [NotNull, ItemNotNull]
        IEnumerable<Item> WhereChildOf([NotNull] Item item);

        [NotNull, ItemNotNull]
        IEnumerable<T> WhereQualifiedName<T>([NotNull] Database database, [NotNull] string qualifiedName) where T : DatabaseProjectItem;

        [NotNull, ItemNotNull]
        IEnumerable<T> WhereShortName<T>([NotNull] Database database, [NotNull] string shortName) where T : DatabaseProjectItem;
    }
}
