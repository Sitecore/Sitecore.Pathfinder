// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging.ProjectPackages
{
    public interface IProjectPackageInfo
    {
        [NotNull]
        string Id { get; }

        [NotNull]
        string PackageDirectory { get; }

        [NotNull]
        string ProjectDirectory { get; }

        [NotNull]
        string Version { get; }
    }
}
