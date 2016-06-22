// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class InstallPackage : WebBuildTaskBase
    {
        public InstallPackage() : base("install-package")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1008, Texts.Installing___);

            if (!IsProjectConfigured(context))
            {
                return;
            }

            var failed = false;

            foreach (var fileName in context.OutputFiles)
            {
                var packageId = Path.GetFileNameWithoutExtension(fileName);
                if (string.IsNullOrEmpty(packageId))
                {
                    continue;
                }

                var feeds = string.Empty;
                if (context.Configuration.GetBool(Constants.Configuration.InstallPackage.AddProjectDirectoriesAsFeeds, true))
                {
                    var packagesDirectory = Path.Combine(context.ProjectDirectory, context.Configuration.GetString(Constants.Configuration.Packages.NugetDirectory));
                    feeds = Path.GetDirectoryName(fileName) + "," + packagesDirectory;
                }

                var queryStringParameters = new Dictionary<string, string>
                {
                    ["w"] = "0",
                    ["rep"] = packageId,
                    ["feeds"] = feeds
                };

                var webRequest = GetWebRequest(context).WithQueryString(queryStringParameters).WithUrl(context.Configuration.GetString(Constants.Configuration.InstallPackage.InstallUrl));
                if (Post(context, webRequest))
                {
                    context.Trace.TraceInformation(Msg.D1009, Texts.Installed, Path.GetFileName(fileName));
                }
                else
                {
                    failed = true;
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
    }
}
