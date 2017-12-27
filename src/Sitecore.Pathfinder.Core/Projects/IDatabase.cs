using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Projects
{
    public interface IDatabase
    {
        [NotNull]
        string DatabaseName { get; }

        [NotNull, ItemNotNull]
        IEnumerable<Item> Items { get; }

        [NotNull, ItemNotNull]
        IEnumerable<Language> Languages { get; }

        [NotNull]
        IProjectBase Project { get; }

        [NotNull, ItemNotNull]
        IEnumerable<Template> Templates { get; }

        [CanBeNull]
        T FindByIdOrPath<T>([NotNull] string idOrPath) where T : DatabaseProjectItem;

        [CanBeNull]
        T FindQualifiedItem<T>([NotNull] string qualifiedName) where T : DatabaseProjectItem;

        [NotNull, ItemNotNull]
        IEnumerable<T> GetByQualifiedName<T>([NotNull] string qualifiedName) where T : DatabaseProjectItem;

        [NotNull, ItemNotNull]
        IEnumerable<T> GetByShortName<T>([NotNull] string shortName) where T : DatabaseProjectItem;

        [CanBeNull]
        Item GetItem([NotNull] string itemPath);

        [CanBeNull]
        Item GetItem(Guid guid);

        [CanBeNull]
        Item GetItem([NotNull] ProjectItemUri uri);

        [NotNull]
        Language GetLanguage([NotNull] string languageName);
    }
}