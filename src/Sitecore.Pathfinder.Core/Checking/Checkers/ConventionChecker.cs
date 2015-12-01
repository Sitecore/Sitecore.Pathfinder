// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Rules;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class ConventionChecker : CheckerBase
    {
        [ImportingConstructor]
        public ConventionChecker([NotNull] IRuleService ruleService) : base("Conventions", All)
        {
            RuleService = ruleService;
        }

        [NotNull]
        protected IRuleService RuleService { get; }

        public override void Check(ICheckerContext context)
        {
            var rules = RuleService.ParseRules("check-project:conventions").ToArray();

            var items = context.Project.ProjectItems.Where(i => !(i is ItemBase) || !((ItemBase)i).IsImport).ToArray();

            foreach (var projectItem in items)
            {
                var ruleContext = new ConventionRuleContext(projectItem);

                foreach (var rule in rules)
                {
                    rule.Execute(ruleContext);
                }
            }
        }
    }
}
