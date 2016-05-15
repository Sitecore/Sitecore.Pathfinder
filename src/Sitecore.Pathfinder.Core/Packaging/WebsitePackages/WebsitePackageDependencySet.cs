// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging.WebsitePackages
{
    public class WebsitePackageDependencySet
    {
        public WebsitePackageDependencySet([NotNull] string targetFramework, [NotNull] string version)
        {
            TargetFramework = targetFramework;
            Version = version;
        }

        [NotNull, ItemNotNull]
        public ICollection<WebsitePackageDependency> Dependencies { get; } = new List<WebsitePackageDependency>();

        [NotNull]
        public string TargetFramework { get; }

        [NotNull]
        public string Version { get; }
    }
}
