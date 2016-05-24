// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
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
            foreach (var checker in CheckerService.Checkers.OrderBy(c => c.Method.Name))
            {
                var name = checker.Method.Name;

                var type = checker.Method.DeclaringType;
                if (type != null)
                {
                    var category = type.Name;

                    if (category.EndsWith("Checker"))
                    {
                        category = category.Left(category.Length - 7);
                    }

                    if (category.EndsWith("Conventions"))
                    {
                        category = category.Left(category.Length - 11);
                    }

                    name += " [" + category + "]";
                }

                context.Trace.WriteLine(name);
            }
        }
    }
}
