// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Deploying
{
    [Export(typeof(ITask))]
    public class InstallPackage : RequestTaskBase
    {
        public InstallPackage() : base("install-package")
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

            context.Trace.TraceInformation(Texts.Installing___);

            var packageId = Path.GetFileNameWithoutExtension(context.Configuration.Get(Constants.Configuration.PackNugetFileName));
            if (string.IsNullOrEmpty(packageId))
            {
                return;
            }

            var queryStringParameters = new Dictionary<string, string>
            {
                ["w"] = "0",
                ["rep"] = packageId
            };

            var url = MakeUrl(context, context.Configuration.GetString(Constants.Configuration.InstallUrl), queryStringParameters);
            if (!Request(context, url))
            {
                return;
            }

            foreach (var snapshot in context.Project.Items.SelectMany(i => i.Snapshots))
            {
                snapshot.SourceFile.IsModified = false;
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Unpacks and installs the project package (including dependencies) in the website.");
        }
    }
}
