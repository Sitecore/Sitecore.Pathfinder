// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Packaging.ProjectPackages
{
    public class ProjectPackageInfo : IProjectPackageInfo
    {
        public ProjectPackageInfo([NotNull] string id, [NotNull] string version, [NotNull] string packageDirectory, [NotNull] string projectDirectory)
        {
            Id = id;
            PackageDirectory = packageDirectory;
            ProjectDirectory = projectDirectory;
            Version = version;
        }

        public string Id { get; }

        public string PackageDirectory { get; }

        public string ProjectDirectory { get; }

        public string Version { get; }
    }
}
