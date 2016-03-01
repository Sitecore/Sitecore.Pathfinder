// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Building.Tasks
{
    public abstract class QueryBuildTaskBase : BuildTaskBase
    {
        protected QueryBuildTaskBase([NotNull] string taskName) : base(taskName)
        {
        }

        protected virtual void Display([NotNull] IBuildContext context, [NotNull, ItemNotNull] IEnumerable<IReference> references)
        {
            foreach (var reference in references)
            {
                string line = $"{reference.Owner.Snapshots.First().SourceFile.ProjectItemName}";

                var textNode = TraceHelper.GetTextNode(reference.SourceProperty);
                line += $"({textNode.TextSpan.LineNumber},{textNode.TextSpan.LineNumber})";

                context.Trace.WriteLine(line);
            }
        }
    }
}
