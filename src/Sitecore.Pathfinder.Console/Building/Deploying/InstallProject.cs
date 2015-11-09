// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Deploying
{
    public class InstallProject : RequestTaskBase
    {
        public InstallProject() : base("install-project")
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

            context.Trace.TraceInformation(Texts.Installing_project___);

            var queryStringParameters = new Dictionary<string, string>
            {
                ["td"] = context.Configuration.GetString(Constants.Configuration.ToolsDirectory),
                ["pd"] = context.ProjectDirectory
            };

            var url = MakeWebApiUrl(context, "InstallProject", queryStringParameters);

            var success = Request(context, url);

            if (success)
            {
                foreach (var snapshot in context.Project.Items.SelectMany(i => i.Snapshots))
                {
                    snapshot.SourceFile.IsModified = false;
                }
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Installs the project directly from the project directory.");
            helpWriter.Remarks.Write("Settings:");
            helpWriter.Remarks.Write("    None.");
        }
    }
}
