// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging.ProjectPackages
{
    public interface IProjectPackageProvider
    {
        bool CopyPackageToWebsite([NotNull] IProjectPackageInfo projectPackageInfo, [NotNull] string destinationDirectory);

        [NotNull, ItemNotNull]
        IEnumerable<IProjectPackageInfo> GetPackages([NotNull] string projectDirectory);

        bool RestorePackage([NotNull] string packageId, [NotNull] string version, [NotNull] string projectDirectory);
    }
}
