// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class ListProjectRoles : BuildTaskBase
    {
        public ListProjectRoles() : base("list-project-roles")
        {
        }

        public override void Run(IBuildContext context)
        {
            foreach (var pair in context.Configuration.GetSubKeys(Constants.Configuration.ProjectRoleCheckers).OrderBy(k => k.Key))
            {
                context.Trace.WriteLine(pair.Key);
            }
        }
    }
}
