using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProjectItemUri
    {
        [NotNull]
        string FileOrDatabaseName { get; }

        Guid Guid { get; }
    }
}