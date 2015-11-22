// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Querying;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Templates
{
    public class UnusedTemplateChecker : CheckerBase
    {
        public UnusedTemplateChecker() : base("Unused template", Templates)
        {
        }

        public override void Check(ICheckerContext context)
        {
            var queryService = context.CompositionService.Resolve<IQueryService>();

            foreach (var template in context.Project.Templates)
            {
                var references = queryService.FindUsages(context.Project, template.QualifiedName);
                if (!references.Any())
                {
                    context.Trace.TraceWarning("Unused template", TraceHelper.GetTextNode(template), $"The template \"{template.ItemName}\" is not used by any items and can be deleted.");
                }
            }
        }
    }
}
