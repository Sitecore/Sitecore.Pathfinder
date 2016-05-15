// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging.WebsitePackages
{
    [Export(typeof(IWebsitePackageService))]
    public class WebsitePackageService : IWebsitePackageService
    {
        [ImportingConstructor]
        public WebsitePackageService([NotNull, ItemNotNull, ImportMany(typeof(IWebsitePackageProvider))] IEnumerable<IWebsitePackageProvider> packageProviders)
        {
            PackageProviders = packageProviders;
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<IWebsitePackageProvider> PackageProviders { get; }

        public virtual IEnumerable<IPackage> CheckForInstallableUpdates(IEnumerable<IPackage> installablePackages)
        {
            var installedPackages = GetInstalledPackages();

            foreach (var installablePackage in installablePackages)
            {
                var installedPackage = installedPackages.FirstOrDefault(p => string.Equals(p.PackageId, installablePackage.PackageId, StringComparison.OrdinalIgnoreCase));
                if (installedPackage == null)
                {
                    continue;
                }

                installablePackage.IsInstalled = true;

                if (installedPackage.CompareTo(installablePackage) < 0)
                {
                    continue;
                }

                installablePackage.HasUpdate = true;
                installablePackage.UpdateVersion = installedPackage.Version;
            }

            return installablePackages;
        }

        public virtual IEnumerable<IPackage> CheckForInstalledUpdates(IEnumerable<IPackage> installedPackages)
        {
            var installablePackages = GetInstallablePackages(string.Empty, string.Empty, string.Empty);

            foreach (var installedPackage in installedPackages)
            {
                var installablePackage = installablePackages.FirstOrDefault(p => string.Equals(p.PackageId, installedPackage.PackageId, StringComparison.OrdinalIgnoreCase));
                if (installablePackage == null)
                {
                    continue;
                }

                if (installablePackage.CompareTo(installedPackage) >= 0)
                {
                    continue;
                }

                installedPackage.HasUpdate = true;
                installedPackage.UpdateVersion = installablePackage.Version;
            }

            return installedPackages;
        }

        public virtual IEnumerable<IPackage> FindInstallablePackagesById(string packageId)
        {
            return PackageProviders.SelectMany(packageProvider => packageProvider.FindInstallableWebsitePackagesById(packageId));
        }

        public virtual IEnumerable<IPackage> FindInstalledPackagesById(string packageId)
        {
            return PackageProviders.SelectMany(packageProvider => packageProvider.FindInstalledWebsitePackagesById(packageId));
        }

        public virtual IEnumerable<IPackage> GetInstallablePackages(string queryText, string author, string tags, int skip = -1)
        {
            return PackageProviders.SelectMany(packageProvider => packageProvider.GetInstallableWebsitePackages(queryText, author, tags, skip));
        }

        public virtual IEnumerable<IPackage> GetInstalledPackages()
        {
            return PackageProviders.SelectMany(packageProvider => packageProvider.GetInstalledWebsitePackages());
        }

        public virtual int GetTotalPackageCount(string queryText, string author, string tags)
        {
            return PackageProviders.Sum(packageProvider => packageProvider.GetTotalWebsitePackageCount(queryText, author, tags));
        }

        [NotNull, ItemNotNull]
        public virtual IEnumerable<IPackage> GetWebsiteUpdatePackages(bool includePrerelease)
        {
            return PackageProviders.SelectMany(packageProvider => packageProvider.GetWebsiteUpdatePackages(includePrerelease));
        }

        public virtual void InstallOrUpdatePackage(string packageId)
        {
            var installed = PackageProviders.Any(packageProvider => packageProvider.InstallOrUpdateWebsitePackage(packageId));
            if (!installed)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }
        }

        public virtual void InstallPackage(string packageId, string version)
        {
            var installed = PackageProviders.Any(packageProvider => packageProvider.InstallPackage(packageId, version));
            if (!installed)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }
        }

        public virtual void UninstallPackage(string packageId)
        {
            var uninstalled = PackageProviders.Any(packageProvider => packageProvider.UninstallWebsitePackage(packageId));
            if (!uninstalled)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }
        }

        public virtual void UpdatePackage(string packageId, string version)
        {
            var updated = PackageProviders.Any(packageProvider => packageProvider.UpdateWebsitePackage(packageId, version));
            if (!updated)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }
        }
    }
}
