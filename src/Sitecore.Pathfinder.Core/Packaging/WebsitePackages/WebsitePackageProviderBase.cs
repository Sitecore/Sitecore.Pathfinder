// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Sitecore.Pathfinder.Packaging.WebsitePackages
{
    [InheritedExport(typeof(IWebsitePackageProvider))]
    public abstract class WebsitePackageProviderBase : IWebsitePackageProvider
    {
        public abstract IEnumerable<IPackage> FindInstallableWebsitePackagesById(string packageId);

        public abstract IEnumerable<IPackage> FindInstalledWebsitePackagesById(string packageId);

        public abstract IEnumerable<IPackage> GetInstallableWebsitePackages(string queryText, string author, string tags, int skip);

        public abstract IEnumerable<IPackage> GetInstalledWebsitePackages();

        public abstract int GetTotalWebsitePackageCount(string queryText, string author, string tags);

        public abstract IEnumerable<IPackage> GetWebsiteUpdatePackages(bool includePrerelease);

        public abstract bool InstallOrUpdateWebsitePackage(string packageId);

        public abstract bool InstallPackage(string packageId, string version);

        public abstract bool UninstallWebsitePackage(string packageId);

        public abstract bool UpdateWebsitePackage(string packageId, string version);
    }
}
