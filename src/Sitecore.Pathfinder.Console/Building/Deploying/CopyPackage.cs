// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Deploying
{
    public class CopyPackage : TaskBase
    {
        public CopyPackage() : base("copy-package")
        {
        }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                context.Trace.TraceInformation(Texts.Package_contains_errors_and_will_not_be_deployed);
                context.IsAborted = true;
                return;
            }

            context.Trace.TraceInformation(Texts.Copying_package_to_website___);

            var destinationDirectory = context.Configuration.Get(Constants.Configuration.Wwwroot);
            destinationDirectory = PathHelper.Combine(destinationDirectory, context.Configuration.Get(Constants.Configuration.DataDirectoryName));
            destinationDirectory = PathHelper.Combine(destinationDirectory, Constants.Configuration.Pathfinder);
            destinationDirectory = PathHelper.Combine(destinationDirectory, context.Configuration.Get(Constants.Configuration.PackageDirectory));

            context.FileSystem.CreateDirectory(destinationDirectory);

            foreach (var fileName in context.OutputFiles)
            {
                var destinationFileName = PathHelper.Combine(destinationDirectory, Path.GetFileName(fileName));

                context.FileSystem.Copy(fileName, destinationFileName);
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Copies the project output to the website.");
        }
    }
}
