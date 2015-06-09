// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Querying;

namespace Sitecore.Pathfinder.Building.Querying
{
    [Export(typeof(ITask))]
    public class FindUsages : QueryTaskBase
    {
        public FindUsages() : base("usages")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.DisplayDoneMessage = false;

            var qualifiedName = context.Configuration.GetString("name");
            if (string.IsNullOrEmpty(qualifiedName))
            {
                context.Trace.Writeline(Texts.You_must_specific_the___name_argument);
                return;
            }

            var queryService = context.CompositionService.Resolve<IQueryService>();

            var references = queryService.FindUsages(context.Project, qualifiedName).ToList();

            Display(context, references);

            context.Trace.Writeline(Texts.Found__ + references.Count());
        }
    }
}
