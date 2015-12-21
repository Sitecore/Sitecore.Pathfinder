// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking
{
    [Export(typeof(ICheckerService))]
    public class CheckerService : ICheckerService
    {
        [ImportingConstructor]
        public CheckerService([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull, ImportMany, ItemNotNull] IEnumerable<IChecker> checkers)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Checkers = checkers;
        }

        public IEnumerable<IChecker> Checkers { get; }

        public int LastCheckerCount { get; protected set; }

        public int LastConventionCount { get; protected set; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public virtual void CheckProject(IProject project)
        {
            var context = CompositionService.Resolve<ICheckerContext>().With(project);

            var disabledCategories = Configuration.GetCommaSeparatedStringList(Constants.Configuration.CheckProjectDisabledCategories);
            var disabledCheckers = Configuration.GetCommaSeparatedStringList(Constants.Configuration.CheckProjectDisabledCheckers);

            foreach (var checker in Checkers)
            {
                if (disabledCheckers.Any(disabledChecker => string.Equals(checker.Name, disabledChecker, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var categories = checker.Categories.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
                if (categories.Any(c => disabledCategories.Contains(c)))
                {
                    continue;
                }

                checker.Check(context);

                context.CheckCount++;
            }

            LastCheckerCount = context.CheckCount;
            LastConventionCount = context.ConventionCount;
        }
    }
}
