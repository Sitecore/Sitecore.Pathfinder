// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ListOutput : BuildTaskBase
    {
        [ImportingConstructor]
        public ListOutput([NotNull] IRuleService ruleService) : base("list-output")
        {
            RuleService = ruleService;
        }

        [NotNull]
        protected IRuleService RuleService { get; }

        public override void Run(IBuildContext context)
        {
            foreach (var item in context.Project.Items.Where(i => i.IsEmittable).OrderBy(i => i.ItemIdOrPath))
            {
                Console.WriteLine(item.ItemIdOrPath);
            }

            foreach (var item in context.Project.Files.OrderBy(i => i.FilePath))
            {
                Console.WriteLine(item.FilePath);
            }
        }
    }
}
