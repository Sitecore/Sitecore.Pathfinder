// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class ListProjectCheckers : BuildTaskBase
    {
        [ImportingConstructor]
        public ListProjectCheckers([NotNull] ICheckerService checkerService) : base("list-project-checkers")
        {
            CheckerService = checkerService;
        }

        [NotNull]
        protected ICheckerService CheckerService { get; }

        public override void Run(IBuildContext context)
        {
            foreach (var checker in CheckerService.GetEnabledCheckers().OrderBy(c => c.Method.Name))
            {
                var name = checker.Method.Name;

                var type = checker.Method.DeclaringType;
                if (type != null)
                {
                    var category = type.Name;
                    name += " [" + category + "]";
                }

                context.Trace.WriteLine(name);
            }
        }
    }
}
