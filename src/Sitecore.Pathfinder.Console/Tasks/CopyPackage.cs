// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class CopyPackage : BuildTaskBase
    {
        [ImportingConstructor]
        public CopyPackage([NotNull] IFileSystemService fileSystem) : base("copy-package")
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1005, Texts.Copying_package_to_website___);

            if (!IsProjectConfigured(context))
            {
                return;
            }

            foreach (var pair in context.Configuration.GetSubKeys("copy-package"))
            {
                var key = "copy-package:" + pair.Key;

                var destinationDirectory = context.Configuration.GetString(key + ":copy-to-directory");
                if (string.IsNullOrEmpty(destinationDirectory))
                {
                    context.Trace.TraceError(Msg.D1006, Texts.Destination_directory_not_found, key + ":copy-to-directory");
                    continue;
                }

                destinationDirectory = PathHelper.NormalizeFilePath(destinationDirectory).TrimStart('\\');
                destinationDirectory = PathHelper.Combine(context.Configuration.GetString(Constants.Configuration.DataFolderDirectory), destinationDirectory);

                FileSystem.CreateDirectory(destinationDirectory);

                foreach (var fileName in context.OutputFiles)
                {
                    var destinationFileName = PathHelper.Combine(destinationDirectory, Path.GetFileName(fileName));
                    FileSystem.Copy(fileName, destinationFileName);
                }
            }
        }
    }
}
