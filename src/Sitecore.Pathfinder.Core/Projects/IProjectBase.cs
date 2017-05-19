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
        [NotNull]
        ProjectContext Context { get; }

        [ItemNotNull, NotNull]
        IEnumerable<Database> Databases { get; }

        [NotNull, ItemNotNull]
        IEnumerable<File> Files { get; }

        [NotNull]
        ProjectIndexes.ProjectIndexes Indexes { get; }

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

        [NotNull]
        Database GetDatabase([NotNull] string databaseName);
    }
}
