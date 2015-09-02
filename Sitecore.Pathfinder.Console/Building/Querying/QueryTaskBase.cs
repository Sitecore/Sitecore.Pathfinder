// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.References;

namespace Sitecore.Pathfinder.Building.Querying
{
    public abstract class QueryTaskBase : TaskBase
    {
        protected QueryTaskBase([NotNull] string taskName) : base(taskName)
        {
        }

        protected virtual void Display([NotNull] IBuildContext context, [NotNull] IEnumerable<IReference> references)
        {
            foreach (var reference in references)
            {
                string line = $"{reference.Owner.Snapshots.First().SourceFile.GetProjectPath(context.Project)}";

                var textNode = reference.SourceSourceProperty?.SourceTextNode;
                line += textNode != null ? $"({textNode.Position.LineNumber},{textNode.Position.LineNumber})" : "(0,0)";

                context.Trace.Writeline(line);
            }
        }
    }
}
