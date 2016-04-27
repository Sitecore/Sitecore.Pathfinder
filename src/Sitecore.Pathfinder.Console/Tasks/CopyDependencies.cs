// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class CopyDependencies : BuildTaskBase
    {
        public CopyDependencies() : base("copy-dependencies")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1000, Texts.Copying_dependencies___);

            var sourceDirectory = context.Configuration.Get(Constants.Configuration.CopyDependenciesSourceDirectory);
            sourceDirectory = Path.Combine(context.ProjectDirectory, sourceDirectory);
            if (!context.FileSystem.DirectoryExists(sourceDirectory))
            {
                context.Trace.TraceInformation(Msg.D1003, Texts.Dependencies_directory_not_found__Skipping, sourceDirectory);
                return;
            }

            foreach (var pair in context.Configuration.GetSubKeys("copy-package"))
            {
                var key = "copy-package:" + pair.Key;

                var destinationDirectory = context.Configuration.GetString(key + ":copy-to-directory");
                if (string.IsNullOrEmpty(destinationDirectory))
                {
                    context.Trace.TraceError(Msg.D1001, Texts.Destination_directory_not_found, key + ":copy-to-directory");
                    continue;
                }

                destinationDirectory = PathHelper.NormalizeFilePath(destinationDirectory).TrimStart('\\');
                destinationDirectory = PathHelper.Combine(context.Configuration.Get(Constants.Configuration.DataFolderDirectory), destinationDirectory);

                context.FileSystem.CreateDirectory(destinationDirectory);

                CopyNuGetPackages(context, sourceDirectory, destinationDirectory);
            }
        }

        private void CopyNuGetPackages([NotNull] IBuildContext context, [NotNull] string sourceDirectory, [NotNull] string destinationDirectory)
        {
            foreach (var sourceFileName in context.FileSystem.GetFiles(sourceDirectory, "*.nupkg", SearchOption.AllDirectories))
            {
                var destinationFileName = Path.Combine(destinationDirectory, Path.GetFileName(sourceFileName));
                if (context.FileSystem.FileExists(destinationFileName) && context.FileSystem.GetLastWriteTimeUtc(sourceFileName) <= context.FileSystem.GetLastWriteTimeUtc(destinationFileName))
                {
                    continue;
                }

                context.Trace.TraceInformation(Msg.D1002, Texts.Copying_dependency, Path.GetFileName(sourceFileName));
                context.FileSystem.Copy(sourceFileName, destinationFileName);
            }
        }
    }
}
