// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Sitecore.Pathfinder.Packaging.ProjectPackages
{
    [InheritedExport(typeof(IProjectPackageProvider))]
    public abstract class ProjectPackageProviderBase : IProjectPackageProvider
    {
        public abstract bool CopyPackageToWebsite(IProjectPackageInfo projectPackageInfo, string destinationDirectory);

        public abstract IEnumerable<IProjectPackageInfo> GetPackages(string projectDirectory);

        public abstract bool RestorePackage(string packageId, string version, string projectDirectory);
    }
}
