namespace Sitecore.Pathfinder.Building.Querying
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.References;

  public abstract class QueryTaskBase : TaskBase
  {
    protected QueryTaskBase([NotNull] string taskName) : base(taskName)
    {
    }

    protected virtual void Display([NotNull] IBuildContext context, [NotNull] IEnumerable<IReference> references)
    {
      foreach (var reference in references)
      {
        string line = $"{reference.Owner.Snapshot.SourceFile.GetProjectPath(context.Project)}";

        var textNode = reference.SourceTextNode;
        line += textNode != null ? $"({textNode.Position.LineNumber},{textNode.Position.LineNumber})" : "(0,0)";

        context.Trace.Writeline(line);
      }
    }
  }
}
