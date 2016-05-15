// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Packaging.WebsitePackages;

namespace Sitecore.Pathfinder.Packaging
{
    public abstract class PackageBase : IPackage
    {
        public string Authors { get; protected set; } = string.Empty;

        public string Copyright { get; protected set; } = string.Empty;

        public ICollection<WebsitePackageDependencySet> DependencySets { get; } = new List<WebsitePackageDependencySet>();

        public string Description { get; protected set; } = string.Empty;

        public bool HasUpdate { get; set; }

        public string IconUrl { get; protected set; } = string.Empty;

        public bool IsInstalled { get; set; }

        public string LicenseUrl { get; protected set; } = string.Empty;

        public string Name { get; protected set; } = string.Empty;

        public string Owners { get; protected set; } = string.Empty;

        public string PackageId { get; protected set; } = string.Empty;

        public string ProjectUrl { get; protected set; } = string.Empty;

        public DateTime Published { get; protected set; } = DateTime.MinValue;

        public string Tags { get; protected set; } = string.Empty;

        public string UpdateVersion { get; set; } = string.Empty;

        public string Version { get; protected set; } = string.Empty;

        public abstract int CompareTo(IPackage package);
    }
}
