// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.IO.Compression;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class SynchronizeWebsite : WebBuildTaskBase
    {
        public SynchronizeWebsite() : base("sync-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.S1000, Texts.SynchronizingWebsite);

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

                    var destinationFileName = Path.Combine(context.ProjectDirectory, entry.FullName);

                    context.FileSystem.CreateDirectoryFromFileName(destinationFileName);

                    entry.ExtractToFile(destinationFileName, true);
                }
            }

            context.FileSystem.DeleteFile(targetFileName);
        }
    }
}
