// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Sitecore.Pathfinder.Packaging
{
    [InheritedExport(typeof(IPackageProvider))]
    public abstract class PackageProviderBase : IPackageProvider
    {
        public abstract IEnumerable<IPackage> FindInstalledPackagesById(string packageId);

        public abstract IEnumerable<IPackage> FindInstallablePackagesById(string packageId);

        public abstract IEnumerable<IPackage> GetInstallablePackages(string queryText, string author, string tags, int skip);

        public abstract IEnumerable<IPackage> GetInstalledPackages();

        public abstract int GetTotalPackageCount(string queryText, string author, string tags);

        public abstract IEnumerable<IPackage> GetUpdatePackages(bool includePrerelease);

        public abstract bool InstallOrUpdatePackage(string packageId);

        public abstract bool InstallPackage(string packageId, string version);

        public abstract bool UninstallPackage(string packageId);

        public abstract bool UpdatePackage(string packageId, string version);

        public abstract bool DownloadPackage(string packageId, string version, string fileName);
    }
}
