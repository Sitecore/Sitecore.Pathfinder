// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class FindReferences : QueryBuildTaskBase
    {
        public FindReferences() : base("find-references")
        {
        }

        public override void Run(IBuildContext context)
        {
            var qualifiedName = context.Configuration.GetCommandLineArg(0);
            if (string.IsNullOrEmpty(qualifiedName))
            {
                context.Trace.WriteLine(Texts.You_must_specific_the___name_argument);
                return;
            }

            var project = context.LoadProject();

            var projectItem = project.Indexes.FindQualifiedItem<IProjectItem>(qualifiedName);
            if (projectItem == null)
            {
                context.Trace.WriteLine(Texts.Project_item_not_found__ + qualifiedName);
                return;
            }

            foreach (var reference in projectItem.References)
            {
                string line = $"{reference.Owner.Snapshot.SourceFile.ProjectFileName}";

                var textNode = reference.TextNode;
                line += $"({textNode.TextSpan.LineNumber},{textNode.TextSpan.LineNumber})";

                line += ": " + reference.ReferenceText;

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
