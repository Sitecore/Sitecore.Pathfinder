// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging.ProjectPackages
{
    public interface IProjectPackageService
    {
        bool CopyPackageToWebsite([NotNull] IProjectPackageInfo projectPackageInfo, [NotNull] string destinationDirectory);

        [NotNull, ItemNotNull, Pure]
        IEnumerable<IProjectPackageInfo> GetPackages();

        void RestorePackage([NotNull] string packageId, [NotNull] string version, [NotNull] string projectDirectory);
    }
}
