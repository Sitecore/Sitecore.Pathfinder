// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Packaging.ProjectPackages
{
    [Export(typeof(IProjectPackageService))]
    public class ProjectPackageService : IProjectPackageService
    {
        [ImportingConstructor]
        public ProjectPackageService([NotNull] IConfiguration configuration, [NotNull, ItemNotNull, ImportMany(typeof(IProjectPackageProvider))] IEnumerable<IProjectPackageProvider> packageProviders)
        {
            Configuration = configuration;
            PackageProviders = packageProviders;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IProjectPackageProvider> PackageProviders { get; }

        public virtual bool CopyPackageToWebsite(IProjectPackageInfo projectPackageInfo, string destinationDirectory)
        {
            return PackageProviders.Any(packageProvider => packageProvider.CopyPackageToWebsite(projectPackageInfo, destinationDirectory));
        }

        public virtual IEnumerable<IProjectPackageInfo> GetPackages()
        {
            var packageRootDirectory = Configuration.GetString(Constants.Configuration.NugetPackageRootDirectory);

            // add packages from configuration
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.Dependencies))
            {
                var id = Configuration.GetString(Constants.Configuration.Dependencies + ":" + pair.Key + ":id");
                var version = Configuration.GetString(Constants.Configuration.Dependencies + ":" + pair.Key + ":version");

                // todo: add support for NPM packages
                var directory = Path.Combine(packageRootDirectory, id + "." + version);

                var project = Path.Combine(directory, "project");
                yield return new ProjectPackageInfo(id, version, directory, project);
            }

            // add packages from package providers
            foreach (var package in PackageProviders.SelectMany(packageProvider => packageProvider.GetPackages(packageRootDirectory)))
            {
                yield return package;
            }
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
