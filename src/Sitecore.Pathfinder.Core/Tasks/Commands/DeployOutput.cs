// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks.Commands
{
    [Export(typeof(ITask)), Shared]
    public class DeployOutput : BuildTaskBase
    {
        [ImportingConstructor]
        public DeployOutput([NotNull] IFileSystem fileSystem) : base("deploy-output", "deploy")
        {
            FileSystem = fileSystem;

            Shortcut = "d";
        }

        [NotNull]
        protected IFileSystem FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            if (!context.OutputFiles.Any())
            {
                return;
            }

            if (!context.Configuration.GetBool(Constants.Configuration.Output.CopyToDataFolderDirectory, true))
            {
                return;
            }

            if (string.IsNullOrEmpty(context.DataFolderDirectory))
            {
                return;
            }

            context.Trace.TraceInformation(Msg.D1030, "Deploying output...");

            var directory = Path.Combine(context.DataFolderDirectory, "pathfinder");

            foreach (var outputFile in context.OutputFiles)
            {
                var destination = Path.Combine(directory, Path.GetFileName(outputFile.FileName));

                try
                {
                    if (FileSystem.CopyIfNewer(outputFile.FileName, destination))
                    {
                        context.Trace.TraceInformation(Msg.D1031, "Deploying", outputFile.FileName + " => " + destination);
                    }
                }
                catch
                {
                    context.Trace.TraceError(Msg.E1000, "Failed to deploy", outputFile.FileName + " => " + destination);
                }
            }
        }
    }
}
