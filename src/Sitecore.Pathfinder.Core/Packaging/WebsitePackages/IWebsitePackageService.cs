// © 2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging.WebsitePackages
{
    public interface IWebsitePackageService
    {
        [NotNull, ItemNotNull]
        IEnumerable<IPackage> CheckForRemoteUpdates([NotNull, ItemNotNull] IEnumerable<IPackage> installablePackages);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> CheckForLocalUpdates([NotNull, ItemNotNull] IEnumerable<IPackage> installedPackages);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> FindRemotePackagesById([NotNull] string packageId);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> FindLocalPackagesById([NotNull] string packageId);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetRemotePackages([NotNull] string queryText, [NotNull] string author, [NotNull] string tags, int skip = -1);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetLocalPackages();

        int GetTotalPackageCount([NotNull] string queryText, [NotNull] string author, [NotNull] string tags);

        void InstallOrUpdatePackage([NotNull] string packageId);

        void InstallPackage([NotNull] string packageId, [NotNull] string version);

        void UninstallPackage([NotNull] string packageId);

        void UpdatePackage([NotNull] string packageId, [NotNull] string version);

        [NotNull]
        IWebsitePackageService With([NotNull, ItemNotNull] IEnumerable<string> feeds);
    }
}
