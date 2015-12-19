// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Querying;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Building.Tasks
{
    public class FindReferences : QueryBuildTaskBase
    {
        public FindReferences() : base("find-references")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.DisplayDoneMessage = false;

            var qualifiedName = context.Configuration.GetCommandLineArg(0);
            if (string.IsNullOrEmpty(qualifiedName))
            {
                context.Trace.WriteLine(Texts.You_must_specific_the___name_argument);
                return;
            }

            var queryService = context.CompositionService.Resolve<IQueryService>();

            var projectItem = queryService.FindProjectItem(context.Project, qualifiedName);
            if (projectItem == null)
            {
                context.Trace.WriteLine(Texts.Project_item_not_found__ + qualifiedName);
                return;
            }

            foreach (var reference in projectItem.References)
            {
                string line = $"{reference.Owner.Snapshots.First().SourceFile.ProjectFileName}";

                var textNode = TraceHelper.GetTextNode(reference.SourceProperty);
                line += $"({textNode.TextSpan.LineNumber},{textNode.TextSpan.LineNumber})";

                line += ": " + reference.SourceProperty.GetValue();

                if (!reference.IsValid)
                {
                    line += " (not valid)";
                }

                context.Trace.WriteLine(line);
            }

            context.Trace.WriteLine(Texts.Found__ + projectItem.References.Count);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Finds all project items that the specified project item references.");
            helpWriter.Remarks.Write("The project item must be fully qualified.");
            helpWriter.Examples.Write("scc find-references /sitecore/content/Home");
        }
    }
}
