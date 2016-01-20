// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging
{
    public class PackageDependencySet
    {
        public PackageDependencySet([NotNull] string targetFramework, [NotNull] string version)
        {
            TargetFramework = targetFramework;
            Version = version;
        }

        [NotNull, ItemNotNull]
        public ICollection<PackageDependency> Dependencies { get; } = new List<PackageDependency>();

        [NotNull]
        public string TargetFramework { get; }

        [NotNull]
        public string Version { get; }
    }
}
