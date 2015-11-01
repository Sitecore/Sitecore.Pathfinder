// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Initializing
{
    public class SynchronizeWebsite : RequestTaskBase
    {
        public SynchronizeWebsite() : base("sync-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.SynchronizingWebsite);

            var queryStringParameters = new Dictionary<string, string>();

            queryStringParameters["t"] = context.Configuration.Get(Constants.Configuration.ToolsDirectory);

            var url = MakeUrl(context, context.Configuration.GetString(Constants.Configuration.UpdateResourcesUrl), queryStringParameters);
            var targetFileName = Path.GetTempFileName();

            if (!DownloadFile(context, url, targetFileName))
            {
                return;
            }

            context.Trace.TraceInformation(Texts.Updating_resources___);

            using (var zip = ZipFile.OpenRead(targetFileName))
            {
                foreach (var entry in zip.Entries)
                {
                    context.Trace.TraceInformation(entry.FullName);
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
