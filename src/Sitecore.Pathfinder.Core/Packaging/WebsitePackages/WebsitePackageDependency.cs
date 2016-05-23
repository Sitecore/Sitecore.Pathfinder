// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging.WebsitePackages
{
    public class WebsitePackageDependency
    {
        public WebsitePackageDependency([NotNull] string id, [NotNull] string version)
        {
            Id = id;
            Version = version;
        }

        [NotNull]
        public string Id { get; }

        [NotNull]
        public string Version { get; }
    }
}
