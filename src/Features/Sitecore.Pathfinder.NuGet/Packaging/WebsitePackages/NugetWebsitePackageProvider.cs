// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using NuGet;
using Sitecore.Data.Engines;
using Sitecore.Data.Proxies;
using Sitecore.Install;
using Sitecore.Install.Files;
using Sitecore.Install.Framework;
using Sitecore.Install.Items;
using Sitecore.Install.Metadata;
using Sitecore.Install.Utils;
using Sitecore.Install.Zip;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Packaging.WebsitePackages;
using Sitecore.Pathfinder.Runtime.Caching;
using Sitecore.SecurityModel;

namespace Sitecore.Pathfinder.NuGet.Packaging.WebsitePackages
{
    public class NugetWebsitePackageProvider : WebsitePackageProviderBase
    {
        [ImportingConstructor]
        public NugetWebsitePackageProvider([NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem, [NotNull] ICacheService cache)
        {
            Configuration = configuration;
            FileSystem = fileSystem;
            Cache = cache;
        }

        [NotNull]
        public string LocalRepository { get; private set; }

        [NotNull]
        public string ProjectDirectory { get; private set; }

        [Diagnostics.NotNull]
        protected ICacheService Cache { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override IEnumerable<Pathfinder.Packaging.IPackage> FindRemotePackagesById(string packageId, IEnumerable<string> feeds)
        {
            var remoteRepository = GetRemoteRepository(feeds);

            var packages = remoteRepository.FindPackagesById(packageId);
            if (packages == null || !packages.Any())
            {
                return Enumerable.Empty<Pathfinder.Packaging.IPackage>();
            }

            return packages.Select(p => new NugetPackage(p));
        }

        public override IEnumerable<Pathfinder.Packaging.IPackage> FindLocalPackagesById(string packageId)
        {
            var localRepository = GetLocalRepository();

            var packages = localRepository.FindPackagesById(packageId);
            if (packages == null || !packages.Any())
            {
                return Enumerable.Empty<Pathfinder.Packaging.IPackage>();
            }

            return packages.Select(p => new NugetPackage(p));
        }

        public override IEnumerable<Pathfinder.Packaging.IPackage> GetRemotePackages(string queryText, string author, string tags, int skip, IEnumerable<string> feeds)
        {
            var query = GetRemotePackagesQuery(queryText, author, tags, feeds);

            if (skip >= 0)
            {
                query = query.Skip(skip).Take(10);
            }

            return query.Select(p => new NugetPackage(p));
        }

        public override IEnumerable<Pathfinder.Packaging.IPackage> GetLocalPackages()
        {
            var installedRepository = GetLocalRepository();

            return installedRepository.GetPackages().Select(p => new NugetPackage(p)
            {
                IsInstalled = true
            });
        }

        public override int GetTotalPackageCount(string queryText, string author, string tags, IEnumerable<string> feeds)
        {
            var query = GetRemotePackagesQuery(queryText, author, tags, feeds);
            return query.Count();
        }

        public override IEnumerable<Pathfinder.Packaging.IPackage> GetUpdatePackages(bool includePrerelease, IEnumerable<string> feeds)
        {
            var repository = GetRemoteRepository(feeds);

            var installedNugetPackages = GetLocalPackages().OfType<NugetPackage>().Select(p => p.Package);

            return repository.GetUpdates(installedNugetPackages, includePrerelease, false).Select(p => new NugetPackage(p));
        }

        public override bool InstallOrUpdatePackage(string packageId, IEnumerable<string> feeds)
        {
            var remoteRepository = GetRemoteRepository(feeds);
            var remotePackage = remoteRepository.FindPackage(packageId);
            if (remotePackage == null)
            {
                return false;
            }

            var localRepository = GetLocalRepository();
            var localPackages = localRepository.GetPackages().ToList();

            var packageManager = new PackageManager(remoteRepository, LocalRepository);
            packageManager.PackageInstalled += InstallPackage;

            var localPackage = localPackages.FirstOrDefault(i => string.Equals(i.Id, packageId, StringComparison.OrdinalIgnoreCase));
            if (localPackage != null)
            {
                if (localPackage.Version == remotePackage.Version)
                {
                    packageManager.UninstallPackage(localPackage, true);
                    packageManager.InstallPackage(remotePackage, false, true);
                }
                else
                {
                    packageManager.UpdatePackage(remotePackage, false, true);
                }
            }
            else
            {
                packageManager.InstallPackage(remotePackage, false, true);
            }

            return true;
        }

        public override bool InstallPackage(string packageId, string version, IEnumerable<string> feeds)
        {
            var remoteRepository = GetRemoteRepository(feeds);

            IPackage package;
            if (string.IsNullOrEmpty(version))
            {
                package = remoteRepository.FindPackage(packageId);
            }
            else
            {
                var semanticVersion = new SemanticVersion(version);
                package = remoteRepository.FindPackage(packageId, semanticVersion);
            }

            if (package == null)
            {
                return false;
            }

            var packageManager = new PackageManager(remoteRepository, LocalRepository);
            packageManager.PackageInstalled += InstallPackage;

            packageManager.InstallPackage(package, false, true);

            return true;
        }

        public override bool UninstallPackage(string packageId)
        {
            var localRepository = GetLocalRepository();

            var package = localRepository.FindPackage(packageId);
            if (package == null)
            {
                return false;
            }

            var packageManager = new PackageManager(localRepository, LocalRepository);

            packageManager.UninstallPackage(package, true);

            return true;
        }

        public override bool UpdatePackage(string packageId, string version)
        {
            var localRepository = GetLocalRepository();

            IPackage package;
            if (string.IsNullOrEmpty(version))
            {
                package = localRepository.FindPackage(packageId);
            }
            else
            {
                var semanticVersion = new SemanticVersion(version);
                package = localRepository.FindPackage(packageId, semanticVersion);
            }

            if (package == null)
            {
                return false;
            }

            var packageManager = new PackageManager(localRepository, LocalRepository);
            packageManager.PackageInstalled += InstallPackage;

            packageManager.UpdatePackage(package, false, true);

            return true;
        }

        protected void CreateDirectories()
        {
            // todo: make this configurable
            var pathfinderDirectory = Path.Combine(Configuration.GetString(Constants.Configuration.DataFolderDirectory), "pathfinder");

            LocalRepository = Path.Combine(pathfinderDirectory, "packages");
            ProjectDirectory = Configuration.GetProjectDirectory();

            if (!Directory.Exists(pathfinderDirectory))
            {
                Directory.CreateDirectory(pathfinderDirectory);
            }

            if (!Directory.Exists(LocalRepository))
            {
                Directory.CreateDirectory(LocalRepository);
            }
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<IPackage> GetRemotePackagesQuery([NotNull] string queryText, [NotNull] string author, [NotNull] string tags, [NotNull, ItemNotNull] IEnumerable<string> feeds)
        {
            var remoteRepository = GetRemoteRepository(feeds);

            var query = remoteRepository.GetPackages();

            if (!string.IsNullOrEmpty(queryText))
            {
                query = query.Where(p => p.Id.IndexOf(queryText, StringComparison.InvariantCultureIgnoreCase) >= 0 || (p.Title != null && p.Title.IndexOf(queryText, StringComparison.InvariantCultureIgnoreCase) >= 0));
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(p => p.Authors.Any(a => string.Equals(a, author, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(tags))
            {
                query = query.Where(p => p.Tags != null && p.Tags.Contains(tags));
            }

            var tempQuery = query.ToList();

            return tempQuery.GroupBy(p => p.Id).Select(y => y.OrderByDescending(p => p.Version).First()).OrderBy(p => p.Title);
        }

        [NotNull]
        protected virtual IPackageRepository GetRemoteRepository([NotNull, ItemNotNull] IEnumerable<string> feeds)
        {
            var repositories = feeds.Select(feed => PackageRepositoryFactory.Default.CreateRepository(feed)).ToList();

            return new AggregateRepository(repositories);
        }

        [NotNull]
        protected virtual IPackageRepository GetLocalRepository()
        {
            CreateDirectories();

            return PackageRepositoryFactory.Default.CreateRepository(LocalRepository);
        }

        protected virtual void InstallPackage([CanBeNull] object sender, [NotNull] PackageOperationEventArgs e)
        {
            // check if this is a Pathfinder NuGet package
            var configFileName = Path.Combine(e.InstallPath, "project\\sitecore.tools\\scconfig.json");
            if (FileSystem.FileExists(configFileName))
            {
                var projectDirectory = Path.Combine(e.InstallPath, "project");
                var toolsDirectory = Path.Combine(projectDirectory, "sitecore.tools");

                var binDirectory = Path.Combine(Configuration.GetString(Constants.Configuration.WebsiteDirectory), "bin");

                var host = new Startup().WithToolsDirectory(toolsDirectory).WithProjectDirectory(projectDirectory).WithExtensionsDirectory(binDirectory).Start();
                if (host == null)
                {
                    return;
                }

                var pathfinderDirectory = Path.Combine(Configuration.GetString(Constants.Configuration.DataFolderDirectory), "pathfinder");

                var baseDirectory = Path.Combine(pathfinderDirectory, ".base\\" + e.Package.Id);

                var emitter = host.CompositionService.Resolve<IEmitterService>().With(baseDirectory);
                emitter.Start();
            }

            var packagesDirectory = Path.Combine(e.InstallPath, "project\\packages");
            if (FileSystem.DirectoryExists(packagesDirectory))
            {
                InstallPackageDirectory(packagesDirectory);
            }
        }

        protected virtual void InstallPackageDirectory([NotNull] string packagesDirectory)
        {
            if (!Directory.Exists(packagesDirectory))
            {
                return;
            }

            foreach (var fileName in Directory.GetFiles(packagesDirectory, "*.zip"))
            {
                InstallSitecorePackage(fileName);
            }
        }

        private void InstallSitecorePackage([NotNull] string fileName)
        {
            Context.SetActiveSite("shell");
            using (new SecurityDisabler())
            {
                using (new ProxyDisabler())
                {
                    using (new SyncOperationContext())
                    {
                        var context = new SimpleProcessingContext();

                        var defaultItemInstallerEvents = new DefaultItemInstallerEvents(new BehaviourOptions(InstallMode.Overwrite, MergeMode.Clear));
                        context.AddAspect(defaultItemInstallerEvents);

                        var defaultFileInstallerEvents = new DefaultFileInstallerEvents(true);
                        context.AddAspect(defaultFileInstallerEvents);

                        var installer = new Installer();
                        installer.InstallPackage(FileUtil.MapPath(fileName), context);

                        var packageReader = new PackageReader(FileUtil.MapPath(fileName));
                        var previewContext = Installer.CreatePreviewContext();
                        var view = new MetadataView(previewContext);
                        var sink = new MetadataSink(view);

                        sink.Initialize(previewContext);

                        packageReader.Populate(sink);

                        installer.ExecutePostStep(view.PostStep, previewContext);
                    }
                }
            }
        }
    }
}
