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
        public NugetWebsitePackageProvider([NotNull] IConfiguration configuration, [NotNull] ICacheService cache)
        {
            Configuration = configuration;
            Cache = cache;
        }

        [NotNull]
        public string InstallableRepository { get; private set; }

        [NotNull]
        public string InstalledRepository { get; private set; }

        [Diagnostics.NotNull]
        protected ICacheService Cache { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public override IEnumerable<Pathfinder.Packaging.IPackage> FindInstallableWebsitePackagesById(string packageId)
        {
            var installableRepository = GetInstallableRepository();

            var packages = installableRepository.FindPackagesById(packageId);
            if (packages == null || !packages.Any())
            {
                return Enumerable.Empty<Pathfinder.Packaging.IPackage>();
            }

            return packages.Select(p => new NugetPackage(p));
        }

        public override IEnumerable<Pathfinder.Packaging.IPackage> FindInstalledWebsitePackagesById(string packageId)
        {
            var installedRepository = GetInstalledRepository();

            var packages = installedRepository.FindPackagesById(packageId);
            if (packages == null || !packages.Any())
            {
                return Enumerable.Empty<Pathfinder.Packaging.IPackage>();
            }

            return packages.Select(p => new NugetPackage(p));
        }

        public override IEnumerable<Pathfinder.Packaging.IPackage> GetInstallableWebsitePackages(string queryText, string author, string tags, int skip)
        {
            var query = GetInstallablePackagesQuery(queryText, author, tags);

            if (skip >= 0)
            {
                query = query.Skip(skip).Take(10);
            }

            return query.Select(p => new NugetPackage(p));
        }

        public override IEnumerable<Pathfinder.Packaging.IPackage> GetInstalledWebsitePackages()
        {
            var installedRepository = GetInstalledRepository();

            return installedRepository.GetPackages().Select(p => new NugetPackage(p)
            {
                IsInstalled = true
            });
        }

        public override int GetTotalWebsitePackageCount(string queryText, string author, string tags)
        {
            var query = GetInstallablePackagesQuery(queryText, author, tags);
            return query.Count();
        }

        public override IEnumerable<Pathfinder.Packaging.IPackage> GetWebsiteUpdatePackages(bool includePrerelease)
        {
            var repository = GetInstallableRepository();

            var installedNugetPackages = GetInstalledWebsitePackages().OfType<NugetPackage>().Select(p => p.Package);

            return repository.GetUpdates(installedNugetPackages, includePrerelease, false).Select(p => new NugetPackage(p));
        }

        public override bool InstallOrUpdateWebsitePackage(string packageId)
        {
            var installableRepository = GetInstallableRepository();
            var installablePackage = installableRepository.FindPackage(packageId);
            if (installablePackage == null)
            {
                return false;
            }

            var installedRepository = GetInstalledRepository();
            var installedPackages = installedRepository.GetPackages().ToList();

            var packageManager = new PackageManager(installableRepository, InstalledRepository);
            packageManager.PackageInstalled += InstallPackage;

            var installedPackage = installedPackages.FirstOrDefault(i => string.Equals(i.Id, packageId, StringComparison.OrdinalIgnoreCase));
            if (installedPackage != null)
            {
                if (installedPackage.Version == installablePackage.Version)
                {
                    packageManager.UninstallPackage(installedPackage, true);
                    packageManager.InstallPackage(installablePackage, false, true);
                }
                else
                {
                    packageManager.UpdatePackage(installablePackage, false, true);
                }
            }
            else
            {
                packageManager.InstallPackage(installablePackage, false, true);
            }

            return true;
        }

        public override bool InstallPackage(string packageId, string version)
        {
            var installableRepository = GetInstallableRepository();

            IPackage package;
            if (string.IsNullOrEmpty(version))
            {
                package = installableRepository.FindPackage(packageId);
            }
            else
            {
                var semanticVersion = new SemanticVersion(version);
                package = installableRepository.FindPackage(packageId, semanticVersion);
            }

            if (package == null)
            {
                return false;
            }

            var packageManager = new PackageManager(installableRepository, InstalledRepository);
            packageManager.PackageInstalled += InstallPackage;

            packageManager.InstallPackage(package, false, true);

            return true;
        }

        public override bool UninstallWebsitePackage(string packageId)
        {
            var installedRepository = GetInstalledRepository();

            var package = installedRepository.FindPackage(packageId);
            if (package == null)
            {
                return false;
            }

            var packageManager = new PackageManager(installedRepository, InstalledRepository);

            packageManager.UninstallPackage(package, true);

            return true;
        }

        public override bool UpdateWebsitePackage(string packageId, string version)
        {
            var installedRepository = GetInstalledRepository();

            IPackage package;
            if (string.IsNullOrEmpty(version))
            {
                package = installedRepository.FindPackage(packageId);
            }
            else
            {
                var semanticVersion = new SemanticVersion(version);
                package = installedRepository.FindPackage(packageId, semanticVersion);
            }

            if (package == null)
            {
                return false;
            }

            var packageManager = new PackageManager(installedRepository, InstalledRepository);
            packageManager.PackageInstalled += InstallPackage;

            packageManager.UpdatePackage(package, false, true);

            return true;
        }

        protected void CreateDirectories()
        {
            // todo: make this configurable
            var pathfinder = Path.Combine(Configuration.GetString(Constants.Configuration.DataFolderDirectory), "pathfinder");
            InstallableRepository = Path.Combine(pathfinder, "packages");
            InstalledRepository = Path.Combine(pathfinder, "installed");

            if (!Directory.Exists(pathfinder))
            {
                Directory.CreateDirectory(pathfinder);
            }

            if (!Directory.Exists(InstallableRepository))
            {
                Directory.CreateDirectory(InstallableRepository);
            }

            if (!Directory.Exists(InstalledRepository))
            {
                Directory.CreateDirectory(InstalledRepository);
            }
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<IPackage> GetInstallablePackagesQuery([NotNull] string queryText, [NotNull] string author, [NotNull] string tags)
        {
            var installableRepository = GetInstallableRepository();

            var query = installableRepository.GetPackages();

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
        protected virtual IPackageRepository GetInstallableRepository()
        {
            CreateDirectories();

            var repositories = new List<IPackageRepository>
            {
                PackageRepositoryFactory.Default.CreateRepository(InstallableRepository)
            };

            var packageSources = Path.Combine(Configuration.GetString(Constants.Configuration.WebsiteDirectory), "sitecore/shell/client/Applications/Pathfinder/PackageSources.txt");
            if (File.Exists(packageSources))
            {
                var sources = File.ReadAllLines(packageSources);
                foreach (var source in sources)
                {
                    if (string.IsNullOrEmpty(source.Trim()))
                    {
                        continue;
                    }

                    repositories.Add(PackageRepositoryFactory.Default.CreateRepository(source));
                }
            }

            return new AggregateRepository(repositories);
        }

        [NotNull]
        protected virtual IPackageRepository GetInstalledRepository()
        {
            CreateDirectories();

            return PackageRepositoryFactory.Default.CreateRepository(InstalledRepository);
        }

        protected virtual void InstallPackage([CanBeNull] object sender, [NotNull] PackageOperationEventArgs e)
        {
            // check if this is a Pathfinder NuGet package
            var configFileName = Path.Combine(e.InstallPath, "project\\sitecore.tools\\scconfig.json");
            if (File.Exists(configFileName))
            {
                var projectDirectory = Path.Combine(e.InstallPath, "project");
                var toolsDirectory = Path.Combine(projectDirectory, "sitecore.tools");

                var binDirectory = Path.Combine(Configuration.GetString(Constants.Configuration.WebsiteDirectory), "bin");

                var app = new Startup().WithToolsDirectory(toolsDirectory).WithProjectDirectory(projectDirectory).WithExtensionsDirectory(binDirectory).Start();
                if (app == null)
                {
                    return;
                }

                var emitter = app.CompositionService.Resolve<IEmitterService>();
                emitter.Start();
            }

            var packagesDirectory = Path.Combine(e.InstallPath, "project\\packages");
            if (Directory.Exists(packagesDirectory))
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
