// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class InstallProject : WebBuildTaskBase
    {
        public InstallProject() : base("install-project")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1011, Texts.Installing_project___);

            if (!IsProjectConfigured(context))
            {
                return;
            }

            var webRequest = GetWebRequest(context).AsTask("InstallProject");

            var success = Post(context, webRequest);
            if (!success)
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
