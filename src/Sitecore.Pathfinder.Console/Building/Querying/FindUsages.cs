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
        public FindUsages() : base("find-usages")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.DisplayDoneMessage = false;

            var qualifiedName = context.Configuration.GetCommandLineArg(0);
            if (string.IsNullOrEmpty(qualifiedName))
            {
                context.Trace.Writeline(Texts.You_must_specific_the___name_argument);
                return;
            }

            var queryService = context.CompositionService.Resolve<IQueryService>();

            var references = queryService.FindUsages(context.Project, qualifiedName).ToList();

            Display(context, references);

            context.Trace.Writeline(Texts.Found__ + references.Count);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Finds all project items that references the specified project item.");
            helpWriter.Remarks.Write("The project item must be fully qualified.");
            helpWriter.Examples.Write("scc find-usages /sitecore/content/Home");
        }
    }
}
