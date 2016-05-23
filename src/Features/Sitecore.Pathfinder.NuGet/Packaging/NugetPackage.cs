// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using NuGet;
using Sitecore.Pathfinder.Packaging;
using IPackage = NuGet.IPackage;

namespace Sitecore.Pathfinder.NuGet.Packaging
{
    public class NugetPackage : PackageBase
    {
        public NugetPackage([NotNull] IPackage package)
        {
            Package = package;

            PackageId = Package.Id;
            Name = Package.Title;
            Version = Package.Version.ToNormalizedString();
            Description = Package.Description;
            IconUrl = Package.IconUrl?.ToString() ?? string.Empty;
            Authors = string.Join(", ", Package.Authors);
            Owners = string.Join(", ", Package.Owners);
            LicenseUrl = Package.LicenseUrl?.ToString() ?? string.Empty;
            ProjectUrl = Package.ProjectUrl?.ToString() ?? string.Empty;
            Copyright = Package.Copyright ?? string.Empty;

            if (Package.Published != null)
            {
                Published = Package.Published.Value.DateTime;
            }

            Tags = Package.Tags;

            if (Package.DependencySets.Any())
            {
                foreach (var set in Package.DependencySets)
                {
                    if (set.TargetFramework == null)
                    {
                        continue;
                    }

                    if (!set.Dependencies.Any())
                    {
                        continue;
                    }

                    var dependencySet = new Pathfinder.Packaging.WebsitePackages.WebsitePackageDependencySet(set.TargetFramework.Identifier, set.TargetFramework.Version.ToString());
                    DependencySets.Add(dependencySet);

                    foreach (var dep in set.Dependencies)
                    {
                        var dependency = new Pathfinder.Packaging.WebsitePackages.WebsitePackageDependency(dep.Id, VersionUtility.PrettyPrint(dep.VersionSpec));
                        dependencySet.Dependencies.Add(dependency);
                    }
                }
            }
        }

        [NotNull]
        public IPackage Package { get; }

        public override int CompareTo(Pathfinder.Packaging.IPackage package)
        {
            return Package.Version.CompareTo(((NugetPackage)package).Package.Version);
        }
    }
}
