// © 2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Packaging.WebsitePackages
{
    [Export(typeof(IWebsitePackageService))]
    public class WebsitePackageService : IWebsitePackageService
    {
        [ImportingConstructor]
        public WebsitePackageService([NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem, [NotNull, ItemNotNull, ImportMany(typeof(IWebsitePackageProvider))] IEnumerable<IWebsitePackageProvider> packageProviders)
        {
            Configuration = configuration;
            FileSystem = fileSystem;
            PackageProviders = packageProviders;
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<string> Feeds { get; private set; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IWebsitePackageProvider> PackageProviders { get; }

        public virtual IEnumerable<IPackage> CheckForLocalUpdates(IEnumerable<IPackage> installedPackages)
        {
            var installablePackages = GetRemotePackages(string.Empty, string.Empty, string.Empty);

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

        public virtual IEnumerable<IPackage> CheckForRemoteUpdates(IEnumerable<IPackage> installablePackages)
        {
            var localPackages = GetLocalPackages();

            foreach (var installablePackage in installablePackages)
            {
                var installedPackage = localPackages.FirstOrDefault(p => string.Equals(p.PackageId, installablePackage.PackageId, StringComparison.OrdinalIgnoreCase));
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

        public virtual IEnumerable<IPackage> FindLocalPackagesById(string packageId)
        {
            return PackageProviders.SelectMany(packageProvider => packageProvider.FindLocalPackagesById(packageId));
        }

        public virtual IEnumerable<IPackage> FindRemotePackagesById(string packageId)
        {
            return PackageProviders.SelectMany(packageProvider => packageProvider.FindRemotePackagesById(packageId, Feeds));
        }

        public virtual IEnumerable<IPackage> GetLocalPackages()
        {
            return PackageProviders.SelectMany(packageProvider => packageProvider.GetLocalPackages());
        }

        public virtual IEnumerable<IPackage> GetRemotePackages(string queryText, string author, string tags, int skip = -1)
        {
            return PackageProviders.SelectMany(packageProvider => packageProvider.GetRemotePackages(queryText, author, tags, skip, Feeds));
        }

        public virtual int GetTotalPackageCount(string queryText, string author, string tags)
        {
            return PackageProviders.Sum(packageProvider => packageProvider.GetTotalPackageCount(queryText, author, tags, Feeds));
        }

        [NotNull, ItemNotNull]
        public virtual IEnumerable<IPackage> GetUpdatePackages(bool includePrerelease)
        {
            return PackageProviders.SelectMany(packageProvider => packageProvider.GetUpdatePackages(includePrerelease, Feeds));
        }

        public virtual void InstallOrUpdatePackage(string packageId)
        {
            var installed = PackageProviders.Any(packageProvider => packageProvider.InstallOrUpdatePackage(packageId, Feeds));
            if (!installed)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }}

        public virtual void InstallPackage(string packageId, string version)
        {
            var installed = PackageProviders.Any(packageProvider => packageProvider.InstallPackage(packageId, version, Feeds));
            if (!installed)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }
        }

        public virtual void UninstallPackage(string packageId)
        {
            var uninstalled = PackageProviders.Any(packageProvider => packageProvider.UninstallPackage(packageId));
            if (!uninstalled)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }
        }

        public virtual void UpdatePackage(string packageId, string version)
        {
            var updated = PackageProviders.Any(packageProvider => packageProvider.UpdatePackage(packageId, version));
            if (!updated)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }
        }

        public IWebsitePackageService With(IEnumerable<string> feeds)
        {
            var list = new List<string>(feeds);

            // add source repositories from sitecore/shell/client/Applications/Pathfinder/PackageSources.txt
            var packageSources = Path.Combine(Configuration.GetWebsiteDirectory(), "sitecore/shell/client/Applications/Pathfinder/PackageSources.txt");
            if (FileSystem.FileExists(packageSources))
            {
                var sources = FileSystem.ReadAllLines(packageSources);
                foreach (var source in sources)
                {
                    if (string.IsNullOrEmpty(source.Trim()))
                    {
                        continue;
                    }

                    list.Add(source);
                }
            }

            // add source repositories from configuration
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.NugetRepositories))
            {
                var source = Configuration.GetString(Constants.Configuration.NugetRepositories + ":" + pair.Key);

                // skip project relative sources
                if (source.IndexOf("://", StringComparison.Ordinal) >= 0)
                {
                    list.Add(source);
                }
            }

            Feeds = list;

            return this;
        }
    }
}
