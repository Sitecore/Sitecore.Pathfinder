// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Packaging;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class RestorePackages : BuildTaskBase
    {
        [ImportingConstructor]
        public RestorePackages([NotNull] IPackageService packageService) : base("restore-packages")
        {
            PackageService = packageService;
        }

        [NotNull]
        protected IPackageService PackageService { get; }

        public override void Run(IBuildContext context)
        {
            var sourceDirectory = context.Configuration.Get(Constants.Configuration.CopyDependenciesSourceDirectory);
            sourceDirectory = Path.Combine(context.ProjectDirectory, sourceDirectory);
            if (!context.FileSystem.DirectoryExists(sourceDirectory))
            {
                context.Trace.TraceInformation(Msg.D1003, Texts.Dependencies_directory_not_found__Skipping, sourceDirectory);
                return;
            }

            foreach (var pair in context.Configuration.GetSubKeys("dependencies"))
            {
                var packageId = pair.Key;
                var version = context.Configuration.GetString("dependencies:" + packageId);

                var fileName = Path.Combine(sourceDirectory, packageId + "." + version + ".nupkg");
                if (context.FileSystem.FileExists(fileName))
                {
                    continue;
                }

                context.Trace.TraceInformation(Msg.D1000, Texts.Restoring, packageId + "." + version);

                PackageService.DownloadPackage(packageId, version, fileName);
            }
        }
    }
}
