// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Packaging.WebsitePackages;

namespace Sitecore.Pathfinder.Packaging.ProjectPackages
{
    [Export(typeof(IProjectPackageService))]
    public class ProjectPackageService : IProjectPackageService
    {
        [ImportingConstructor]
        public ProjectPackageService([NotNull, ItemNotNull, ImportMany(typeof(IProjectPackageProvider))] IEnumerable<IProjectPackageProvider> packageProviders)
        {
            PackageProviders = packageProviders;
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<IProjectPackageProvider> PackageProviders { get; }

        public virtual bool CopyPackageToWebsite(IProjectPackageInfo projectPackageInfo, string destinationDirectory)
        {
            return PackageProviders.Any(packageProvider => packageProvider.CopyPackageToWebsite(projectPackageInfo, destinationDirectory));
        }

        public virtual IEnumerable<IProjectPackageInfo> GetPackages(string projectDirectory)
        {
            return PackageProviders.SelectMany(packageProvider => packageProvider.GetPackages(projectDirectory));
        }

        public void RestorePackage(string packageId, string version, string projectDirectory)
        {
            var restored = PackageProviders.Any(packageProvider => packageProvider.RestorePackage(packageId, version, projectDirectory));
            if (!restored)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }
        }
    }
}
