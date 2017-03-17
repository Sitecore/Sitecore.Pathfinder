// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class ListCheckers : BuildTaskBase
    {
        [ImportingConstructor]
        public ListCheckers([NotNull] ICheckerService checkerService) : base("list-checkers")
        {
            CheckerService = checkerService;
        }

        [NotNull]
        protected ICheckerService CheckerService { get; }

        public override void Run(IBuildContext context)
        {
            foreach (var checker in CheckerService.Checkers.OrderBy(c => c.Name))
            {
                var name = checker.Name + " [" + checker.Category + "]";
                context.Trace.WriteLine(name);
            }
        }
    }
}
