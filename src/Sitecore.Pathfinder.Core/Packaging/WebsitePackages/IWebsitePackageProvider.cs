// © 2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging.WebsitePackages
{
    public interface IWebsitePackageProvider
    {
        [NotNull, ItemNotNull]
        IEnumerable<IPackage> FindLocalPackagesById([NotNull] string packageId);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> FindRemotePackagesById([NotNull] string packageId, [NotNull, ItemNotNull] IEnumerable<string> feeds);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetLocalPackages();

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetRemotePackages([NotNull] string queryText, [NotNull] string author, [NotNull] string tags, int skip, [NotNull, ItemNotNull] IEnumerable<string> feeds);

        int GetTotalPackageCount([NotNull] string queryText, [NotNull] string author, [NotNull] string tags, [NotNull, ItemNotNull] IEnumerable<string> feeds);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetUpdatePackages(bool includePrerelease, [NotNull, ItemNotNull] IEnumerable<string> feeds);

        bool InstallOrUpdatePackage([NotNull] string packageId, [NotNull, ItemNotNull] IEnumerable<string> feeds);

        bool InstallPackage([NotNull] string packageId, [NotNull] string version, [NotNull, ItemNotNull] IEnumerable<string> feeds);

        bool UninstallPackage([NotNull] string packageId);

        bool UpdatePackage([NotNull] string packageId, [NotNull] string version);
    }
}
