// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging.WebsitePackages
{
    public interface IWebsitePackageProvider
    {
        [NotNull, ItemNotNull]
        IEnumerable<IPackage> FindInstallableWebsitePackagesById([NotNull] string packageId);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> FindInstalledWebsitePackagesById([NotNull] string packageId);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetInstallableWebsitePackages([NotNull] string queryText, [NotNull] string author, [NotNull] string tags, int skip);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetInstalledWebsitePackages();

        int GetTotalWebsitePackageCount([NotNull] string queryText, [NotNull] string author, [NotNull] string tags);

        [NotNull, ItemNotNull]
        IEnumerable<IPackage> GetWebsiteUpdatePackages(bool includePrerelease);

        bool InstallOrUpdateWebsitePackage([NotNull] string packageId);

        bool InstallPackage([NotNull] string packageId, [NotNull] string version);

        bool UninstallWebsitePackage([NotNull] string packageId);

        bool UpdateWebsitePackage([NotNull] string packageId, [NotNull] string version);
    }
}
