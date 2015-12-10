// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules;

namespace Sitecore.Pathfinder.Building.Commands
{
    public class ListRules : BuildTaskBase
    {
        [ImportingConstructor]
        public ListRules([NotNull] IRuleService ruleService) : base("list-rules")
        {
            RuleService = ruleService;
        }

        [NotNull]
        protected IRuleService RuleService { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.WriteLine("Conditions:");
            foreach (var condition in RuleService.Conditions.OrderBy(c => c.Name))
            {
                context.Trace.WriteLine(condition.Name);
            }

            context.Trace.WriteLine("");
            context.Trace.WriteLine("Actions:");
            foreach (var action in RuleService.Actions.OrderBy(c => c.Name))
            {
                context.Trace.WriteLine(action.Name);
            }

            context.DisplayDoneMessage = false;
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Lists the available conditions and actions.");
        }
    }
}
