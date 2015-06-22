// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Initializing.SynchronizeWebsites
{
    [Export(typeof(ITask))]
    public class SynchronizeWebsite : RequestTaskBase
    {
        public SynchronizeWebsite() : base("sync-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.SynchronizingWebsite);

            var url = MakeUrl(context, context.Configuration.GetString(Constants.Configuration.UpdateResourcesUrl), new Dictionary<string, string>());
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
                    entry.ExtractToFile(Path.Combine(context.SolutionDirectory, entry.FullName), true);
                }
            }

            context.FileSystem.DeleteFile(targetFileName);
        }
    }
}
