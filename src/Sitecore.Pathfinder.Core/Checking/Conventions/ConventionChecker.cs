// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Checking.Checkers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Rules;

namespace Sitecore.Pathfinder.Checking.Conventions
{
    public class ConventionChecker : Checker
    {
        [ImportingConstructor]
        public ConventionChecker([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] IRuleService ruleService)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            RuleService = ruleService;
        }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IRuleService RuleService { get; }

        [NotNull, ItemNotNull, Export("Check")]
        public IEnumerable<Diagnostic> Conventions([NotNull] ICheckerContext context)
        {
            // todo: consider deprecating Json based rules
            CheckJsonRules(context);

            return Enumerable.Empty<Diagnostic>();
        }

        protected virtual void CheckJsonRules([NotNull] ICheckerContext context)
        {
            var rules = RuleService.ParseRules("check-project:conventions").ToArray();

            CheckProject(context, rules);
            CheckProjectItems(context, rules);
        }

        protected virtual void CheckProject([NotNull] ICheckerContext context, [NotNull, ItemNotNull] IRule[] rules)
        {
            var ruleContext = new ConventionRuleContext(context.Trace, context.Project);

            foreach (var rule in rules.Where(rule => string.Equals(rule.Filter, "project", StringComparison.OrdinalIgnoreCase)))
            {
                rule.Execute(ruleContext);

                if (ruleContext.IsAborted)
                {
                    break;
                }
            }
        }

        protected virtual void CheckProjectItems([NotNull] ICheckerContext context, [NotNull, ItemNotNull] IRule[] rules)
        {
            var items = context.Project.ProjectItems.Where(i => !(i is DatabaseProjectItem) || !((DatabaseProjectItem)i).IsImport).ToArray();
            foreach (var projectItem in items)
            {
                var ruleContext = new ConventionRuleContext(context.Trace, projectItem);

                foreach (var rule in rules)
                {
                    switch (rule.Filter.ToLowerInvariant())
                    {
                        case "items":
                            var item = projectItem as Item;
                            if (item == null)
                            {
                                continue;
                            }

                            break;
                        case "templates":
                            var template = projectItem as Template;
                            if (template == null)
                            {
                                continue;
                            }

                            break;

                        case "databaseprojectitems":
                            var databaseProjectItem = projectItem as DatabaseProjectItem;
                            if (databaseProjectItem == null)
                            {
                                continue;
                            }

                            break;

                        case "file":
                            var file = projectItem as File;
                            if (file == null)
                            {
                                continue;
                            }

                            break;

                        case "project":
                            continue;
                    }

                    rule.Execute(ruleContext);

                    if (ruleContext.IsAborted)
                    {
                        break;
                    }
                }
            }
        }
    }
}
