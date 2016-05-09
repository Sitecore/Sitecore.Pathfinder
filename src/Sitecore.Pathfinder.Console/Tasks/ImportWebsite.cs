// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.IO.Compression;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ImportWebsite : WebBuildTaskBase
    {
        public ImportWebsite() : base("import-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.G1012, Texts.Importing_website___);

            context.IsAborted = true;

            var webRequest = GetWebRequest(context).AsTask("ImportWebsite");

            var zipFileName = Path.GetTempFileName();

            if (!DownloadFile(context, webRequest, zipFileName))
            {
                return;
            }

            context.Trace.TraceInformation(Msg.G1015, Texts.Writing_files_and_items___);

            int fileCount;

            using (var zip = ZipFile.OpenRead(zipFileName))
            {
                fileCount = zip.Entries.Count;

                foreach (var entry in zip.Entries)
                {
                    var destinationFileName = Path.Combine(context.ProjectDirectory, entry.FullName);

                    context.FileSystem.CreateDirectoryFromFileName(destinationFileName);

                    entry.ExtractToFile(destinationFileName, true);
                }
            }

            context.FileSystem.DeleteFile(zipFileName);

            context.Trace.TraceInformation(Msg.G1015, Texts.Files_imported, fileCount.ToString());
        }
    }
}
