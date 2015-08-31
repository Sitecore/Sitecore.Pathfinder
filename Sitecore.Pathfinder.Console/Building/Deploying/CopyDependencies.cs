// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Deploying
{
    [Export(typeof(ITask))]
    public class CopyDependencies : TaskBase
    {
        public CopyDependencies() : base("copy-dependencies")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Copying_dependencies___);

            var dependenciesDirectory = context.Configuration.Get(Constants.Configuration.DependenciesDirectory);
            var sourceDirectory = Path.Combine(context.SolutionDirectory, dependenciesDirectory);
            if (!context.FileSystem.DirectoryExists(sourceDirectory))
            {
                context.Trace.TraceInformation(Texts.Dependencies_directory_not_found__Skipping, dependenciesDirectory);
                return;
            }

            var destinationDirectory = context.Configuration.Get(Constants.Configuration.Wwwroot);
            destinationDirectory = PathHelper.Combine(destinationDirectory, context.Configuration.Get(Constants.Configuration.DataDirectoryName));
            destinationDirectory = PathHelper.Combine(destinationDirectory, Constants.Configuration.Pathfinder);
            destinationDirectory = PathHelper.Combine(destinationDirectory, context.Configuration.Get(Constants.Configuration.PackageDirectory));

            context.FileSystem.CreateDirectory(destinationDirectory);

            CopySitecorePackages(context, sourceDirectory, destinationDirectory);
            CopyNuGetPackages(context, sourceDirectory, destinationDirectory);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Copies the dependencies to the website.");
            helpWriter.Remarks.Write("The dependencies can be Nuget packages and Sitecore packages. The packages are located in the sitecore.project/dependencies directory.");
        }

        private void CopyNuGetPackages([NotNull] IBuildContext context, [NotNull] string sourceDirectory, [NotNull] string destinationDirectory)
        {
            foreach (var sourceFileName in context.FileSystem.GetFiles(sourceDirectory, "*.nupkg", SearchOption.AllDirectories))
            {
                var destinationFileName = Path.Combine(destinationDirectory, Path.GetFileName(sourceFileName) ?? string.Empty);
                if (!context.FileSystem.FileExists(destinationFileName) || context.FileSystem.GetLastWriteTimeUtc(sourceFileName) > context.FileSystem.GetLastWriteTimeUtc(destinationFileName))
                {
                    context.Trace.TraceInformation(Texts.Copying_dependency, Path.GetFileName(sourceFileName) ?? string.Empty);
                    context.FileSystem.Copy(sourceFileName, destinationFileName);
                }
            }
        }

        private void CopySitecorePackages([NotNull] IBuildContext context, [NotNull] string sourceDirectory, [NotNull] string destinationDirectory)
        {
            foreach (var sourceFileName in context.FileSystem.GetFiles(sourceDirectory, "*.zip", SearchOption.AllDirectories))
            {
                var destinationFileName = Path.Combine(destinationDirectory, Path.GetFileName(sourceFileName) ?? string.Empty);
                if (!context.FileSystem.FileExists(destinationFileName) || context.FileSystem.GetLastWriteTimeUtc(sourceFileName) > context.FileSystem.GetLastWriteTimeUtc(destinationFileName))
                {
                    context.Trace.TraceInformation(Texts.Copying_dependency, Path.GetFileName(sourceFileName) ?? string.Empty);
                    context.FileSystem.Copy(sourceFileName, destinationFileName);
                }
            }
        }
    }
}
