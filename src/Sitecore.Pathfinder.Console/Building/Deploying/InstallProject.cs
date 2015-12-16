// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;

namespace Sitecore.Pathfinder.Building.Deploying
{
    public class InstallProject : RequestBuildTaskBase
    {
        public InstallProject() : base("install-project")
        {
        }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                context.Trace.TraceInformation(Msg.D1010, Texts.Package_contains_errors_and_will_not_be_deployed);
                context.IsAborted = true;
                return;
            }

            context.Trace.TraceInformation(Msg.D1011, Texts.Installing_project___);

            var url = MakeWebApiUrl(context, "InstallProject");

            var success = Request(context, url);

            if (success)
            {
                foreach (var snapshot in context.Project.ProjectItems.SelectMany(i => i.Snapshots))
                {
                    snapshot.SourceFile.IsModified = false;
                }
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Installs the project directly from the project directory.");
        }
    }
}
