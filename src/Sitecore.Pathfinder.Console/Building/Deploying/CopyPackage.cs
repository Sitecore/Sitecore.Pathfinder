// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Deploying
{
    public class CopyPackage : BuildTaskBase
    {
        public CopyPackage() : base("copy-package")
        {
        }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                context.Trace.TraceInformation(Msg.D1004, Texts.Package_contains_errors_and_will_not_be_deployed);
                context.IsAborted = true;
                return;
            }

            context.Trace.TraceInformation(Msg.D1005, Texts.Copying_package_to_website___);

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
                destinationDirectory = PathHelper.Combine(context.Configuration.Get(Constants.Configuration.DataFolderDirectory), destinationDirectory);

                context.FileSystem.CreateDirectory(destinationDirectory);

                foreach (var fileName in context.OutputFiles)
                {
                    var destinationFileName = PathHelper.Combine(destinationDirectory, Path.GetFileName(fileName));
                    context.FileSystem.Copy(fileName, destinationFileName);
                }
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Copies the project output to the website.");
            helpWriter.Remarks.Write("Settings:");
            helpWriter.Remarks.Write("    copy-package:package-directory");
            helpWriter.Remarks.Write("        The directory under the website Data Folder to copy to.");
        }
    }
}
