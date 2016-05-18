// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
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

        public virtual IEnumerable<IProjectPackageInfo> GetPackages(string projectDirectory)
        {
            // add packages from configuration
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.Dependencies))
            {
                var id = pair.Key;
                var version = Configuration.GetString(Constants.Configuration.Dependencies + ":" + id);

                // todo: add support for NPM packages
                var directory = Path.Combine(projectDirectory, Configuration.GetString(Constants.Configuration.PackagesNugetDirectory) + "\\" + id + "." + version);

                var project = Path.Combine(directory, "project");
                yield return new ProjectPackageInfo(id, version, directory, project);
            }

            foreach (var package in PackageProviders.SelectMany(packageProvider => packageProvider.GetPackages(projectDirectory)))
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
