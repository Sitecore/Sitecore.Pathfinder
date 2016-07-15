// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks.Building
{
    public class NugetOutputFile : OutputFile
    {
        public NugetOutputFile([NotNull] string fileName, [NotNull] string packageId) : base(fileName)
        {
            PackageId = packageId;
        }

        [NotNull]
        public string PackageId { get; }
    }
}
