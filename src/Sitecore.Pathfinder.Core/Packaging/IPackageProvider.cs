// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging
{
    public interface IPackageProvider
    {
        bool DownloadPackage([NotNull] string packageId, [NotNull] string version, [NotNull] string fileName);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> FindInstallablePackagesById([NotNull] string packageId);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> FindInstalledPackagesById([NotNull] string packageId);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetInstallablePackages([NotNull] string queryText, [NotNull] string author, [NotNull] string tags, int skip);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetInstalledPackages();

        int GetTotalPackageCount([NotNull] string queryText, [NotNull] string author, [NotNull] string tags);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetUpdatePackages(bool includePrerelease);

        bool InstallOrUpdatePackage([NotNull] string packageId);

        bool InstallPackage([NotNull] string packageId, [NotNull] string version);

        bool UninstallPackage([NotNull] string packageId);

        bool UpdatePackage([NotNull] string packageId, [NotNull] string version);
    }
}
