// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.IO.Compression;

namespace Sitecore.Pathfinder.Building.Maintaining
{
    public class SynchronizeWebsite : RequestBuildTaskBase
    {
        public SynchronizeWebsite() : base("sync-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.S1000, Texts.SynchronizingWebsite);

            var url = MakeWebApiUrl(context, "SynchronizeWebsite");
            var targetFileName = Path.GetTempFileName();

            if (!DownloadFile(context, url, targetFileName))
            {
                return;
            }

            context.Trace.TraceInformation(Msg.S1001, Texts.Updating_resources___);

            using (var zip = ZipFile.OpenRead(targetFileName))
            {
                foreach (var entry in zip.Entries)
                {
                    context.Trace.TraceInformation(Msg.S1002, entry.FullName);
                    entry.ExtractToFile(Path.Combine(context.ProjectDirectory, entry.FullName), true);
                }
            }

            context.FileSystem.DeleteFile(targetFileName);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Synchronizes project and the website.");
            helpWriter.Remarks.WriteLine("Downloads Xml and Json schemas from the website to make item and layout IntelliSense work.");
        }
    }
}
