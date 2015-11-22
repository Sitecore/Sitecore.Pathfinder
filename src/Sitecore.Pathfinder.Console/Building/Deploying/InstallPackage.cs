// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Deploying
{
    public class InstallPackage : RequestBuildTaskBase
    {
        public InstallPackage() : base("install-package")
        {
        }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                context.Trace.TraceInformation(Msg.D1007, Texts.Package_contains_errors_and_will_not_be_deployed);
                context.IsAborted = true;
                return;
            }

            context.Trace.TraceInformation(Msg.D1008, Texts.Installing___);

            var failed = false;

            foreach (var fileName in context.OutputFiles)
            {
                var packageId = Path.GetFileNameWithoutExtension(fileName);
                if (string.IsNullOrEmpty(packageId))
                {
                    continue;
                }

                var queryStringParameters = new Dictionary<string, string>
                {
                    ["w"] = "0",
                    ["rep"] = packageId
                };


                var url = MakeUrl(context, context.Configuration.GetString(Constants.Configuration.InstallUrl), queryStringParameters);
                if (!Request(context, url))
                {
                    failed = true;
                }
                else
                {
                    context.Trace.TraceInformation(Msg.D1009, Texts.Installed, Path.GetFileName(fileName));
                }
            }

            if (failed)
            {
                return;
            }

            foreach (var snapshot in context.Project.ProjectItems.SelectMany(i => i.Snapshots))
            {
                snapshot.SourceFile.IsModified = false;
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Unpacks and installs the project package (including dependencies) in the website.");

            helpWriter.Remarks.Write("Settings:");
            helpWriter.Remarks.Write("    install-package:check-bin-file-version");
            helpWriter.Remarks.Write("        If true, check the versions of assemblies in /bin before copying.");
            helpWriter.Remarks.Write("    install-package:install-url");
            helpWriter.Remarks.Write("        The URL for installing a package.");
        }
    }
}
