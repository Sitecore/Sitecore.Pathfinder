// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ListProjectRoles : BuildTaskBase
    {
        public ListProjectRoles() : base("list-project-roles")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;
            context.DisplayDoneMessage = false;

            foreach (var pair in context.Configuration.GetSubKeys(Constants.Configuration.ProjectRoleCheckers).OrderBy(k => k.Key))
            {
                context.Trace.WriteLine(pair.Key);
            }
        }
    }
}
