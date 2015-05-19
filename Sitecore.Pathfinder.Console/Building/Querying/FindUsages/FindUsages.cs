namespace Sitecore.Pathfinder.Building.Querying.FindUsages
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;
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
        context.Trace.Writeline("You must specific the --name argument");
        return;
      }

      var queryService = context.CompositionService.Resolve<IQueryService>();

      var references = queryService.FindUsages(context.Project, qualifiedName).ToList();

      this.Display(context, references);

      context.Trace.Writeline("Found: " + references.Count());
    }
  }
}
