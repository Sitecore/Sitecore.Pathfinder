// © 2015 Sitecore Corporation A/S. All rights reserved.

using NuGet;

namespace Sitecore.Pathfinder.Packages.Packages
{
    public abstract class PackageBase
    {
        public bool HasUpdate { get; set; }

        public bool IsInstalled { get; set; }

        [Diagnostics.NotNull]
        public abstract string Name { get; }

        [Diagnostics.NotNull]
        public abstract string PackageId { get; }

        [Diagnostics.NotNull]
        public abstract string Status { get; }

        [Diagnostics.NotNull]
        public SemanticVersion UpdateVersion { get; set; }

        [Diagnostics.NotNull]
        public abstract SemanticVersion Version { get; }
    }
}
