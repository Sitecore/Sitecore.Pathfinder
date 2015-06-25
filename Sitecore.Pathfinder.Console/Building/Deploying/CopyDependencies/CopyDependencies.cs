// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Deploying.CopyDependencies
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

            var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.Configuration.SolutionDirectory), ".packages");
            if (!context.FileSystem.DirectoryExists(sourceDirectory))
            {
                context.Trace.TraceInformation("'.packages' directory not found. Skipping");
                return;
            }

            var destinationDirectory = context.Configuration.Get(Constants.Configuration.Wwwroot);
            destinationDirectory = PathHelper.Combine(destinationDirectory, context.Configuration.Get(Constants.Configuration.DataDirectoryName));
            destinationDirectory = PathHelper.Combine(destinationDirectory, Constants.Configuration.Pathfinder);
            destinationDirectory = PathHelper.Combine(destinationDirectory, context.Configuration.Get(Constants.Configuration.PackageDirectory));

            context.FileSystem.CreateDirectory(destinationDirectory);

            foreach (var sourceFileName in context.FileSystem.GetFiles(sourceDirectory, "*nupkg", SearchOption.AllDirectories))
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
