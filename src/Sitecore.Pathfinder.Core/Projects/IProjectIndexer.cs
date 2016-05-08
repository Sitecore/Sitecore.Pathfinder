// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProjectIndexer
    {
        void Add([NotNull] IProjectItem projectItem);

        [CanBeNull]
        T GetByGuid<T>(Guid guid) where T : class, IProjectItem;

        [CanBeNull]
        T GetByGuid<T>([NotNull] Database database, Guid guid) where T : DatabaseProjectItem;

        [CanBeNull]
        T GetByQualifiedName<T>([NotNull] string qualifiedName) where T : class, IProjectItem;

        [CanBeNull]
        T GetByQualifiedName<T>([NotNull] Database database, [NotNull] string qualifiedName) where T : DatabaseProjectItem;

        [CanBeNull]
        T GetByUri<T>([NotNull] ProjectItemUri uri) where T : class, IProjectItem;

        void Remove([NotNull] IProjectItem projectItem);
    }
}
