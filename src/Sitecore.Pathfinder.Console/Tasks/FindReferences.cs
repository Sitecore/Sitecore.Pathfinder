// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Querying;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class FindReferences : QueryBuildTaskBase
    {
        [ImportingConstructor]
        public FindReferences([NotNull] IQueryService queryService) : base("find-references")
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

            var projectItem = QueryService.FindProjectItem(context.Project, qualifiedName);
            if (projectItem == null)
            {
                context.Trace.WriteLine(Texts.Project_item_not_found__ + qualifiedName);
                return;
            }

            foreach (var reference in projectItem.References)
            {
                string line = $"{reference.Owner.Snapshots.First().SourceFile.ProjectItemName}";

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
    }
}
