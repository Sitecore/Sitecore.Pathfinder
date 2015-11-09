// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Initializing
{
    public class WriteWebsiteExports : RequestTaskBase
    {
        public WriteWebsiteExports() : base("write-website-exports")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation("Writing website exports...");

            var queryStringParameters = new Dictionary<string, string>
            {
                ["td"] = context.Configuration.GetString(Constants.Configuration.ToolsDirectory),
                ["pd"] = context.ProjectDirectory
            };

            var url = MakeWebApiUrl(context, "WriteWebsiteExports", queryStringParameters);
            var targetFileName = Path.GetTempFileName();

            if (!DownloadFile(context, url, targetFileName))
            {
                return;
            }

            using (var zip = ZipFile.OpenRead(targetFileName))
            {
                foreach (var entry in zip.Entries)
                {
                    context.Trace.TraceInformation(entry.FullName);

                    var fileName = Path.Combine(context.ProjectDirectory, entry.FullName);
                    context.FileSystem.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);

                    entry.ExtractToFile(fileName, true);
                }
            }

            context.FileSystem.DeleteFile(targetFileName);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Write website exports.");
        }
    }
}
