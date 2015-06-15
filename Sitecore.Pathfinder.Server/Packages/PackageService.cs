// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Packages.Packages;

namespace Sitecore.Pathfinder.Packages
{
    public class PackageService
    {
        public PackageService()
        {
            // todo: make this configurable
            var pathfinder = Path.Combine(Sitecore.Configuration.Settings.DataFolder, "pathfinder");
            AvailableRepository = Path.Combine(pathfinder, "packages");
            InstalledRepository = Path.Combine(pathfinder, "installed");

            if (!Directory.Exists(pathfinder))
            {
                Directory.CreateDirectory(pathfinder);
            }

            if (!Directory.Exists(AvailableRepository))
            {
                Directory.CreateDirectory(AvailableRepository);
            }

            if (!Directory.Exists(InstalledRepository))
            {
                Directory.CreateDirectory(InstalledRepository);
            }
        }

        [NotNull]
        public string AvailableRepository { get; }

        [NotNull]
        public string InstalledRepository { get; }

        [NotNull]
        public virtual IEnumerable<PackageBase> CheckForAvailableUpdates([NotNull] IEnumerable<PackageBase> availablePackages)
        {
            var installedPackages = GetInstalledPackages();

            foreach (var availablePackage in availablePackages)
            {
                var installedPackage = installedPackages.FirstOrDefault(p => string.Compare(p.PackageId, availablePackage.PackageId, StringComparison.OrdinalIgnoreCase) == 0);
                if (installedPackage == null)
                {
                    continue;
                }

                availablePackage.IsInstalled = true;

                if (installedPackage.Version >= availablePackage.Version)
                {
                    continue;
                }

                availablePackage.HasUpdate = true;
                availablePackage.UpdateVersion = installedPackage.Version;
            }

            return availablePackages;
        }

        [NotNull]
        public virtual IEnumerable<PackageBase> CheckForInstalledUpdates([NotNull] IEnumerable<PackageBase> installedPackages)
        {
            var availablePackages = GetAvailablePackages(string.Empty, string.Empty);

            foreach (var installedPackage in installedPackages)
            {
                var availablePackage = availablePackages.FirstOrDefault(p => string.Compare(p.PackageId, installedPackage.PackageId, StringComparison.OrdinalIgnoreCase) == 0);
                if (availablePackage == null)
                {
                    continue;
                }

                if (availablePackage.Version <= installedPackage.Version)
                {
                    continue;
                }

                installedPackage.HasUpdate = true;
                installedPackage.UpdateVersion = availablePackage.Version;
            }

            return installedPackages;
        }

        [NotNull]
        public virtual IEnumerable<PackageBase> GetAvailablePackages(string author, string tags)
        {
            var availableRepository = GetAvailableRepository();

            var query = availableRepository.GetPackages();

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(p => p.Authors.Any(a => string.Compare(a, author, StringComparison.OrdinalIgnoreCase) == 0));
            }

            if (!string.IsNullOrEmpty(tags))
            {
                query = query.Where(p => p.Tags != null && p.Tags.Contains(tags));
            }

            query = query.OrderByDescending(p => p.DownloadCount).ThenBy(p => p.Title);

            return query.GroupBy(p => p.Id).Select(y => y.OrderByDescending(p => p.Version).First()).Select(p => new NugetPackage(p)).ToList();
        }

        [NotNull]
        public virtual IEnumerable<PackageBase> GetInstalledPackages()
        {
            var installedRepository = GetInstalledRepository();

            return installedRepository.GetPackages().Select(p => new NugetPackage(p)
            {
                IsInstalled = true
            }).ToList();
        }

        [NotNull]
        public virtual IEnumerable<PackageBase> GetUpdatePackages(bool includePrerelease)
        {
            var repository = GetAvailableRepository();

            var installedNugetPackages = GetInstalledPackages().OfType<NugetPackage>().Select(p => p.Package);

            return repository.GetUpdates(installedNugetPackages, includePrerelease, false).Select(p => new NugetPackage(p)).ToList();
        }

        public virtual void InstallOrUpdatePackage([NotNull] string packageId)
        {
            var availableRepository = GetAvailableRepository();
            var availablePackage = availableRepository.FindPackage(packageId);
            if (availablePackage == null)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }

            var installedRepository = GetInstalledRepository();
            var installedPackages = installedRepository.GetPackages().ToList();

            var packageManager = new PackageManager(availableRepository, InstalledRepository);
            packageManager.PackageInstalled += InstallPackage;

            var installedPackage = installedPackages.FirstOrDefault(i => string.Compare(i.Id, packageId, StringComparison.OrdinalIgnoreCase) == 0);
            if (installedPackage != null)
            {
                if (installedPackage.Version == availablePackage.Version)
                {
                    packageManager.UninstallPackage(installedPackage, true);
                    packageManager.InstallPackage(availablePackage, false, true);
                }
                else
                {
                    packageManager.UpdatePackage(availablePackage, false, true);
                }
            }
            else
            {
                packageManager.InstallPackage(availablePackage, false, true);
            }
        }

        public virtual void InstallPackage([NotNull] string packageId)
        {
            var availableRepository = GetAvailableRepository();

            var package = availableRepository.FindPackage(packageId);
            if (package == null)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }

            var packageManager = new PackageManager(availableRepository, InstalledRepository);
            packageManager.PackageInstalled += InstallPackage;

            packageManager.InstallPackage(package, false, true);
        }

        public virtual void UninstallPackage([NotNull] string packageId)
        {
            var installedRepository = GetInstalledRepository();

            var package = installedRepository.FindPackage(packageId);
            if (package == null)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }

            var packageManager = new PackageManager(installedRepository, InstalledRepository);

            packageManager.UninstallPackage(package, true);
        }

        public virtual void UpdatePackage([NotNull] string packageId)
        {
            var availableRepository = GetAvailableRepository();

            var package = availableRepository.FindPackage(packageId);
            if (package == null)
            {
                throw new InvalidOperationException("Package not found: " + packageId);
            }

            var packageManager = new PackageManager(availableRepository, InstalledRepository);
            packageManager.PackageInstalled += InstallPackage;

            packageManager.UpdatePackage(package, false, true);
        }

        [NotNull]
        protected virtual IPackageRepository GetAvailableRepository()
        {
            return PackageRepositoryFactory.Default.CreateRepository(AvailableRepository);
        }



        [NotNull]
        protected virtual IPackageRepository GetInstalledRepository()
        {
            return PackageRepositoryFactory.Default.CreateRepository(InstalledRepository);
        }

        protected virtual void InstallPackage([CanBeNull] object sender, [NotNull] PackageOperationEventArgs e)
        {
            var emitService = new EmitService(e.InstallPath);
            emitService.Start();
        }
    }
}
