// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

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
