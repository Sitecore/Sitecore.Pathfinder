// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Framework.ConfigurationModel;
using NuGet;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Packaging.ProjectPackages;
using Sitecore.Pathfinder.Runtime.Caching;

namespace Sitecore.Pathfinder.NuGet.Packaging.ProjectPackages
{
    public class NugetProjectPackageProvider : ProjectPackageProviderBase
    {
        [ImportingConstructor]
        public NugetProjectPackageProvider([NotNull] IConfiguration configuration, [Diagnostics.NotNull] IFileSystemService fileSystem, [NotNull] ICacheService cache)
        {
            Configuration = configuration;
            FileSystem = fileSystem;
            Cache = cache;
        }

        [Diagnostics.NotNull]
        protected ICacheService Cache { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override bool CopyPackageToWebsite(IProjectPackageInfo projectPackageInfo, string destinationDirectory)
        {
            var nupkgFileName = projectPackageInfo.Id + "." + projectPackageInfo.Version + ".nupkg";

            var srcFileName = Path.Combine(projectPackageInfo.PackageDirectory, nupkgFileName);
            if (!FileSystem.FileExists(srcFileName))
            {
                return false;
            }

            var destFileName = Path.Combine(destinationDirectory, nupkgFileName);
            if (FileSystem.FileExists(destFileName))
            {
                return false;
            }

            if (FileSystem.GetLastWriteTimeUtc(srcFileName) <= FileSystem.GetLastWriteTimeUtc(destFileName))
            {
                return false;
            }

            FileSystem.Copy(srcFileName, destFileName);
            return true;
        }

        public override IEnumerable<IProjectPackageInfo> GetPackages(string projectDirectory)
        {
            if (!Configuration.GetBool(Constants.Configuration.Packages.IncludePackagesConfigAsDependencies, true))
            {
                return Enumerable.Empty<IProjectPackageInfo>();
            }

            var projectPackages = Cache.Get<List<IProjectPackageInfo>>(Constants.Cache.ProjectPackages);
            if (projectPackages != null)
            {
                return projectPackages;
            }

            projectPackages = new List<IProjectPackageInfo>();

            // add packages from packages.config
            var packages = Path.Combine(projectDirectory, "packages.config");
            if (FileSystem.FileExists(packages))
            {
                var doc = FileSystem.ReadXml(packages);

                var root = doc.Root;
                if (root != null)
                {
                    foreach (var element in root.Elements())
                    {
                        var id = element.GetAttributeValue("id");
                        var version = element.GetAttributeValue("version");
                        var directory = Path.Combine(projectDirectory, Configuration.GetString(Constants.Configuration.Packages.NugetDirectory) + "\\" + id + "." + version);
                        var project = Path.Combine(directory, "project");

                        projectPackages.Add(new ProjectPackageInfo(id, version, directory, project));
                    }
                }
            }

            Cache.Set(Constants.Cache.ProjectPackages, packages);

            return projectPackages;
        }

        public override bool RestorePackage(string packageId, string version, string projectDirectory)
        {
            var repository = Cache.Get<IPackageRepository>(Constants.Cache.NugetRepositories);
            if (repository == null)
            {
                // add default repository which is located in the tools directory
                var packageDirectory = PathHelper.NormalizeFilePath(Path.Combine(Configuration.GetToolsDirectory(), Configuration.GetString(Constants.Configuration.RestorePackages.Directory)));
                var repositories = new List<IPackageRepository>
                {
                    PackageRepositoryFactory.Default.CreateRepository(packageDirectory)
                };

                // add source repositories from configuration
                foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.NugetRepositories))
                {
                    var source = Configuration.GetString(Constants.Configuration.NugetRepositories + ":" + pair.Key);

                    // make relative to project directory, unless the source has a protocol
                    if (source.IndexOf("://", StringComparison.Ordinal) < 0)
                    {
                        source = PathHelper.Combine(projectDirectory, source);
                    }

                    repositories.Add(PackageRepositoryFactory.Default.CreateRepository(source));
                }

                repository = new AggregateRepository(repositories);

                Cache.Set(Constants.Cache.NugetRepositories, repository);
            }

            IPackage package;
            if (string.IsNullOrEmpty(version))
            {
                package = repository.FindPackage(packageId);
            }
            else
            {
                var semanticVersion = new SemanticVersion(version);
                package = repository.FindPackage(packageId, semanticVersion, true, false);

                // if package was not found, try this weird hack
                if (package == null)
                {
                    // ReSharper disable once UnusedVariable
                    var dummy = repository.GetPackages().Count();
                    package = repository.FindPackage(packageId, semanticVersion, true, false);
                }

                // still not found, try this then
                if (package == null)
                {
                    package = repository.FindPackage(packageId);
                    if (package != null && package.Version != semanticVersion)
                    {
                        package = null;
                    }
                }
            }

            if (package == null)
            {
                return false;
            }

            var path = Path.Combine(projectDirectory, Configuration.GetString(Constants.Configuration.Packages.NugetDirectory));

            var packageManager = new PackageManager(repository, path);
            packageManager.InstallPackage(package, false, true);

            return true;
        }
    }
}
