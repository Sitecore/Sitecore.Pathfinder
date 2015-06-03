namespace Sitecore.Pathfinder.Building.Querying
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Extensions;
  using Sitecore.Pathfinder.Querying;

  [Export(typeof(ITask))]
  public class FindUsages : QueryTaskBase
  {
    public FindUsages() : base("usages")
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

      var references = queryService.FindUsages(context.Project, qualifiedName).ToList();

      this.Display(context, references);

      context.Trace.Writeline(Texts.Found__ + references.Count());
    }
  }
}
