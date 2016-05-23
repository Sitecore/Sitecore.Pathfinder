// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging.WebsitePackages
{
    [InheritedExport(typeof(IWebsitePackageProvider))]
    public abstract class WebsitePackageProviderBase : IWebsitePackageProvider
    {
        public abstract IEnumerable<IPackage> FindRemotePackagesById(string packageId, IEnumerable<string> feeds);

        public abstract IEnumerable<IPackage> FindLocalPackagesById(string packageId);

        public abstract IEnumerable<IPackage> GetRemotePackages(string queryText, string author, string tags, int skip, IEnumerable<string> feeds);

        public abstract IEnumerable<IPackage> GetLocalPackages();

        public abstract int GetTotalPackageCount(string queryText, string author, string tags, IEnumerable<string> feeds);

        public abstract IEnumerable<IPackage> GetUpdatePackages(bool includePrerelease, IEnumerable<string> feeds);

        public abstract bool InstallOrUpdatePackage(string packageId, IEnumerable<string> feeds);

        public abstract bool InstallPackage(string packageId, string version, IEnumerable<string> feeds);

        public abstract bool UninstallPackage(string packageId);

        public abstract bool UpdatePackage(string packageId, string version);
    }
}
