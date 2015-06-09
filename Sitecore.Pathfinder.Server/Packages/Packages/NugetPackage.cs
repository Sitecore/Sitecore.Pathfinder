// © 2015 Sitecore Corporation A/S. All rights reserved.

using NuGet;

namespace Sitecore.Pathfinder.Packages.Packages
{
    public class NugetPackage : PackageBase
    {
        public NugetPackage([NotNull] IPackage package)
        {
            Package = package;
        }

        public override string Name => Package.Title;

        [NotNull]
        public IPackage Package { get; }

        public override string PackageId => Package.Id;

        public override string Status => "Available";

        public override SemanticVersion Version => Package.Version;
    }
}
