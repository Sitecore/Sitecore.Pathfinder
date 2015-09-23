// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    [Export(typeof(ICheckerService))]
    public class CheckerService : ICheckerService
    {
        [ImportingConstructor]
        public CheckerService([NotNull] ICompositionService compositionService)
        {
            CompositionService = compositionService;
        }

        [NotNull]
        [ImportMany]
        [ItemNotNull]
        protected IEnumerable<IChecker> Checkers { get; private set; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        public void CheckProject(IProject project)
        {
            var context = CompositionService.Resolve<ICheckerContext>().With(project);

            foreach (var checker in Checkers)
            {
                checker.Check(context);
            }
        }
    }
}
