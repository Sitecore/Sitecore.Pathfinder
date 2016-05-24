// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class SynchronizeWebsite : WebBuildTaskBase
    {
        [ImportingConstructor]
        public SynchronizeWebsite([NotNull] IFileSystemService fileSystem) : base("sync-website")
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.S1000, Texts.SynchronizingWebsite);

            if (!IsProjectConfigured(context))
            {
                return;
            }

            var webRequest = GetWebRequest(context).AsTask("SynchronizeWebsite");

            var targetFileName = Path.GetTempFileName();

            if (!DownloadFile(context, webRequest, targetFileName))
            {
                return;
            }

            context.Trace.TraceInformation(Msg.S1001, Texts.Updating_resources___);

            using (var zip = ZipFile.OpenRead(targetFileName))
            {
                foreach (var entry in zip.Entries)
                {
                    context.Trace.TraceInformation(Msg.S1002, entry.FullName);

                    var destinationFileName = Path.Combine(context.Project.ProjectDirectory, entry.FullName);

                    FileSystem.CreateDirectoryFromFileName(destinationFileName);

                    entry.ExtractToFile(destinationFileName, true);
                }
            }

            FileSystem.DeleteFile(targetFileName);
        }
    }
}
