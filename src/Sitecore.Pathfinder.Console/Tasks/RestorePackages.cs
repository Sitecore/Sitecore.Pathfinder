// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Packaging.ProjectPackages;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class RestorePackages : BuildTaskBase
    {
        [ImportingConstructor]
        public RestorePackages([NotNull] IFileSystemService fileSystem, [NotNull] IProjectPackageService projectPackages) : base("restore-packages")
        {
            FileSystem = fileSystem;
            ProjectPackages = projectPackages;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        protected IProjectPackageService ProjectPackages { get; }

        public override void Run(IBuildContext context)
        {
            // todo: replace with official NuGet RestorePackages, if such exists
            foreach (var packageInfo in ProjectPackages.GetPackages(context.Project.ProjectDirectory))
            {
                if (FileSystem.DirectoryExists(packageInfo.PackageDirectory))
                {
                    continue;
                }

                context.Trace.TraceInformation(Msg.D1000, Texts.Restoring, packageInfo.Id + "." + packageInfo.Version);

                ProjectPackages.RestorePackage(packageInfo.Id, packageInfo.Version, context.Project.ProjectDirectory);
            }
        }
    }
}
