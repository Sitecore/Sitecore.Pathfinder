// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Packaging.ProjectPackages;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class RestorePackages : BuildTaskBase
    {
        [ImportingConstructor]
        public RestorePackages([NotNull] IProjectPackageService projectPackages) : base("restore-packages")
        {
            ProjectPackages = projectPackages;
        }

        [NotNull]
        protected IProjectPackageService ProjectPackages { get; }

        public override void Run(IBuildContext context)
        {
            var sourceDirectory = context.Configuration.GetString(Constants.Configuration.CopyDependenciesSourceDirectory);
            sourceDirectory = Path.Combine(context.ProjectDirectory, sourceDirectory);
            if (!context.FileSystem.DirectoryExists(sourceDirectory))
            {
                context.Trace.TraceInformation(Msg.D1003, Texts.Dependencies_directory_not_found__Skipping, sourceDirectory);
                return;
            }

            // todo: replace with official NuGet RestorePackages, if such exists
            foreach (var packageInfo in ProjectPackages.GetPackages(context.ProjectDirectory))
            {
                if (context.FileSystem.DirectoryExists(packageInfo.PackageDirectory))
                {
                    continue;
                }

                context.Trace.TraceInformation(Msg.D1000, Texts.Restoring, packageInfo.Id + "." + packageInfo.Version);

                ProjectPackages.RestorePackage(packageInfo.Id, packageInfo.Version, context.ProjectDirectory);
            }
        }
    }
}
