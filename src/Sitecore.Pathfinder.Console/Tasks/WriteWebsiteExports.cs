// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class WriteWebsiteExports : WebBuildTaskBase
    {
        [ImportingConstructor]
        public WriteWebsiteExports([NotNull] IFileSystemService fileSystem) : base("write-website-exports")
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.M1012, Texts.Writing_website_exports___);

            if (!IsProjectConfigured(context))
            {
                return;
            }

            var webRequest = GetWebRequest(context).AsTask("WriteWebsiteExports");

            var targetFileName = Path.GetTempFileName();

            if (!DownloadFile(context, webRequest, targetFileName))
            {
                return;
            }

            using (var zip = ZipFile.OpenRead(targetFileName))
            {
                foreach (var entry in zip.Entries)
                {
                    context.Trace.TraceInformation(Msg.M1013, entry.FullName);

                    var fileName = Path.Combine(context.ProjectDirectory, entry.FullName);
                    FileSystem.CreateDirectory(Path.GetDirectoryName(fileName));

                    entry.ExtractToFile(fileName, true);
                }
            }

            FileSystem.DeleteFile(targetFileName);
        }
    }
}
