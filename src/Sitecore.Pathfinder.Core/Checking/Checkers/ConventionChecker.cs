// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Checking.Conventions;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Rules;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class ConventionChecker : CheckerBase
    {
        [ImportingConstructor]
        public ConventionChecker([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] IRuleService ruleService) : base("Conventions", All)
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

        public override void Check(ICheckerContext context)
        {
            CheckConventions(context);

            // todo: consider deprecating Json based rules
            CheckRules(context);
        }

        protected virtual void CheckConventions([NotNull] ICheckerContext context)
        {
            var conventionNames = new HashSet<string>();

            var projectRoles = Configuration.GetCommaSeparatedStringList(Constants.Configuration.ProjectRole);
            foreach (var projectRole in projectRoles)
            {
                foreach (var pair in Configuration.GetSubKeys("project-role-conventions:" + projectRole))
                {
                    var conventionName = Configuration.GetString("project-role-conventions:" + projectRole + ":" + pair.Key);
                    if (string.IsNullOrEmpty(conventionName))
                    {
                        continue;
                    }

                    // ignore json rule files
                    if (conventionName.IndexOf(".json", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        conventionNames.Add(conventionName);
                    }
                }
            }

            var conventions = CompositionService.ResolveMany<ConventionsBase>();

            foreach (var convention in conventions)
            {
                if (!conventionNames.Contains(convention.GetType().Name))
                {
                    continue;
                }

                context.ConventionCount += convention.ConventionCount;

                convention.Check(context);
            }
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

        protected virtual void CheckRules([NotNull] ICheckerContext context)
        {
            var rules = RuleService.ParseRules("check-project:conventions").ToArray();

            context.ConventionCount += rules.Length;

            CheckProject(context, rules);
            CheckProjectItems(context, rules);
        }
    }
}
