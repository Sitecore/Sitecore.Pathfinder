namespace Sitecore.Pathfinder.Building.Refactoring
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Building.Querying;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;
  using Sitecore.Pathfinder.Querying;

  [Export(typeof(ITask))]
  public class Rename : QueryTaskBase
  {
    public Rename() : base("rename")
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

      var newQualifiedName = context.Configuration.GetString("to");
      if (string.IsNullOrEmpty(qualifiedName))
      {
        context.Trace.Writeline(Texts.You_must_specific_the___to_argument);
        return;
      }

      var queryService = context.CompositionService.Resolve<IQueryService>();

      var references = queryService.FindUsages(context.Project, qualifiedName);

      foreach (var reference in references)
      {
        reference.SourceTextNode
      }
    }
  }
}