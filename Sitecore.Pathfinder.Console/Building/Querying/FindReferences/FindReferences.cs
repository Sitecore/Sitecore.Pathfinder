namespace Sitecore.Pathfinder.Building.Querying.FindReferences
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;
  using Sitecore.Pathfinder.Querying;

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
        context.Trace.Writeline("You must specific the --name argument");
        return;
      }

      var queryService = context.CompositionService.Resolve<IQueryService>();

      var projectItem = queryService.FindProjectItem(context.Project, qualifiedName);
      if (projectItem == null)
      {
        context.Trace.Writeline("Project item not found: " + qualifiedName);
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

      context.Trace.Writeline("Found: " + projectItem.References.Count());
    }
  }
}