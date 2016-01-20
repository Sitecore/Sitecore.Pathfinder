// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging
{
    public interface IPackage
    {
        [NotNull]
        string Authors { get; }

        [NotNull]
        string Copyright { get; }

        [NotNull, ItemNotNull]
        ICollection<PackageDependencySet> DependencySets { get; }

        [NotNull]
        string Description { get; }

        bool HasUpdate { get; set; }

        [NotNull]
        string IconUrl { get; }

        bool IsInstalled { get; set; }

        [NotNull]
        string LicenseUrl { get; }

        [NotNull]
        string Name { get; }

        [NotNull]
        string Owners { get; }

        [NotNull]
        string PackageId { get; }

        [NotNull]
        string ProjectUrl { get; }

        DateTime Published { get; }

        [NotNull]
        string Tags { get; }

        [NotNull]
        string UpdateVersion { get; set; }

        [NotNull]
        string Version { get; }

        int CompareTo([NotNull] IPackage package);
    }
}
