// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging
{
    public interface IPackageService
    {
        [NotNull, ItemNotNull]
        IEnumerable<IPackage> CheckForInstallableUpdates([NotNull, ItemNotNull] IEnumerable<IPackage> installablePackages);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> CheckForInstalledUpdates([NotNull, ItemNotNull] IEnumerable<IPackage> installedPackages);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> FindInstalledPackagesById([NotNull] string packageId);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> FindInstallablePackagesById([NotNull] string packageId);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetInstallablePackages([NotNull] string queryText, [NotNull] string author, [NotNull] string tags, int skip = -1);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetInstalledPackages();

        int GetTotalPackageCount([NotNull] string queryText, [NotNull] string author, [NotNull] string tags);

        void InstallOrUpdatePackage([NotNull] string packageId);

        void InstallPackage([NotNull] string packageId, [NotNull] string version);

        void UninstallPackage([NotNull] string packageId);

        void UpdatePackage([NotNull] string packageId, [NotNull] string version);
    }
}
