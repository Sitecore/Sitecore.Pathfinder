// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Deploying
{
    public class CopyDependencies : TaskBase
    {
        public CopyDependencies() : base("copy-dependencies")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Copying_dependencies___);

            var packagesDirectory = context.Configuration.Get(Constants.Configuration.PackagesDirectory);
            var sourceDirectory = Path.Combine(context.ProjectDirectory, packagesDirectory);
            if (!context.FileSystem.DirectoryExists(sourceDirectory))
            {
                context.Trace.TraceInformation(Texts.Dependencies_directory_not_found__Skipping, packagesDirectory);
                return;
            }

            var destinationDirectory = context.Configuration.Get(Constants.Configuration.DataFolderDirectory);
            destinationDirectory = PathHelper.Combine(destinationDirectory, Constants.Configuration.Pathfinder);
            destinationDirectory = PathHelper.Combine(destinationDirectory, context.Configuration.Get(Constants.Configuration.PackageDirectory));

            context.FileSystem.CreateDirectory(destinationDirectory);

            CopyNuGetPackages(context, sourceDirectory, destinationDirectory);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Copies the dependency packages to the website.");
            helpWriter.Remarks.Write("The packages dependencies are Nuget packages. The packages are located in the sitecore.project/packages directory. To wrap a Sitecore package (.zip) in a Nuget package use the 'pack-dependencies' task.");
        }

        private void CopyNuGetPackages([NotNull] IBuildContext context, [NotNull] string sourceDirectory, [NotNull] string destinationDirectory)
        {
            foreach (var sourceFileName in context.FileSystem.GetFiles(sourceDirectory, "*.nupkg", SearchOption.AllDirectories))
            {
                var destinationFileName = Path.Combine(destinationDirectory, Path.GetFileName(sourceFileName));
                if (!context.FileSystem.FileExists(destinationFileName) || context.FileSystem.GetLastWriteTimeUtc(sourceFileName) > context.FileSystem.GetLastWriteTimeUtc(destinationFileName))
                {
                    context.Trace.TraceInformation(Texts.Copying_dependency, Path.GetFileName(sourceFileName));
                    context.FileSystem.Copy(sourceFileName, destinationFileName);
                }
            }
        }
    }
}
