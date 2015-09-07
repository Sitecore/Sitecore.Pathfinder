// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Querying;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Building.Querying
{
    [Export(typeof(ITask))]
    public class FindReferences : QueryTaskBase
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
                context.Trace.Writeline(Texts.You_must_specific_the___name_argument);
                return;
            }

            var queryService = context.CompositionService.Resolve<IQueryService>();

            var projectItem = queryService.FindProjectItem(context.Project, qualifiedName);
            if (projectItem == null)
            {
                context.Trace.Writeline(Texts.Project_item_not_found__ + qualifiedName);
                return;
            }

            foreach (var reference in projectItem.References)
            {
                string line = $"{reference.Owner.Snapshots.First().SourceFile.GetProjectPath(context.Project)}";

                var textNode = TraceHelper.GetTextNode(reference.SourceProperty);
                line += $"({textNode.Position.LineNumber},{textNode.Position.LineNumber})";

                line += ": " + reference.TargetQualifiedName;

                if (!reference.IsValid)
                {
                    line += " (not valid)";
                }

                context.Trace.Writeline(line);
            }

            context.Trace.Writeline(Texts.Found__ + projectItem.References.Count());
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Finds all project items that the specified project item references.");
            helpWriter.Remarks.Write("The project item must be fully qualified.");
            helpWriter.Examples.Write("scc find-references /sitecore/content/Home");
        }
    }
}
