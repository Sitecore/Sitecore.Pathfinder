// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Querying;

namespace Sitecore.Pathfinder.Building.Querying
{
    [Export(typeof(ITask))]
    public class FindReferences : QueryTaskBase
    {
        public FindReferences() : base("references")
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

            var projectItem = queryService.FindProjectItem(context.Project, qualifiedName);
            if (projectItem == null)
            {
                context.Trace.Writeline(Texts.Project_item_not_found__ + qualifiedName);
                return;
            }

            foreach (var reference in projectItem.References)
            {
                string line = $"{reference.Owner.Snapshot.SourceFile.GetProjectPath(context.Project)}";

                var textNode = reference.SourceTextNode;
                line += textNode != null ? $"({textNode.Position.LineNumber},{textNode.Position.LineNumber})" : "(0,0)";

                line += ": " + reference.TargetQualifiedName;

                if (!reference.IsValid)
                {
                    line += " (not valid)";
                }

                context.Trace.Writeline(line);
            }

            context.Trace.Writeline(Texts.Found__ + projectItem.References.Count());
        }
    }
}
