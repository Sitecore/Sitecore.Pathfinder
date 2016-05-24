// � 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Querying;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class FindUsages : QueryBuildTaskBase
    {
        [ImportingConstructor]
        public FindUsages([NotNull] IQueryService queryService) : base("find-usages")
        {
            QueryService = queryService;
        }

        [NotNull]
        protected IQueryService QueryService { get; }

        public override void Run(IBuildContext context)
        {
            var qualifiedName = context.Configuration.GetCommandLineArg(0);
            if (string.IsNullOrEmpty(qualifiedName))
            {
                context.Trace.WriteLine(Texts.You_must_specific_the___name_argument);
                return;
            }

            var references = QueryService.FindUsages(context.Project, qualifiedName).ToList();

            Display(context, references);

            context.Trace.WriteLine(Texts.Found__ + references.Count);
        }
    }
}
